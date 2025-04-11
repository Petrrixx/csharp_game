using Raylib_cs;
using System.Numerics;

namespace VampireSurvivorsClone.Entities;

public class XpGem
{
    public Vector2 Position;
    public bool IsCollected = false;
    private float size = 10f;

    public XpGem(Vector2 pos)
    {
        Position = pos;
    }

    public void Update(Vector2 playerPos)
    {
        if (Vector2.Distance(playerPos, Position) < 20f)
        {
            IsCollected = true;
        }
    }

    public void Draw()
    {
        Raylib.DrawCircle((int)Position.X, (int)Position.Y, size, Color.GREEN);
    }
}
