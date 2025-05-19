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
        public float SizeValue { get => Size; set => Size = value; }
        private int Damage;
        public int DamageValue { get => Damage; set => Damage = value; }
        private ProjectileType Type;
        public ProjectileType TypeValue => Type;

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

        public void Update(float deltaTime, List<Enemy> enemies = null)
        {
            if (Type == ProjectileType.Homing && enemies != null && enemies.Count > 0)
            {
                // Najdi najbližšieho nepriateľa
                Enemy closest = null;
                float minDist = float.MaxValue;
                foreach (var e in enemies)
                {
                    float dist = Vector2.Distance(Position, e.Position);
                    if (dist < minDist)
                    {
                        minDist = dist;
                        closest = e;
                    }
                }
                if (closest != null)
                {
                    Direction = Vector2.Normalize(closest.Position - Position);
                }
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

            if (Type == ProjectileType.Explosive)
                Raylib.DrawCircleV(Position, Size * 1.2f, color);
            else
                Raylib.DrawRectangle((int)Position.X, (int)Position.Y, (int)Size, (int)Size, color);
        }
    }
}
