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
    private float lastAttackTime = -2f;  // To prevent immediate attack on spawn
    private float attackCooldown = 2f;   // 2 seconds cooldown between attacks

    public float LastAttackTime { get => lastAttackTime; set => lastAttackTime = value; }
    public float AttackCooldown { get => attackCooldown; set => attackCooldown = value; }

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

    public void Update(Vector2 playerPosition, float deltaTime)
    {
         // Move towards the player
        Vector2 direction = playerPosition - Position;
        if (direction != Vector2.Zero)
        {
            direction = Vector2.Normalize(direction);
            Position += direction * speed * deltaTime;
        }

        // Attack cooldown logic
        lastAttackTime += deltaTime;

        // Check if enemy can attack the player (colliding + cooldown)
        if (Vector2.Distance(playerPosition, Position) < size && lastAttackTime >= attackCooldown)
        {
            lastAttackTime = 0f; // Reset cooldown
            
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


