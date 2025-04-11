using Raylib_cs;
using System.Numerics;

namespace VampireSurvivorsClone.Entities;

public class Player
{
    public Vector2 Position { get; private set; }
    private float speed = 200f;
    private int size = 32;
    private List<Projectile> projectiles = new();
    private float fireCooldown = 0.5f;
    private float fireTimer = 0f;
    public int Health { get; private set; } = 5;


    public Player()
    {
        // Center the player
        Position = new Vector2(1280 / 2, 720 / 2);
    }

    public List<Projectile> Projectiles => projectiles;

    public void TakeDamage(int amount)
    {
        Health -= amount;
    }

    public void Update()
    {
        Vector2 direction = Vector2.Zero;

        if (Raylib.IsKeyDown(KeyboardKey.KEY_W)) direction.Y -= 1;
        if (Raylib.IsKeyDown(KeyboardKey.KEY_S)) direction.Y += 1;
        if (Raylib.IsKeyDown(KeyboardKey.KEY_A)) direction.X -= 1;
        if (Raylib.IsKeyDown(KeyboardKey.KEY_D)) direction.X += 1;

        if (direction != Vector2.Zero)
        {
            direction = Vector2.Normalize(direction);
            Position += direction * speed * Raylib.GetFrameTime();
        }

        // Auto-fire logic
        fireTimer -= Raylib.GetFrameTime();
        if (fireTimer <= 0f)
        {
            FireProjectile();
            fireTimer = fireCooldown;
        }

        // Update projectiles
        foreach (var p in projectiles)
        {
            p.Update(Raylib.GetFrameTime());
        }

        // Remove dead ones
        projectiles.RemoveAll(p => !p.IsAlive);
    }

    private void FireProjectile()
    {
        var start = new Vector2(Position.X + size / 2, Position.Y);
        var dir = new Vector2(0, -1); // Straight up
        projectiles.Add(new Projectile(start, dir));
    }

    public void Draw()
    {
        Raylib.DrawRectangle((int)Position.X, (int)Position.Y, size, size, Color.BLUE);

        foreach (var p in projectiles)
        {
            p.Draw();
        }
    }
}
