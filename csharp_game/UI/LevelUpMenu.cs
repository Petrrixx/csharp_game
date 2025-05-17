using Raylib_cs;

namespace VampireSurvivorsClone.Entities
{
    public class LevelUpMenu
    {
        private int xpRequired;
        private int currentLevel;
        private int playerXP;
        private int abilityPoints;
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
            this.Vitality = player.Health; // Assuming Player class has Vitality property
            this.Dexterity = player.Dexterity; // Assuming Player class has Dexterity property
            this.abilityPoints = player.AbilityPoints; // Assuming Player class has AbilityPoints property

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
            Raylib.DrawText($"Ability Points: {abilityPoints}", 520, 180, 20, Color.YELLOW);

            // Draw available upgrades
            DrawUpgrades();

            // Check if the player presses Enter to move to the Shop Menu
            if (Raylib.IsKeyPressed(KeyboardKey.KEY_ENTER) && playerXP >= xpRequired)
            {
                player.Level++;  // Increase the player level
                playerXP -= xpRequired;  // Deduct the XP required for leveling up
                abilityPoints++; // Give the player 1 ability point per level up

                // Move to the shop menu after upgrading stats
                isLevelUpMenuActive = false;
                isShopMenuActive = true;
            }

            Raylib.EndDrawing();
        }

        // Draw available upgrade options
        private void DrawUpgrades()
        {
            // Display current stats
            Raylib.DrawText($"1. Strength (Damage): {Strength}", 520, 200, 20, Color.WHITE);
            Raylib.DrawText($"2. Agility (Speed): {Agility}", 520, 230, 20, Color.WHITE);
            Raylib.DrawText($"3. Vitality (Health): {Vitality}", 520, 260, 20, Color.WHITE);
            Raylib.DrawText($"4. Dexterity (Cooldown): {Dexterity}", 520, 290, 20, Color.WHITE);

            // Handle upgrades if the player has enough ability points
            if (Raylib.IsKeyPressed(KeyboardKey.KEY_ONE) && abilityPoints > 0)
            {
                Strength++;
                abilityPoints--;
            }

            if (Raylib.IsKeyPressed(KeyboardKey.KEY_TWO) && abilityPoints > 0)
            {
                Agility++;
                abilityPoints--;
            }

            if (Raylib.IsKeyPressed(KeyboardKey.KEY_THREE) && abilityPoints > 0)
            {
                Vitality++;
                abilityPoints--;
            }

            if (Raylib.IsKeyPressed(KeyboardKey.KEY_FOUR) && abilityPoints > 0)
            {
                Dexterity++;
                abilityPoints--;
            }
        }

        // Show the shop menu
        private void ShowShopMenu()
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.BLACK);

            Raylib.DrawText("Shop Menu", 520, 100, 30, Color.LIME);

            // Randomly generate 3 items for the player to choose from
            string[] availableItems = GetRandomShopItems();
            Raylib.DrawText($"1. {availableItems[0]} - 50 XP", 520, 150, 20, Color.WHITE);
            Raylib.DrawText($"2. {availableItems[1]} - 60 XP", 520, 180, 20, Color.WHITE);
            Raylib.DrawText($"3. {availableItems[2]} - 40 XP", 520, 210, 20, Color.WHITE);

            // Purchase items from the shop
            if (Raylib.IsKeyPressed(KeyboardKey.KEY_ONE) && playerXP >= 50)
            {
                // Add the selected item (Homing Projectile, Explosive Projectile, or Melee Attack) to the player
                playerXP -= 50;
                // Add specific ability (based on the randomly generated item)
                AddAbilityToPlayer(availableItems[0]);
                isShopMenuActive = false;
            }

            if (Raylib.IsKeyPressed(KeyboardKey.KEY_TWO) && playerXP >= 60)
            {
                playerXP -= 60;
                AddAbilityToPlayer(availableItems[1]);
                isShopMenuActive = false;
            }

            if (Raylib.IsKeyPressed(KeyboardKey.KEY_THREE) && playerXP >= 40)
            {
                playerXP -= 40;
                AddAbilityToPlayer(availableItems[2]);
                isShopMenuActive = false;
            }

            // Wait for any key to close the shop and continue to the next wave
            if (Raylib.IsKeyPressed(KeyboardKey.KEY_ENTER))
            {
                isShopMenuActive = false;  // Close shop menu
            }

            Raylib.EndDrawing();
        }

        // Randomly select 3 shop items for the player to choose from
        private string[] GetRandomShopItems()
        {
            // Example shop items: Melee or Projectile types
            string[] items = new string[] { "Normal Projectile", "Homing Projectile", "Explosive Projectile", "Melee Attack" };
            Random rand = new Random();
            return items.OrderBy(x => rand.Next()).Take(3).ToArray(); // Shuffle and pick 3 random items
        }

        // Add the selected ability to the player's arsenal
        private void AddAbilityToPlayer(string ability)
        {
            if (ability == "Normal Projectile")
            {
                player.addProjectile(new Projectile(player.Position, player.FacingDirection, Data.ProjectileType.Normal));  // Assuming addNormalProjectile is a method in the Player class
            }
            // Based on the ability chosen, add it to the player's list of abilities (this should be implemented in Player.cs)
            if (ability == "Homing Projectile")
            {
                player.addProjectile(new Projectile(player.Position, player.FacingDirection, Data.ProjectileType.Homing));  // Assuming AddHomingProjectile is a method in the Player class
            }
            else if (ability == "Explosive Projectile")
            {
                player.addProjectile(new Projectile(player.Position, player.FacingDirection, Data.ProjectileType.Explosive));  // Assuming AddExplosiveProjectile is a method in the Player class
            }
            else if (ability == "Melee Attack")
            {
                player.addMelee(new Melee(player.Position, player.FacingDirection));  
            }
        }
    }
}
