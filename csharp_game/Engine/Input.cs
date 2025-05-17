using Raylib_cs;
using System.Numerics;

namespace VampireSurvivorsClone;

public static class Input
{
    // Method to get player movement direction
    public static Vector2 GetMovementDirection()
    {
        Vector2 direction = Vector2.Zero;

        if (Raylib.IsKeyDown(KeyboardKey.KEY_W)) direction.Y -= 1;
        if (Raylib.IsKeyDown(KeyboardKey.KEY_S)) direction.Y += 1;
        if (Raylib.IsKeyDown(KeyboardKey.KEY_A)) direction.X -= 1;
        if (Raylib.IsKeyDown(KeyboardKey.KEY_D)) direction.X += 1;

        return direction;
    }
}
