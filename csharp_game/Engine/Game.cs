using Raylib_cs;
using VampireSurvivorsClone.Entities;
using VampireSurvivorsClone.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using VampireSurvivorsClone.UI;
using VampireSurvivorsClone.Systems;

namespace VampireSurvivorsClone.Engine
{
    public class Game
    {
        private Player player;
        private List<Enemy> enemies = new();
        private List<XpGem> xpGems = new();
        private int playerXP = 0;
        private Timer gameTimer;
        private int waveNumber = 1;
        private float spawnChance = 0.1f;
        private bool isWaveChanging = false;  // To track if wave change is happening
        private int sizeW;
        private int sizeH;
        private Camera2D camera;
        private Spawner spawner;  // Add the Spawner to handle enemy spawning
        private float spawnInterval = 1f;  // Declare spawn interval for spawning control
        private float spawnTimer = 0f;  // Declare spawn timer for enemy spawning
        private LevelUpMenu levelUpMenu;

        public Game(int sizeW, int sizeH)
        {
            this.sizeW = sizeW;
            this.sizeH = sizeH;

            player = new Player();
            gameTimer = new Timer();
            spawner = new Spawner(sizeW, sizeH, enemies, player);  // Initialize the Spawner with screen size and enemies list
            levelUpMenu = new LevelUpMenu(player);  // Initialize the LevelUpMenu with the player

            camera = new Camera2D
            {
                target = player.Position,
                offset = new Vector2(sizeW / 2, sizeH / 2),
                rotation = 0.0f,
                zoom = 1.0f
            };

            // Initial wave setup
            spawner.SpawnEnemy(waveNumber);
        }

        public void Update()
        {
            // Ak je aktívne level up menu, update iba menu a return
            if (levelUpMenu.IsActive)
            {
                levelUpMenu.Update();
                return;
            }

            if (!isWaveChanging)
            {
                player.Update();
            }

            gameTimer.Update(Raylib.GetFrameTime());

            // Check for wave transition (every 30 seconds)
            if (gameTimer.IsWaveReady && !isWaveChanging)
            {
                isWaveChanging = true;
                waveNumber++;  // Increment the wave number
                spawnChance += 0.1f;  // Increase spawn chance for next wave
                spawnInterval = Math.Max(0.5f, spawnInterval - 0.1f);  // Faster spawn interval
                gameTimer.Reset(30f);  // Reset timer for the next wave

                // Remove all existing entities for the new wave
                enemies.Clear();
                player.Projectiles.Clear();
                xpGems.Clear();

                // Spawn new enemies for the next wave
                spawner.SpawnEnemy(waveNumber);

                // Reset player position to the center of the screen
                player.Position = new Vector2(sizeW / 2, sizeH / 2); // Reset player position to center using defined window sizes

                // Display wave transition
                DisplayWaveTransition();
            }

            // Spawn enemies based on spawn chance
            spawnTimer -= Raylib.GetFrameTime();
            if (spawnTimer <= 0f && RandomSpawnCheck())
            {
                spawner.SpawnEnemy(waveNumber);  // Use the spawner to handle enemy creation
                spawnTimer = spawnInterval;
            }

            // Update enemies and handle collision with player
            foreach (var enemy in enemies.ToList())
            {
                enemy.Update(player, Raylib.GetFrameTime());  // Pass player position and delta time
            }

            // Bullet-enemy collision and gem collection
            foreach (var enemy in enemies.ToList())
            {
                foreach (var proj in player.Projectiles)
                {
                    if (Vector2.Distance(enemy.Position, proj.Position) < 20f)
                    {
                        if (proj.TypeValue == ProjectileType.Explosive)
                        {
                            // AoE damage: poškodenie všetkým nepriateľom v okruhu
                            foreach (var aoeEnemy in enemies)
                            {
                                if (Vector2.Distance(proj.Position, aoeEnemy.Position) < 60f) // polomer výbuchu
                                    aoeEnemy.TakeDamage(proj.DamageValue + player.Strength);
                            }
                        }
                        else
                        {
                            enemy.TakeDamage(proj.DamageValue + player.Strength);
                        }
                        proj.Lifetime = 0;
                    }
                }
            }

            // Update melee attacks and check for collision with enemies
            foreach (var melee in player.MeleeAttacks)
            {
                foreach (var enemy in enemies)
                {
                    if (Vector2.Distance(melee.Position + melee.Direction * melee.Range, enemy.Position) < melee.Size + 10f)
                    {
                        enemy.TakeDamage((int)melee.Damage + player.Damage);
                        melee.TriggerHitEffect();
                    }
                }
            }

            // Update projectiles
            foreach (var p in player.Projectiles)
            {
                p.Update(Raylib.GetFrameTime(), this.enemies);
            }

            // Update XP gems
            foreach (var gem in xpGems)
            {
                gem.Update(player.Position);
                if (gem.IsCollected) playerXP += gem.XPValue + player.Luck;  // Add XP based on player's luck
                player.XP = playerXP;  // Update player's XP
            }
            xpGems.RemoveAll(g => g.IsCollected);

            // Add XP gems for all newly dead enemies (projektil aj melee kill)
            foreach (var enemy in enemies.ToList())
            {
                if (!enemy.IsAlive && !xpGems.Any(g => Vector2.Distance(g.Position, enemy.Position) < 1f))
                {
                    xpGems.Add(new XpGem(enemy.Position, enemy.XPDrop));
                }
            }

            // Remove dead enemies
            enemies.RemoveAll(e => !e.IsAlive);

            // Toggle fullscreen on key press (F11)
            if (Raylib.IsKeyPressed(KeyboardKey.KEY_F11))
            {
                Raylib.ToggleFullscreen();
            }

            // Update the camera target to follow the player
            camera.target = player.Position;

            // Adjust the camera position if it's in fullscreen mode
            if (Raylib.IsWindowFullscreen())
            {
                // Calculate the center of the screen based on the current window size
                camera.offset = new Vector2(Raylib.GetScreenWidth() / 2, Raylib.GetScreenHeight() / 2);
            }
            else
            {
                // Set the camera offset to the center of the window
                camera.offset = new Vector2(sizeW / 2, sizeH / 2);
            }

            // XP level up check
            int xpNeeded = 5 + (player.Level * 5);
            if (player.XP >= xpNeeded)
            {
                levelUpMenu.Open();
                playerXP -= xpNeeded;    
                player.XP -= xpNeeded;     
            }
        }

        public void Draw()
        {
            if (levelUpMenu.IsActive)
            {
                levelUpMenu.Draw();
                return;
            }

            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.DARKGRAY);

            Raylib.BeginMode2D(camera); // Start 2D mode with camera

            // Draw the player, enemies, and XP gems
            player.Draw();
            foreach (var enemy in enemies)
                enemy.Draw();

            foreach (var gem in xpGems)
                gem.Draw();

            Raylib.EndMode2D();

            // Draw the HUD (static, outside camera mode)
            HUD.Draw(player, playerXP, waveNumber, gameTimer);

            Raylib.EndDrawing();
        }

        private bool RandomSpawnCheck()
        {
            Random rand = new();
            return rand.NextDouble() < spawnChance;  // Random chance based on wave
        }

        private void DisplayWaveTransition()
        {
            // Display wave number on the screen for 2 seconds
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.DARKGRAY);

            Raylib.DrawText($"Wave {waveNumber}", 520, 360, 50, Color.RED);

            Raylib.EndDrawing();

            System.Threading.Thread.Sleep(2000);  // Pause for 2 seconds to show wave number

            // After the transition, allow the game to continue
            isWaveChanging = false;
        }
    }
}
