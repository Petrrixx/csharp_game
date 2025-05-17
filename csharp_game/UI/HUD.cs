using Raylib_cs;
using VampireSurvivorsClone.Entities;

namespace VampireSurvivorsClone.UI
{
    public class HUD
    {
        // Draw all static elements of the HUD (health, xp, wave, timer)
        public static void Draw(Player player, int playerXP, int waveNumber, VampireSurvivorsClone.Engine.Timer gameTimer)
        {
            Raylib.DrawText($"XP: {playerXP}", 10, 10, 20, Color.LIME); // Display XP
            Raylib.DrawText($"Health: {player.Health}", 10, 40, 20, Color.RED); // Display Health
            Raylib.DrawText($"Wave {waveNumber}", 10, 70, 30, Color.LIME); // Display Current Wave

            // Display game timer
            Raylib.DrawText($"Time: {Math.Ceiling(gameTimer.TimeRemaining)}", 1150, 10, 20, Color.RED);
        }
    }
}
