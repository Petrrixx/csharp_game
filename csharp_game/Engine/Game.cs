using Raylib_cs;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using VampireSurvivorsClone.Entities;

namespace VampireSurvivorsClone.Engine;

public class Game
{
    private Player player;
    private List<Enemy> enemies = new();
    private float spawnTimer = 0f;
    private float spawnInterval = 1f;
    private List<XpGem> xpGems = new();
    private int playerXP = 0;
    private Timer gameTimer;
    private int waveNumber = 1;
    private float spawnChance = 0.1f;
    private bool isWaveChanging = false;  // To track if wave change is happening
    private int sizeW;
    private int sizeH;
    private Camera2D camera;

    public Game(int sizeW, int sizeH)
    {
        this.sizeH = sizeH;
        this.sizeW = sizeW;

        player = new Player();
        gameTimer = new Timer();

        camera = new Camera2D {
            target = player.Position,
            offset = new Vector2(sizeW / 2, sizeH / 2),
            rotation = 0.0f,
            zoom = 1.0f
        };

        // Spawn 1 starter enemy
        SpawnEnemy();
    }

    public void Update()
    {
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

            // Remove all existing enemies for the new wave
            enemies.Clear();

            // Reset player position to the center of the screen
            player.Position = new Vector2(sizeW / 2, sizeH / 2); // Reset player position to center using defined window sizes

            // Display wave transition
            DisplayWaveTransition();
        }

        // Spawn enemies based on spawn chance
        if (!isWaveChanging)
        {
            spawnTimer -= Raylib.GetFrameTime();
            if (spawnTimer <= 0f && RandomSpawnCheck())
            {
                SpawnEnemy();
                spawnTimer = spawnInterval;
            }
        }

        // Update enemies and handle collision with player
        foreach (var enemy in enemies.ToList())
        {
            enemy.Update(player.Position, Raylib.GetFrameTime());  // Pass player position and delta time

            // Enemy attacks the player if they collide
            if (Vector2.Distance(enemy.Position, player.Position) < 30f && enemy.IsAlive)
            {
                // Only attack if enough cooldown has passed
                if (enemy.LastAttackTime >= enemy.AttackCooldown)
                {
                    player.TakeDamage(1);
                    enemy.LastAttackTime = 0f;  // Reset attack cooldown
                }
                else
                {
                    enemy.LastAttackTime += Raylib.GetFrameTime();  // Increment cooldown timer
                }
            }
        }

        // Bullet-enemy collision and gem collection
        foreach (var enemy in enemies.ToList())
        {
            foreach (var proj in player.Projectiles)
            {
                if (Vector2.Distance(enemy.Position, proj.Position) < 20f)
                {
                    enemy.TakeDamage(1f);
                    proj.Lifetime = 0;

                    if (!enemy.IsAlive)
                    {
                        xpGems.Add(new XpGem(enemy.Position));
                    }
                }
            }
        }

        // Update enemy attack cooldowns
        foreach (var enemy in enemies)
        {
            if (enemy.IsAlive)
            {
            enemy.LastAttackTime += Raylib.GetFrameTime();
            }
        }

        // Update XP gems
        foreach (var gem in xpGems)
        {
            gem.Update(player.Position);
            if (gem.IsCollected) playerXP++;
        }
        xpGems.RemoveAll(g => g.IsCollected);

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
    }

    public void Draw()
    {
        Raylib.BeginDrawing();
        Raylib.ClearBackground(Color.DARKGRAY);

        Raylib.BeginMode2D(camera); // Start 2D mode with camera

        // Display timer countdown in the top-right corner
        Raylib.DrawText($"Time: {Math.Ceiling(gameTimer.TimeRemaining)}", 1150, 10, 20, Color.RED);

        player.Draw();
        foreach (var enemy in enemies)
            enemy.Draw();

        foreach (var gem in xpGems)
            gem.Draw();

        Raylib.DrawText($"XP: {playerXP}", 10, 10, 20, Color.LIME);

        Raylib.DrawText($"Health: {player.Health}", 10, 40, 20, Color.RED);

        // Display current wave
        Raylib.DrawText($"Wave {waveNumber}", 10, 10, 30, Color.LIME);

        Raylib.EndMode2D();

        Raylib.EndDrawing();
    }

    // ERROR - unhandled exception - rand(minvalue exceeds maxvalue)
    // This method spawns an enemy at a random position outside the player's view
    private void SpawnEnemy()
    {
        Random rand = new();

        // Set the minimum spawn distance from the player to 10% of the screen size (but at least 1000 pixels)
        float minOffset = Math.Max(1000, Math.Min(sizeW, sizeH) * 0.1f);  // 10% of the smaller dimension, but no less than 1000 pixels
        
        // Set the maximum spawn offset to the full screen size
        float maxOffsetX = sizeW;
        float maxOffsetY = sizeH;

        // Randomly choose the spawn offset within valid limits
        float offsetX = rand.Next((int)minOffset, (int)maxOffsetX);  // X offset for spawning
        float offsetY = rand.Next((int)minOffset, (int)maxOffsetY);  // Y offset for spawning

        // Randomly decide spawn direction (left/right and top/bottom relative to the player)
        float spawnX = player.Position.X + (rand.Next(0, 2) == 0 ? 1 : -1) * offsetX;
        float spawnY = player.Position.Y + (rand.Next(0, 2) == 0 ? 1 : -1) * offsetY;

        // Add the enemy to the list with the calculated spawn position
        enemies.Add(new Enemy(new Vector2(spawnX, spawnY)));
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


