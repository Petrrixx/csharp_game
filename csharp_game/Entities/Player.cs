using Raylib_cs;
using System.Numerics;

namespace VampireSurvivorsClone.Entities;

public class Player
{
    public Vector2 Position { get; set; }
    private float speed = 200f;
    private int size = 32;
    private float fireCooldown = 0.5f;
    private float fireTimer = 0f;
    private int health= 5;

    public int Health {get => health; set => health = value; }  // Property for health
    private List<Projectile> projectiles = new();

    public Player()
    {
        // Center the player
        Position = new Vector2(1280 / 2, 720 / 2);
    }

    public List<Projectile> Projectiles => projectiles;

    public void TakeDamage(int amount)
    {
        Health -= amount;
        if (Health < 0) Health = 0;  // Prevent health from going below 0
    }

    public Vector2 FacingDirection { get; private set; } = new Vector2(0, -1);  // Default: facing upwards

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
            FacingDirection = direction;  // Update facing direction
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

        // Check for game over if health <= 0
        if (Health <= 0)
        {
            // Display Game Over screen and exit
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.BLACK);
            Raylib.DrawText("GAME OVER", 520, 360, 50, Color.RED);
            Raylib.EndDrawing();
            System.Threading.Thread.Sleep(2000);  // Wait 2 seconds before quitting
            Raylib.CloseWindow();  // Close the window
            System.Environment.Exit(0);  // Exit the game
        }
    }

   private void FireProjectile()
{
    var start = new Vector2(Position.X + size / 2, Position.Y);
    // Use the player's facing direction to fire the projectile
    projectiles.Add(new Projectile(start, FacingDirection));
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
