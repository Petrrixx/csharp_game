using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text.Json.Serialization;

namespace VampireSurvivorsClone.Data
{
    public class SaveGameData
    {
        // Save metadata
        public string SaveName { get; set; } = string.Empty;
        public DateTime SaveDate { get; set; } = DateTime.Now;
        public string SaveVersion { get; set; } = "1.0";
        
        // Game state
        public int WaveNumber { get; set; } = 1;
        public float TimeRemaining { get; set; } = 30f;
        public int PlayerXP { get; set; } = 0;
        public int PlayerLevel { get; set; } = 1;
        
        public PlayerSaveData Player { get; set; } = new PlayerSaveData();
        
        // Difficulty
        public string Difficulty { get; set; } = "Normal";
        
        public class PlayerSaveData
        {
            public float PositionX { get; set; }
            public float PositionY { get; set; }
            
            [JsonIgnore]
            public Vector2 Position 
            { 
                get => new Vector2(PositionX, PositionY);
                set { PositionX = value.X; PositionY = value.Y; }
            }
            
            public int Health { get; set; }
            public int Strength { get; set; }
            public int Agility { get; set; }
            public int Dexterity { get; set; }
            public int Luck { get; set; }
            public int PlayerLevel { get; set; } = 1;
            public List<WeaponSaveData> Weapons { get; set; } = new List<WeaponSaveData>();
            public int MaxWeapons { get; set; } = 8;
        }
        
        public class WeaponSaveData
        {
            public string Name { get; set; } = string.Empty;
            public int Level { get; set; } = 1;
        }
    }
}