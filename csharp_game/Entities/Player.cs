using Raylib_cs;
using System.Numerics;
using VampireSurvivorsClone.Data;

namespace VampireSurvivorsClone.Entities;

public class Player
{
    public Vector2 Position { get; set; }
    private float speed = 200f;
    private int size = 32; 
    private float fireCooldown = 0.5f; // Cooldown time in seconds
    private float fireTimer = 0f; // Timer for firing projectiles
    private int health= 50; // Player's health
    private int strength = 10;
    private int agility = 1;
    private int dexterity = 1;
    private int level = 1; // Player's level
    public int Level { get => level; set => level = value; }  // Property for level
    public int Strength { get => strength; set => strength = value; }  // Property for strength
    public int Agility { get => agility; set => agility = value; }  // Property for agility
    public int Dexterity { get => dexterity; set => dexterity = value; }  // Property for dexterity
    public int Health { get => health; set => health = value; }  // Property for health
    private List<Projectile> projectiles = new();
    private List<Melee> meleeAttacks = new();
    private int xp = 0; // Player's experience points
    public int XP { get => xp; set => xp = value; }  // Property for XP
    public int AbilityPoints { get; set; } = 0; // Points to spend on upgrades

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
        Vector2 direction = Input.GetMovementDirection();

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

        foreach (var m in meleeAttacks)
        {
            m.Update(Raylib.GetFrameTime());
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
        projectiles.Add(new Projectile(start, FacingDirection, ProjectileType.Normal));
    }

    public void addProjectile(Projectile projectile)
    {
        projectiles.Add(projectile);
    }

    public void addMelee(Melee melee)
    {
        meleeAttacks.Add(melee);
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
