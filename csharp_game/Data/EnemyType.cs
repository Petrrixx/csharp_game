namespace VampireSurvivorsClone.Data
{
    // Enum for different enemy types
    public enum EnemyType
    {
        Common,
        Rare,
        VeryRare,
        Legendary,
        Boss
    }

    // Class to hold enemy data
    public class EnemyData
    {
        public EnemyType Type { get; set; }
        public int Health { get; set; }
        public int Speed { get; set; }
        public int Damage { get; set; }
        public int XP { get; set; }

        // Constructor for enemy data
        public EnemyData(EnemyType type, int health, int speed, int damage, int xp)
        {
            Type = type;
            Health = health;
            Speed = speed;
            Damage = damage;
            XP = xp;
        }

        // Static method to get the default enemy data based on enemy type
        public static EnemyData GetEnemyData(EnemyType type, int waveNumber)
        {
            // Uprav základné hodnoty pre prvé vlny, potom rýchlejšie škáluj
            int baseHealth = 20 + waveNumber * 5; // Začni na 20 HP, rýchlejšie rastie
            int baseDamage = 2 + waveNumber / 2;  // Začni na 2 dmg, rastie pomalšie
            int baseSpeed = 100 + waveNumber * 2; // Začni pomalšie

            switch (type)
            {
                case EnemyType.Common:
                    return new EnemyData(EnemyType.Common, baseHealth, baseSpeed, baseDamage, 10);

                case EnemyType.Rare:
                    return new EnemyData(EnemyType.Rare, baseHealth * 2, baseSpeed - 10, baseDamage + 2, 20);

                case EnemyType.VeryRare:
                    return new EnemyData(EnemyType.VeryRare, baseHealth * 4, baseSpeed - 20, baseDamage + 5, 50);

                case EnemyType.Legendary:
                    return new EnemyData(EnemyType.Legendary, baseHealth * 8, baseSpeed - 40, baseDamage + 10, 100);

                case EnemyType.Boss:
                    return new EnemyData(EnemyType.Boss, baseHealth * 20, baseSpeed - 70, baseDamage + 20, 500);

                default:
                    return new EnemyData(EnemyType.Common, baseHealth, baseSpeed, baseDamage, 10);
            }
        }
    }
}
