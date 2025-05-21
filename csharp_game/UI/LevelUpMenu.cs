using Raylib_cs;
using System;
using System.Linq;
using System.Collections.Generic;
using VampireSurvivorsClone.Entities;
using VampireSurvivorsClone.Data;

namespace VampireSurvivorsClone.UI
{
    public class LevelUpMenu
    {
        private Player player;
        private bool isActive = false;
        private List<UpgradeOption> options = new();
        private int selected = 0;

        private static readonly string[] StatNames = { "Strength", "Agility", "Vitality", "Dexterity" };
        private static readonly string[] WeaponNames = { "Normal Projectile", "Homing Projectile", "Explosive Projectile", "Melee Attack", "Garlic", "Shuriken" };

        public LevelUpMenu(Player player)
        {
            this.player = player;
        }

        public void Open()
        {
            isActive = true;
            GenerateOptions();
            selected = 0;
        }

        public bool IsActive => isActive;

        public void Update()
        {
            if (!isActive) return;

            // Navigation
            if (Raylib.IsKeyPressed(KeyboardKey.KEY_RIGHT)) selected = (selected + 1) % 3;
            if (Raylib.IsKeyPressed(KeyboardKey.KEY_LEFT)) selected = (selected + 2) % 3;

            // Confirm selection
            if (Raylib.IsKeyPressed(KeyboardKey.KEY_ENTER))
            {
                ApplyOption(options[selected]);
                isActive = false;
            }
        }

        public void Draw()
        {
            if (!isActive) return;

            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.DARKGRAY);
            Raylib.DrawText("LEVEL UP!", 540, 80, 40, Color.LIME);

            for (int i = 0; i < options.Count; i++)
            {
                var opt = options[i];
                int y = 220 + i * 100;
                Color col = (i == selected) ? Color.YELLOW : Color.WHITE;
                Raylib.DrawRectangle(400 + i * 200, y - 20, 180, 80, (i == selected) ? Color.DARKGREEN : Color.DARKGRAY);
                Raylib.DrawText(opt.Description, 410 + i * 200, y, 20, col);
            }

            Raylib.EndDrawing();
        }

        private void GenerateOptions()
        {
            options.Clear();
            var rand = new Random();

            var pool = new List<UpgradeOption>
            {
                new UpgradeOption { Type = UpgradeType.Stat, Stat = "Strength", Description = "Strength +1 (Damage)" },
                new UpgradeOption { Type = UpgradeType.Stat, Stat = "Agility", Description = "Agility +1 (Speed)" },
                new UpgradeOption { Type = UpgradeType.Stat, Stat = "Vitality", Description = "Vitality +10 (Health)" },
                new UpgradeOption { Type = UpgradeType.Stat, Stat = "Dexterity", Description = "Dexterity +1 (Cooldown)" },
                new UpgradeOption { Type = UpgradeType.Stat, Stat = "Luck", Description = "Luck +1 (More XP from gems)" },
                new UpgradeOption { Type = UpgradeType.Weapon, Weapon = "Garlic", Description = WeaponOptionDesc("Garlic") },
                new UpgradeOption { Type = UpgradeType.Weapon, Weapon = "Normal Projectile", Description = WeaponOptionDesc("Normal Projectile") },
                new UpgradeOption { Type = UpgradeType.Weapon, Weapon = "Homing Projectile", Description = WeaponOptionDesc("Homing Projectile") },
                new UpgradeOption { Type = UpgradeType.Weapon, Weapon = "Explosive Projectile", Description = WeaponOptionDesc("Explosive Projectile") },
                new UpgradeOption { Type = UpgradeType.Weapon, Weapon = "Melee Attack", Description = WeaponOptionDesc("Melee Attack") },
                new UpgradeOption { Type = UpgradeType.Weapon, Weapon = "Shuriken", Description = WeaponOptionDesc("Shuriken") },
            };

             // Inventory Slot +1 chance
            if (rand.NextDouble() < 0.05)
            {
                pool.Add(new UpgradeOption { Type = UpgradeType.Stat, Stat = "Inventory", Description = "Inventory Slot +1 (Very Rare)" });
            }

            // Filter out already maxed weapons (stat upgrades are not filtered)
            pool.RemoveAll(x =>
                x.Type == UpgradeType.Weapon && player.GetWeaponLevel(x.Weapon) >= 5
            );

            // Shuffle and pick 3
            options = pool.OrderBy(x => rand.Next()).Take(3).ToList();
        }

        private string WeaponOptionDesc(string weapon)
        {
            int level = player.GetWeaponLevel(weapon);
            if (level == 0)
                return $"{weapon} (NEW)";
            else if (level >= 5)
                return $"{weapon} (MAX)";
            else
                return $"{weapon} Lv.{level + 1} (Upgrade)";
        }

        private void ApplyOption(UpgradeOption option)
        {
            if (option.Type == UpgradeType.Stat)
            {
                switch (option.Stat)
                {
                    case "Strength": player.Strength++; break;
                    case "Agility": player.Agility++; break;
                    case "Vitality": player.Health += 10; break;
                    case "Dexterity": player.Dexterity++; break;
                    case "Luck": player.Luck++; break;
                    case "Inventory": player.IncreaseInventorySlot(); break;
                }
            }
            else if (option.Type == UpgradeType.Weapon)
            {
                player.AddOrUpgradeWeapon(option.Weapon);
            }
            player.Level++;
            player.XP = 0; // Reset XP after level up
        }

        // Helper class for upgrade options
        private class UpgradeOption
        {
            public UpgradeType Type;
            public string Stat;
            public string Weapon;
            public string Description;
        }
        private enum UpgradeType { Stat, Weapon }
    }
}