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

            // Display game timer (moved lower)
            Raylib.DrawText($"Time: {Math.Ceiling(gameTimer.TimeRemaining)}", 1150, barY + barHeight + 70, 20, Color.RED);

            // --- Inventory ---
            int invY = barY + barHeight + 110;
            Raylib.DrawText($"Inventory (Slots: {player.WeaponInventory.Count}/{player.MaxWeapons})", 10, invY, 20, Color.LIGHTGRAY);
            int i = 0;
            foreach (var weapon in player.WeaponInventory)
            {
                Raylib.DrawText($"{weapon.Name} (Lv.{weapon.Level})", 30, invY + 25 + i * 22, 18, Color.WHITE);
                i++;
            }

            // --- Player stats ---
            int statsX = 300;
            int statsY = invY;
            Raylib.DrawText("Player Stats:", statsX, statsY, 20, Color.LIGHTGRAY);
            Raylib.DrawText($"Strength: {player.Strength}", statsX + 20, statsY + 25, 18, Color.WHITE);
            Raylib.DrawText($"Agility: {player.Agility}", statsX + 20, statsY + 47, 18, Color.WHITE);
            Raylib.DrawText($"Dexterity: {player.Dexterity}", statsX + 20, statsY + 69, 18, Color.WHITE);
            Raylib.DrawText($"Luck: {player.Luck}", statsX + 20, statsY + 91, 18, Color.WHITE);
        }
    }
}
