namespace VampireSurvivorsClone.Data
{
    // Enum for different enemy types
    public enum EnemyType
    {
        Common,
        Rare,
        VeryRare,
        Legendary,
        Boss,
        DemonKing,
        Mimic
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
        public static EnemyData GetEnemyData(EnemyType type, int waveNumber, DifficultyPreset preset)
        {
            // Base stats for enemies
            int baseHealth = (int)((20 + waveNumber * 5) * preset.EnemyHealthMultiplier);
            int baseDamage = (int)((2 + waveNumber / 2) * preset.EnemyDamageMultiplier);
            int baseSpeed = 100 + waveNumber * 2;
            int baseXP = (int)((waveNumber - 1) * preset.EnemyXPMultiplier);

            switch (type)
            {
                case EnemyType.Common:
                    return new EnemyData(EnemyType.Common, baseHealth, baseSpeed, baseDamage, 1 + baseXP);

                case EnemyType.Rare:
                    return new EnemyData(EnemyType.Rare, baseHealth * 2, baseSpeed - 10, baseDamage + 2, 2 + baseXP);

                case EnemyType.VeryRare:
                    return new EnemyData(EnemyType.VeryRare, baseHealth * 4, baseSpeed - 20, baseDamage + 5, 5 + baseXP);

                case EnemyType.Legendary:
                    return new EnemyData(EnemyType.Legendary, baseHealth * 8, baseSpeed - 40, baseDamage + 10, 10 + baseXP);

                case EnemyType.Boss:
                    return new EnemyData(EnemyType.Boss, baseHealth * 20, baseSpeed - 70, baseDamage + 20, 30 + baseXP);

                case EnemyType.DemonKing:
                    return new EnemyData(EnemyType.DemonKing, baseHealth * 40, baseSpeed - 80, baseDamage + 40, 50 + baseXP);

                case EnemyType.Mimic:
                    return new EnemyData(EnemyType.Mimic, (int)preset.PlayerHealth, 200, (int)preset.PlayerStrength, 25 + baseXP);

                default:
                    return new EnemyData(EnemyType.Common, baseHealth, baseSpeed, baseDamage, 10);
            }
        }
    }
}
