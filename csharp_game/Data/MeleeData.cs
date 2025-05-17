namespace VampireSurvivorsClone.Data
{
    public enum MeleeType
    {
        BasicSlash,
        HeavySmash,
        FastStrike
    }

    public class MeleeData
    {
        public MeleeType Type { get; set; }
        public float Damage { get; set; }
        public float Range { get; set; }
        public float Cooldown { get; set; }
        public string Texture { get; set; }  // This will be used to store texture paths or texture names when textures are added

        // Constructor for melee attacks
        public MeleeData(MeleeType type, float damage, float range, float cooldown, string texture)
        {
            Type = type;
            Damage = damage;
            Range = range;
            Cooldown = cooldown;
            Texture = texture;
        }

        // Return melee data based on melee type
        public static MeleeData GetMeleeData(MeleeType type)
        {
            return type switch
            {
                MeleeType.BasicSlash => new MeleeData(MeleeType.BasicSlash, 15f, 50f, 1f, "BasicSlashTexture"),
                MeleeType.HeavySmash => new MeleeData(MeleeType.HeavySmash, 30f, 60f, 2f, "HeavySmashTexture"),
                MeleeType.FastStrike => new MeleeData(MeleeType.FastStrike, 10f, 40f, 0.5f, "FastStrikeTexture"),
                _ => new MeleeData(MeleeType.BasicSlash, 15f, 50f, 1f, "BasicSlashTexture"),
            };
        }
    }
}
