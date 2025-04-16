using Raylib_cs;
using System.Numerics;

namespace VampireSurvivorsClone.Entities;

public class Projectile
{
    public Vector2 Position;
    public Vector2 Direction;
    public float Speed = 400f;
    public float Lifetime = 2f;
    public bool IsAlive => Lifetime > 0;

    private float size = 6f;

    public Projectile(Vector2 startPosition, Vector2 direction)
    {
        Position = startPosition;
        Direction = Vector2.Normalize(direction);  // Normalize direction for consistent speed
    }

    public void Update(float deltaTime)
    {
        Position += Direction * Speed * deltaTime;
        Lifetime -= deltaTime;
    }

    public void Draw()
    {
        Raylib.DrawRectangle((int)Position.X, (int)Position.Y, (int)size, (int)size, Color.YELLOW);
    }
}
