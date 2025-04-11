using Raylib_cs;
using System.Numerics;
using VampireSurvivorsClone.Entities;

namespace VampireSurvivorsClone.Engine;

public class Game
{
    private Player player;
    private List<Enemy> enemies = new();
    private float spawnTimer = 0f;
    private float spawnInterval = 2f;
    private List<XpGem> xpGems = new();
    private int playerXP = 0;

    public Game()
    {
        player = new Player();

        // Spawn 1 starter enemy
        SpawnEnemy();
    }

    private void SpawnEnemy()
    {
        Random rand = new();
        float x = rand.Next(0, 1280);
        float y = rand.Next(0, 720);
        enemies.Add(new Enemy(new Vector2(x, y)));
    }

    public void Update()
    {
        player.Update();

        // Spawn logic
        spawnTimer -= Raylib.GetFrameTime();
        if (spawnTimer <= 0)
        {
            SpawnEnemy();
            spawnTimer = spawnInterval;
        }

        // Enemy collision with player
        foreach (var enemy in enemies)
        {
            if (Vector2.Distance(enemy.Position, player.Position) < 20f)
            {
                player.TakeDamage(1);
                // Optional: knockback or destroy enemy
            }
        }

        // Update enemies
        foreach (var enemy in enemies)
        {
            enemy.Update(player.Position);
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

        // Update XP gems
        foreach (var gem in xpGems)
        {
            gem.Update(player.Position);
            if (gem.IsCollected) playerXP++;
        }
        xpGems.RemoveAll(g => g.IsCollected);

        // Remove dead enemies
        enemies.RemoveAll(e => !e.IsAlive);
    }

    public void Draw()
    {
        Raylib.BeginDrawing();
        Raylib.ClearBackground(Color.DARKGRAY);

        player.Draw();
        foreach (var enemy in enemies)
            enemy.Draw();

        foreach (var gem in xpGems)
            gem.Draw();

        Raylib.DrawText($"XP: {playerXP}", 10, 10, 20, Color.LIME);

        Raylib.DrawText($"Health: {player.Health}", 10, 40, 20, Color.RED);

        Raylib.EndDrawing();
    }
}
