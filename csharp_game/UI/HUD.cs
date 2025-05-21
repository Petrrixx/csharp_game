using Raylib_cs;
using VampireSurvivorsClone.Entities;

namespace VampireSurvivorsClone.UI
{
    public class HUD
    {
        // Draw all static elements of the HUD (health, xp, wave, timer)
        public static void Draw(Player player, int playerXP, int waveNumber, VampireSurvivorsClone.Engine.Timer gameTimer)
        {
            // XP bar parameters
            int barX = 0;
            int barY = 0;
            int barWidth = 1280;
            int barHeight = 24;

            // XP needed for next level
            int xpNeeded = 5 + player.Level * 5;
            float xpRatio = Math.Clamp((float)playerXP / xpNeeded, 0f, 1f);

            // Draw XP bar background
            Raylib.DrawRectangle(barX, barY, barWidth, barHeight, Color.DARKBLUE);
            // Draw XP bar fill
            Raylib.DrawRectangle(barX, barY, (int)(barWidth * xpRatio), barHeight, Color.BLUE);
            // Draw XP text
            Raylib.DrawText($"XP: {playerXP} / {xpNeeded}", barX + 10, barY + 2, 20, Color.LIGHTGRAY);

            // Draw player level (top left, under XP bar)
            Raylib.DrawText($"Level: {player.Level}", 10, barY + barHeight + 8, 24, Color.YELLOW);

            // Display Health
            Raylib.DrawText($"Health: {player.Health}", 10, barY + barHeight + 40, 20, Color.RED);

            // Display Current Wave
            Raylib.DrawText($"Wave {waveNumber}", 10, barY + barHeight + 70, 30, Color.LIME);

            // Display game timer
            Raylib.DrawText($"Time: {Math.Ceiling(gameTimer.TimeRemaining)}", 1150, 10, 20, Color.RED);
        }
    }
}
