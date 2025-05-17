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

        public Spawner(int screenWidth, int screenHeight, List<Enemy> enemies)
        {
            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;
            this.enemies = enemies;
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
            int minOffset = 200;
            int maxOffsetX = screenWidth / 2 + 100;
            int maxOffsetY = screenHeight / 2 + 100;

            // Randomly choose a side to spawn the enemy
            int side = rand.Next(0, 4);
            float spawnX = screenWidth / 2;  // Start at the center
            float spawnY = screenHeight / 2;

            switch (side)
            {
                case 0: // Left
                    spawnX -= rand.Next(minOffset, maxOffsetX);
                    spawnY += rand.Next(-maxOffsetY, maxOffsetY);
                    break;
                case 1: // Right
                    spawnX += rand.Next(minOffset, maxOffsetX);
                    spawnY += rand.Next(-maxOffsetY, maxOffsetY);
                    break;
                case 2: // Top
                    spawnX += rand.Next(-maxOffsetX, maxOffsetX);
                    spawnY -= rand.Next(minOffset, maxOffsetY);
                    break;
                case 3: // Bottom
                    spawnX += rand.Next(-maxOffsetX, maxOffsetX);
                    spawnY += rand.Next(minOffset, maxOffsetY);
                    break;
            }

            return new Vector2(spawnX, spawnY);
        }
    }
}
