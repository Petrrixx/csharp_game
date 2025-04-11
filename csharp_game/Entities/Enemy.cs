using Raylib_cs;
using System.Numerics;

namespace VampireSurvivorsClone.Entities;

public class Enemy
{
    public Vector2 Position { get; private set; }
    private float speed = 100f;
    private float size = 20f;
    public bool IsAlive { get; private set; } = true;
    private float health = 1f;

    public Enemy(Vector2 spawnPosition)
    {
        Position = spawnPosition;
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0)
            IsAlive = false;
    }

    public void Update(Vector2 playerPosition)
    {
        Vector2 direction = playerPosition - Position;
        if (direction != Vector2.Zero)
        {
            direction = Vector2.Normalize(direction);
            Position += direction * speed * Raylib.GetFrameTime();
        }
    }

    public void Draw()
    {
        Vector2 point1 = new(Position.X, Position.Y - size);
        Vector2 point2 = new(Position.X - size, Position.Y + size);
        Vector2 point3 = new(Position.X + size, Position.Y + size);

        Raylib.DrawTriangle(point1, point2, point3, Color.RED);
    }
}
