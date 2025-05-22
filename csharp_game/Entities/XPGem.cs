using Raylib_cs;
using System.Numerics;

namespace VampireSurvivorsClone.Entities;

public class XpGem
{
    public Vector2 Position;
    public bool IsCollected = false;
    private float size = 10f;
    public int XPValue { get; set; } = 1;

    public XpGem(Vector2 pos, int xpValue = 1)
    {
        Position = pos;
        this.XPValue = xpValue;
    }

    public void Update(Vector2 playerPos)
    {
        float playerRadius = Player.SizeValue;
        float gemRadius = size;
        if (Vector2.Distance(playerPos, Position) < playerRadius + gemRadius)
        {
            IsCollected = true;
        }
    }

    public void Draw()
    {
        Raylib.DrawCircle((int)Position.X, (int)Position.Y, size, Color.GREEN);
    }
}
