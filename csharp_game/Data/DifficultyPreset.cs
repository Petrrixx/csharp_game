namespace VampireSurvivorsClone.Data;

public enum Difficulty
{
    Easy,
    Normal,
    Hard
}

public class DifficultyPreset
{
    public float PlayerHealth;
    public float PlayerStrength;
    public float PlayerAgility;
    public float PlayerDexterity;
    public float EnemyHealthMultiplier;
    public float EnemyDamageMultiplier;
    public float EnemyXPMultiplier;

    public static DifficultyPreset Get(Difficulty diff)
    {
        return diff switch
        {
            Difficulty.Easy => new DifficultyPreset
            {
                PlayerHealth = 100,
                PlayerStrength = 15,
                PlayerAgility = 2,
                PlayerDexterity = 2,
                EnemyHealthMultiplier = 0.8f,
                EnemyDamageMultiplier = 0.8f,
                EnemyXPMultiplier = 0.5f
            },
            Difficulty.Normal => new DifficultyPreset
            {
                PlayerHealth = 70,
                PlayerStrength = 10,
                PlayerAgility = 1,
                PlayerDexterity = 1,
                EnemyHealthMultiplier = 1f,
                EnemyDamageMultiplier = 1f,
                EnemyXPMultiplier = 1f,
            },
            Difficulty.Hard => new DifficultyPreset
            {
                PlayerHealth = 50,
                PlayerStrength = 8,
                PlayerAgility = 1,
                PlayerDexterity = 1,
                EnemyHealthMultiplier = 1.3f,
                EnemyDamageMultiplier = 1.3f,
                EnemyXPMultiplier = 1.5f
            },
            _ => new DifficultyPreset()
        };
    }
}