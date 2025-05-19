using System;
using System.Numerics;
using Raylib_cs;
using VampireSurvivorsClone.Data;

namespace VampireSurvivorsClone.Entities
{
    public class Enemy
    {
        public Vector2 Position { get; private set; }
        private float speed;
        private float size = 20f;
        public bool IsAlive { get; private set; } = true;
        private int health;
        private float lastAttackTime = -2f;  // To prevent immediate attack on spawn
        private float attackCooldown = 2f;   // 2 seconds cooldown between attacks
        private int damage;
        public int XPDrop { get; private set; } = 1;

        // Constructor for enemy
        public Enemy(Vector2 spawnPosition, EnemyData data)
        {
            Position = spawnPosition;
            health = data.Health;
            speed = data.Speed;
            damage = data.Damage;
            XPDrop = data.XP;
        }

        public void TakeDamage(int amount)
        {
            health -= amount;
            if (health <= 0)
                IsAlive = false;
        }

        public void Update(Player player, float deltaTime)
        {
            var playerPosition = player.Position;
            // Move towards the player
            Vector2 direction = playerPosition - Position;
            if (direction != Vector2.Zero)
            {
                direction = Vector2.Normalize(direction);
                Position += direction * speed * deltaTime;
            }

            // Attack cooldown logic (update it here only)
            lastAttackTime += deltaTime;

            // Check if enemy can attack the player (colliding + cooldown)
            if (Vector2.Distance(playerPosition, Position) < size && lastAttackTime >= attackCooldown)
            {
                // Attack the player
                player.TakeDamage(damage);
                lastAttackTime = 0f; // Reset cooldown after attack
            }
        }

        public void Draw()
        {
            Raylib.DrawTriangle(
                new Vector2(Position.X, Position.Y - size),
                new Vector2(Position.X - size, Position.Y + size),
                new Vector2(Position.X + size, Position.Y + size),
                Color.RED
            );
        }
    }
}
