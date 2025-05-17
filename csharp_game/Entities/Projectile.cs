using System.Numerics;
using Raylib_cs;
using VampireSurvivorsClone.Data;

namespace VampireSurvivorsClone.Entities
{
    public class Projectile
    {
        public Vector2 Position;
        public Vector2 Direction;
        public float Speed;
        public float Lifetime;
        public bool IsAlive => Lifetime > 0;

        private float Size;
        private int Damage;
        public int DamageValue => Damage;
        private ProjectileType Type;

        public Projectile(Vector2 startPosition, Vector2 direction, ProjectileType type)
        {
            var data = ProjectileData.GetProjectileData(type);
            Position = startPosition;
            Direction = Vector2.Normalize(direction);
            Speed = data.Speed;
            Lifetime = data.Lifetime;
            Size = data.Size;
            Damage = data.Damage;
            Type = type;
        }

        public void Update(float deltaTime)
        {
            if (Type == ProjectileType.Homing)
            {
                // Add homing behavior: Make the projectile seek the player or enemies
                // For now, let's assume we just point it in the direction of the target
            }

            Position += Direction * Speed * deltaTime;
            Lifetime -= deltaTime;
        }

        public void Draw()
        {
            Color color = Type switch
            {
                ProjectileType.Normal => Color.YELLOW,
                ProjectileType.Homing => Color.GREEN,
                ProjectileType.Explosive => Color.RED,
                ProjectileType.Piercing => Color.BLUE,
                _ => Color.YELLOW
            };

            Raylib.DrawRectangle((int)Position.X, (int)Position.Y, (int)Size, (int)Size, color);
        }
    }
}
