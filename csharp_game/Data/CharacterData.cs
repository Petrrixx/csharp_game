namespace VampireSurvivorsClone.Data
{
    public class CharacterData
    {
        public float Health { get; set; }
        public float Speed { get; set; }
        public float Strength { get; set; }
        public float Dexterity { get; set; }
        public float Agility { get; set; }
        public string Texture { get; set; }  // For storing the character's texture

        // Constructor for initializing character stats
        public CharacterData(float health, float speed, float strength, float dexterity, float agility, string texture)
        {
            Health = health;
            Speed = speed;
            Strength = strength;
            Dexterity = dexterity;
            Agility = agility;
            Texture = texture;
        }

        // Update character stats, useful when leveling up or changing stats
        public void UpdateStats(float healthIncrease, float speedIncrease, float strengthIncrease, float dexterityIncrease, float agilityIncrease)
        {
            Health += healthIncrease;
            Speed += speedIncrease;
            Strength += strengthIncrease;
            Dexterity += dexterityIncrease;
            Agility += agilityIncrease;
        }

        // Static method to return default character data, or you can modify it based on level or other conditions
        public static CharacterData GetDefaultCharacterData()
        {
            return new CharacterData(100f, 5f, 10f, 8f, 6f, "PlayerTexture");
        }
    }
}
