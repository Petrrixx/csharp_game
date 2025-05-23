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
        private EnemyType type;
        private float shootTimer = 0f;
        private float shootInterval = 0.5f; // 2 projektily za sekundu

        // Constructor for enemy
        public Enemy(Vector2 spawnPosition, EnemyData data)
        {
            Position = spawnPosition;
            health = data.Health;
            speed = data.Speed;
            damage = data.Damage;
            XPDrop = data.XP;
            type = data.Type;
        }

        public void TakeDamage(int amount)
        {
            health -= amount;
            if (health <= 0)
                IsAlive = false;
        }

        public void Update(Player player, float deltaTime)
        {
            switch (type)
            {
                case EnemyType.DemonKing:
                    // Shooting projectiles at the player
                    shootTimer -= deltaTime;
                    if (shootTimer <= 0f)
                    {
                        Vector2 dir = Vector2.Normalize(player.Position - Position);
                        player.addProjectile(new Projectile(Position, dir, ProjectileType.Piercing));
                        shootTimer = shootInterval;
                    }
                    return;

                case EnemyType.Mimic:
                // Mimic is a special enemy that mimics the players base stats and weapons
                shootTimer -= deltaTime;
                if (shootTimer <= 0f)
                {
                    Vector2 dir = Vector2.Normalize(player.Position - Position);
                    //player.addProjectile(new Projectile(Position, dir, ProjectileType.Normal));
                    shootTimer = 1f; // Once every second
                }
                break;

                case EnemyType.Legendary:
                    // Very slow, lot of HP, and occasional projectiles
                    shootTimer -= deltaTime;
                    if (shootTimer <= 0f)
                    {
                        Vector2 dir = Vector2.Normalize(player.Position - Position);
                        //player.addProjectile(new Projectile(Position, dir, ProjectileType.Normal));
                        shootTimer = 4f; // 4 seconds between shots
                    }
                    break;

                case EnemyType.VeryRare:
                    // Slow, more HP
                    break;

                case EnemyType.Rare:
                    // Balanced
                    break;

                case EnemyType.Common:
                    // Fast, but weak
                    speed += 2f; // Common enemies are faster
                    if (speed > player.Speed)
                        speed = player.Speed - 40; // Cap speed to Player's max speed
                    if (speed > 150)
                    {
                        speed = 150; // Cap speed to 150
                    }
                    break;
            }

            // Movement towards the player
            var playerPosition = player.Position;
            Vector2 direction = playerPosition - Position;
            if (direction != Vector2.Zero)
            {
                direction = Vector2.Normalize(direction);
                Position += direction * speed * deltaTime;
            }

            // Attack logic
            lastAttackTime += deltaTime;
            if (Vector2.Distance(playerPosition, Position) < size && lastAttackTime >= attackCooldown)
            {
                player.TakeDamage(damage);
                lastAttackTime = 0f;
            }
        }

        public void Draw()
        {
            if (type == EnemyType.Mimic)
                Raylib.DrawRectangle((int)Position.X, (int)Position.Y, 32, 32, Color.WHITE);
            else
                Raylib.DrawTriangle(
                    new Vector2(Position.X, Position.Y - size),
                    new Vector2(Position.X - size, Position.Y + size),
                    new Vector2(Position.X + size, Position.Y + size),
                    Color.RED
                );
        }
    }
}
