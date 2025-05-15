using Raylib_cs;
using System.Numerics;

namespace VampireSurvivorsClone.Entities
{
    public class Melee
    {
        public Vector2 Position;
        public Vector2 Direction;
        public float Range = 50f;  // Range of the melee attack
        public float Damage = 20f; // Melee damage
        public float Size = 20f;   // Size of the melee area (could represent an arc or radius)

        public Melee(Vector2 startPosition, Vector2 direction)
        {
            Position = startPosition;
            Direction = Vector2.Normalize(direction);
        }

        public void Update(float deltaTime)
        {
            // Melee doesn't move, but it could have an effect, like checking collision with enemies in range
            // Add logic for attacking and hitting enemies here
        }

        public void Draw()
        {
            // Draw a representation of the melee attack
            Raylib.DrawCircleV(Position + Direction * Range, Size, Color.ORANGE);
        }
    }
}
