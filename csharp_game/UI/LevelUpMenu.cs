using Raylib_cs;
using System;

namespace VampireSurvivorsClone.Entities
{
    public class LevelUpMenu
    {
        private int xpRequired;
        private int currentLevel;
        private int playerXP;
        private bool isLevelUpMenuActive = true; // When created, the level-up menu is active
        private bool isShopMenuActive = false; // Controls whether to show ShopMenu
        private Player player; // Reference to the player object

        // Stats the player can upgrade
        public int Strength { get; set; } // Damage
        public int Agility { get; set; }  // Movement Speed
        public int Vitality { get; set; } // Health Points
        public int Dexterity { get; set; } // Cooldown Reduction

        public LevelUpMenu(Player player)
        {
            this.player = player; // Store the reference to the player
        }

        // Update function to handle input and show the appropriate menu
        public void Update()
        {
            this.currentLevel = player.Level; // Assuming Player class has Level property
            xpRequired = currentLevel * 10; // Example: level 1 = 10 XP, level 2 = 20 XP, etc.

            this.playerXP = player.XP; // Assuming Player class has an XP property
            this.Strength = player.Strength; // Assuming Player class has Strength property
            this.Agility = player.Agility; // Assuming Player class has Agility property
            this.Vitality = player.Vitality; // Assuming Player class has Vitality property
            this.Dexterity = player.Dexterity; // Assuming Player class has Dexterity property

            if (isLevelUpMenuActive)
            {
                ShowLevelUpMenu();
            }
            else if (isShopMenuActive)
            {
                ShowShopMenu();
            }
        }

        // Show the level-up menu
        private void ShowLevelUpMenu()
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.BLACK);

            Raylib.DrawText($"Level {currentLevel} Upgrade", 520, 100, 30, Color.LIME);
            Raylib.DrawText($"XP: {playerXP}/{xpRequired}", 520, 150, 20, Color.YELLOW);

            // Draw available upgrades
            DrawUpgrades();

            // Check if the player presses Enter to move to the Shop Menu
            if (Raylib.IsKeyPressed(KeyboardKey.KEY_ENTER) && playerXP >= xpRequired)
            {
                isLevelUpMenuActive = false;
                isShopMenuActive = true;
            }

            Raylib.EndDrawing();
        }

        // Draw available upgrade options
        private void DrawUpgrades()
        {
            Raylib.DrawText($"1. Strength (Damage): {Strength}", 520, 200, 20, Color.WHITE);
            Raylib.DrawText($"2. Agility (Speed): {Agility}", 520, 230, 20, Color.WHITE);
            Raylib.DrawText($"3. Vitality (Health): {Vitality}", 520, 260, 20, Color.WHITE);
            Raylib.DrawText($"4. Dexterity (Cooldown): {Dexterity}", 520, 290, 20, Color.WHITE);

            // Example of checking input for upgrades (use actual input handling logic)
            if (Raylib.IsKeyPressed(KeyboardKey.KEY_ONE) && playerXP >= 10)
            {
                Strength++;
                playerXP -= 10; // Deduct XP for the upgrade
            }

            if (Raylib.IsKeyPressed(KeyboardKey.KEY_TWO) && playerXP >= 10)
            {
                Agility++;
                playerXP -= 10;
            }

            if (Raylib.IsKeyPressed(KeyboardKey.KEY_THREE) && playerXP >= 10)
            {
                Vitality++;
                playerXP -= 10;
            }
            
            if (Raylib.IsKeyPressed(KeyboardKey.KEY_FOUR) && playerXP >= 10)
            {
                Dexterity++;
                playerXP -= 10;
            }
        }

        // Show the shop menu
        private void ShowShopMenu()
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.BLACK);

            Raylib.DrawText("Shop Menu", 520, 100, 30, Color.LIME);

            // Draw available items to purchase
            Raylib.DrawText("1. Homing Projectile - 50 XP", 520, 150, 20, Color.WHITE);
            Raylib.DrawText("2. Explosive Projectile - 60 XP", 520, 180, 20, Color.WHITE);
            Raylib.DrawText("3. Melee Attack - 40 XP", 520, 210, 20, Color.WHITE);

            // Purchase items from the shop
            if (Raylib.IsKeyPressed(KeyboardKey.KEY_ONE) && playerXP >= 50)
            {
                // Add homing projectile ability to player
                // Assuming Player class has an AddAbility method (you can implement it)
                playerXP -= 50;
                isShopMenuActive = false;
            }

            if (Raylib.IsKeyPressed(KeyboardKey.KEY_TWO) && playerXP >= 60)
            {
                // Add explosive projectile ability to player
                playerXP -= 60;
                isShopMenuActive = false;
            }

            if (Raylib.IsKeyPressed(KeyboardKey.KEY_THREE) && playerXP >= 40)
            {
                // Add melee attack ability to player
                playerXP -= 40;
                isShopMenuActive = false;
            }

            // Wait for any key to close the shop and continue to next wave
            if (Raylib.IsKeyPressed(KeyboardKey.KEY_ENTER))
            {
                isShopMenuActive = false;  // Close shop menu
            }

            Raylib.EndDrawing();
        }
    }
}
