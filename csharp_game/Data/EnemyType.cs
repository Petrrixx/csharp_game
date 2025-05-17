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
            // Modify stats based on the wave number for more challenging enemies
            switch (type)
            {
                case EnemyType.Common:
                    return new EnemyData(EnemyType.Common, 50 + waveNumber * 5, 120, 5, 10);

                case EnemyType.Rare:
                    return new EnemyData(EnemyType.Rare, 100 + waveNumber * 10, 100, 10, 20);

                case EnemyType.VeryRare:
                    return new EnemyData(EnemyType.VeryRare, 200 + waveNumber * 20, 80, 15, 50);

                case EnemyType.Legendary:
                    return new EnemyData(EnemyType.Legendary, 400 + waveNumber * 40, 60, 20, 100);

                case EnemyType.Boss:
                    return new EnemyData(EnemyType.Boss, 1000 + waveNumber * 100, 30, 40, 500);

                default:
                    return new EnemyData(EnemyType.Common, 50, 120, 5, 10);
            }
        }
    }
}
