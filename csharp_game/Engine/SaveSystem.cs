using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using VampireSurvivorsClone.Data;
using VampireSurvivorsClone.Entities;
using Raylib_cs;
using System.Numerics;

namespace VampireSurvivorsClone.Engine
{
    public class SaveSystem
    {
        private static readonly string SAVE_DIRECTORY = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "VampireSurvivorsClone",
            "saves"
        );
        private const string SAVE_EXTENSION = ".json";
        
        public SaveSystem()
        {
            // Create save directory if it doesn't exist
            if (!Directory.Exists(SAVE_DIRECTORY))
            {
                Directory.CreateDirectory(SAVE_DIRECTORY);
            }
        }
        
        public void SaveGame(Game game)
        {
            var saveData = new SaveGameData
            {
                SaveName = $"Save_{DateTime.Now:yyyyMMdd_HHmmss}",
                SaveDate = DateTime.Now,
                WaveNumber = game.CurrentWave,
                TimeRemaining = game.GameTimer.TimeRemaining,
                PlayerXP = game.PlayerXP,
                PlayerLevel = game.PlayerLevel, // Backwards compatibility
                Difficulty = game.Preset != null ? game.Preset.GetType().Name.Replace("Difficulty", "") : "Normal",
                Player = CreatePlayerSaveData(game.PlayerInstance)
            };
            
            // Generate the save file name
            string fileName = Path.Combine(SAVE_DIRECTORY, saveData.SaveName + SAVE_EXTENSION);
            
            // Serialize and save
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(saveData, options);
            File.WriteAllText(fileName, jsonString);
        }
        
        public List<SaveGameData> GetSaveFiles()
        {
            List<SaveGameData> saves = new();
            
            if (!Directory.Exists(SAVE_DIRECTORY))
                return saves;
            
            foreach (string filePath in Directory.GetFiles(SAVE_DIRECTORY, $"*{SAVE_EXTENSION}"))
            {
                try
                {
                    string json = File.ReadAllText(filePath);
                    var saveData = JsonSerializer.Deserialize<SaveGameData>(json);
                    if (saveData != null)
                        saves.Add(saveData);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading save file {filePath}: {ex.Message}");
                }
            }
            
            // Sort by date, newest first
            saves.Sort((a, b) => b.SaveDate.CompareTo(a.SaveDate));
            
            return saves;
        }
        
        public void LoadGame(Game game, SaveGameData saveData)
        {
            // Load game state
            game.SetWaveNumber(saveData.WaveNumber);
            game.GameTimer.Reset(saveData.TimeRemaining);
            game.PlayerXP = saveData.PlayerXP;
            
            // Load player state
            var player = game.PlayerInstance;
            player.Position = new Vector2(saveData.Player.PositionX, saveData.Player.PositionY);
            player.Health = saveData.Player.Health;
            player.Strength = saveData.Player.Strength;
            player.Agility = saveData.Player.Agility;
            player.Dexterity = saveData.Player.Dexterity;
            player.Level = saveData.Player.PlayerLevel > 0 ? saveData.Player.PlayerLevel : saveData.PlayerLevel;
            player.Luck = saveData.Player.Luck;
            
            // Clear existing weapons
            game.ClearPlayerWeapons();
            
            // Add saved weapons
            foreach (var weapon in saveData.Player.Weapons)
            {
                for (int i = 0; i < weapon.Level; i++)
                {
                    player.AddOrUpgradeWeapon(weapon.Name);
                }
            }
        }
        
        private SaveGameData.PlayerSaveData CreatePlayerSaveData(Player player)
        {
            var playerData = new SaveGameData.PlayerSaveData
            {
                Position = player.Position,
                Health = player.Health,
                Strength = player.Strength,
                Agility = player.Agility,
                Dexterity = player.Dexterity,
                PlayerLevel = player.Level,
                Luck = player.Luck,
                MaxWeapons = player.MaxWeapons,
                Weapons = new List<SaveGameData.WeaponSaveData>()
            };
            
            // Save weapons
            foreach (var weapon in player.WeaponInventory)
            {
                playerData.Weapons.Add(new SaveGameData.WeaponSaveData
                {
                    Name = weapon.Name,
                    Level = weapon.Level
                });
            }
            
            return playerData;
        }

        public void DeleteSave(string saveName)
        {
            string filePath = Path.Combine(SAVE_DIRECTORY, saveName + SAVE_EXTENSION);
            if (File.Exists(filePath))
            {
                try
                {
                    File.Delete(filePath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error deleting save file {filePath}: {ex.Message}");
                }
            }
        }
    }
}