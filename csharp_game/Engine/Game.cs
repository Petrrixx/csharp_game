using Raylib_cs;
using VampireSurvivorsClone.Entities;
using VampireSurvivorsClone.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using VampireSurvivorsClone.UI;
using VampireSurvivorsClone.Systems;
using System.Formats.Asn1;

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
        private DifficultyPreset preset;
        private bool isBonusWave = false;
        private bool bossDefeated = false;
        private float bossDefeatedTimer = 0f;
        private bool isPaused = false;

        public Game(int sizeW, int sizeH, DifficultyPreset preset)
        {
            this.sizeW = sizeW;
            this.sizeH = sizeH;

            this.preset = preset;

            player = new Player(preset);
            gameTimer = new Timer();
            spawner = new Spawner(sizeW, sizeH, enemies, player, preset);  // Initialize the Spawner with screen size and enemies list + preset
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
            // Pause toggle
            if (Input.IsActionPressed("Pause") && !levelUpMenu.IsActive && !isWaveChanging)
            {
                isPaused = !isPaused;
            }

            if (isPaused)
            {
                // E = Quit Game
                if (Input.isActionPressed("Quit"))
                {
                    Raylib.CloseWindow();
                    System.Environment.Exit(0);
                }
                return;
            }

            // Check if the game is paused by Level Up Menu
            if (levelUpMenu.IsActive)
            {
                levelUpMenu.Update();
                return;
            }

            // Update the player
            if (!isWaveChanging)
            {
                player.Update();
            }

            // Update the game timer
            if (isBonusWave)
            {
                // Handle the bonus wave
                if (enemies.Count == 0 && !bossDefeated)
                {
                    // Spawn Demon King
                    var demonKingData = EnemyData.GetEnemyData(EnemyType.DemonKing, waveNumber, preset);
                    Vector2 bossSpawn = spawner.GetRandomSpawnPosition();
                    enemies.Add(new Enemy(bossSpawn, demonKingData));
                    // Ak treba, resetni hráča do stredu
                    player.Position = new Vector2(sizeW / 2, sizeH / 2);
                }
                if (bossDefeated)
                {
                    bossDefeatedTimer -= Raylib.GetFrameTime();
                    if (bossDefeatedTimer <= 0f)
                    {
                        isBonusWave = false;
                        isWaveChanging = false;
                    }
                }
                return;
            }

            gameTimer.Update(Raylib.GetFrameTime());

            // Check for wave transition (every 30 seconds)
            if (gameTimer.IsWaveReady && !isWaveChanging)
            {
                isWaveChanging = true;
                waveNumber++;
                spawnChance += 0.1f;
                spawnInterval = Math.Max(0.5f, spawnInterval - 0.1f);
                gameTimer.Reset(30f);

                // Remove all existing entities for the new wave
                enemies.Clear();
                player.Projectiles.Clear();
                xpGems.Clear();

                // BONUS WAVE: 5% chance to spawn Demon King
                isBonusWave = false;
                bossDefeated = false;
                bossDefeatedTimer = 0f;
                if (new Random().NextDouble() < 0.05)
                {
                    isBonusWave = true;
                    // Spawn Demon King using spawner's GetRandomSpawnPosition
                    var demonKingData = EnemyData.GetEnemyData(EnemyType.DemonKing, waveNumber, preset);
                    Vector2 bossSpawn = spawner.GetRandomSpawnPosition();
                    enemies.Add(new Enemy(bossSpawn, demonKingData));
                }
                else
                {
                    spawner.SpawnEnemy(waveNumber);
                }

                player.Position = new Vector2(sizeW / 2, sizeH / 2);
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

            // Projectile-enemy collision
            foreach (var enemy in enemies.ToList())
            {
                foreach (var proj in player.Projectiles)
                {
                    if (Vector2.Distance(enemy.Position, proj.Position) < 20f)
                    {
                        if (proj.TypeValue == ProjectileType.Explosive)
                        {
                            foreach (var aoeEnemy in enemies)
                            {
                                if (Vector2.Distance(proj.Position, aoeEnemy.Position) < 60f)
                                    aoeEnemy.TakeDamage(proj.DamageValue + player.Strength);
                            }
                            proj.Lifetime = 0;
                        }
                        else if (proj.TypeValue == ProjectileType.Shuriken)
                        {
                            // Shuriken logic: if it hits an enemy, it will pierce through and continue
                            if (proj.HitEnemies == null)
                                proj.HitEnemies = new HashSet<Enemy>();

                            if (!proj.HitEnemies.Contains(enemy))
                            {
                                enemy.TakeDamage(proj.DamageValue + player.Strength);
                                proj.HitEnemies.Add(enemy);
                                proj.PierceCount--;
                                if (proj.PierceCount <= 0)
                                    proj.Lifetime = 0;
                            }
                        }
                        else
                        {
                            enemy.TakeDamage(proj.DamageValue + player.Strength);
                            proj.Lifetime = 0;
                        }
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
            
            if (player.WeaponInventory.Any(w => w.Name == "Garlic"))
            {
                int garlicLevel = player.GetWeaponLevel("Garlic");
                // Garlic damage and radius based on level
                int circleCount = garlicLevel >= 5 ? 3 : (garlicLevel >= 3 ? 2 : 1);
                float[] radii = circleCount switch
                {
                    1 => new[] { 60f },
                    2 => new[] { 60f, 90f },
                    3 => new[] { 60f, 90f, 120f },
                    _ => new[] { 60f }
                };
                float[] dps = circleCount switch
                {
                    1 => new[] { 5f + (garlicLevel - 1) * 2f },
                    2 => new[] { 7f, 10f },
                    3 => new[] { 10f, 15f, 20f },
                    _ => new[] { 5f }
                };

                // Apply damage to enemies within the garlic circles
                for (int i = 0; i < circleCount; i++)
                {
                    foreach (var enemy in enemies)
                    {
                        if (enemy.IsAlive && Vector2.Distance(player.Position, enemy.Position) < radii[i])
                        {
                            enemy.TakeDamage((int)(dps[i] * Raylib.GetFrameTime()));
                        }
                    }
                }
            }
        }

        public void Draw()
        {
            if (levelUpMenu.IsActive)
            {
                levelUpMenu.Draw();
                return;
            }

            // Second priority, we cant quit during Level Up Menu
            if (isPaused)
            {
                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.DARKGRAY);
                // Blur effect can be simulated with a semi-transparent rectangle
                Raylib.DrawRectangle(0, 0, Raylib.GetScreenWidth(), Raylib.GetScreenHeight(), new Color(0, 0, 0, 180));
                Raylib.DrawText("GAME PAUSED", 480, 300, 50, Color.YELLOW);
                Raylib.DrawText("Press E to return to Quit Game", 420, 400, 30, Color.LIGHTGRAY);
                Raylib.EndDrawing();
                return;
            }

            if (isBonusWave)
            {
                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.DARKGRAY);
                Raylib.DrawText("Defeat the Boss", 480, 20, 40, Color.RED);
                if (bossDefeated)
                    Raylib.DrawText("Boss Defeated! All your stats are boosted for 2 minutes.", 200, 100, 30, Color.GOLD);
                Raylib.EndDrawing();
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
