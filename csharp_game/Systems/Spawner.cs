using System.Numerics;
using VampireSurvivorsClone.Entities;
using VampireSurvivorsClone.Data;

namespace VampireSurvivorsClone.Systems
{
    public class Spawner
    {
        private Random rand = new();
        private int screenWidth;
        private int screenHeight;
        private List<Enemy> enemies;
        private Player player;

        public Spawner(int screenWidth, int screenHeight, List<Enemy> enemies, Player player)
        {
            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;
            this.enemies = enemies;
            this.player = player;
        }

        // Spawn an enemy based on the current wave number and spawn chance
        public void SpawnEnemy(int waveNumber)
        {
            // Randomly choose an enemy type based on the wave number
            EnemyType enemyType = GetRandomEnemyType(waveNumber);

            // Get the enemy data for the selected type
            EnemyData enemyData = EnemyData.GetEnemyData(enemyType, waveNumber);

            // Generate a random spawn position outside of the player's view
            Vector2 spawnPosition = GetRandomSpawnPosition();

            // Create a new enemy using the data and spawn position
            Enemy enemy = new Enemy(spawnPosition, enemyData);

            // Add the enemy to the list of enemies
            enemies.Add(enemy);
        }

        // Randomly determine the enemy type based on the wave number and spawn chance
        private EnemyType GetRandomEnemyType(int waveNumber)
        {
            float spawnChance = Math.Min(1f, waveNumber * 0.05f); // Increase spawn chance based on wave number
            float randomValue = (float)rand.NextDouble();

            // Probability of spawning a boss or rare enemy increases as wave number increases
            if (randomValue < 0.1f)
            {
                return EnemyType.Boss; // 10% chance to spawn a boss
            }
            else if (randomValue < 0.2f)
            {
                return EnemyType.Legendary; // 10% chance to spawn a legendary enemy
            }
            else if (randomValue < 0.35f)
            {
                return EnemyType.VeryRare; // 15% chance to spawn a very rare enemy
            }
            else if (randomValue < 0.65f)
            {
                return EnemyType.Rare; // 30% chance to spawn a rare enemy
            }
            else
            {
                return EnemyType.Common; // 35% chance to spawn a common enemy
            }
        }

        // Generate a random spawn position for the enemy, ensuring it's outside the player's view
        private Vector2 GetRandomSpawnPosition()
        {
            // Get the player's position
            Vector2 playerPos = player.Position;

            // Nastav spawn pásmo (napr. 400 až 900 pixelov od hráča)
            float minRadius = 400f;
            float maxRadius = 900f;

            double angle = rand.NextDouble() * Math.PI * 2;
            float radius = (float)(minRadius + rand.NextDouble() * (maxRadius - minRadius));
            float spawnX = playerPos.X + (float)Math.Cos(angle) * radius;
            float spawnY = playerPos.Y + (float)Math.Sin(angle) * radius;

            return new Vector2(spawnX, spawnY);
        }
    }
}
