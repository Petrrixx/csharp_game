namespace VampireSurvivorsClone.Data
{
    public enum ProjectileType
    {
        Normal,
        Homing,
        Explosive,
        Piercing,
        Shuriken
    }

    public class ProjectileData
    {
        public ProjectileType Type { get; set; }
        public float Speed { get; set; }
        public float Size { get; set; }
        public float Lifetime { get; set; }
        public int Damage { get; set; }

        // Constructor for basic projectiles
        public ProjectileData(ProjectileType type, float speed, float size, float lifetime, int damage)
        {
            Type = type;
            Speed = speed;
            Size = size;
            Lifetime = lifetime;
            Damage = damage;
        }

        // Add additional logic for different types here
        public static ProjectileData GetProjectileData(ProjectileType type)
        {
            return type switch
            {
                ProjectileType.Normal => new ProjectileData(ProjectileType.Normal, 400f, 6f, 2f, 10),
                ProjectileType.Homing => new ProjectileData(ProjectileType.Homing, 300f, 8f, 5f, 10),
                ProjectileType.Explosive => new ProjectileData(ProjectileType.Explosive, 350f, 10f, 1.5f, 30),
                ProjectileType.Piercing => new ProjectileData(ProjectileType.Piercing, 450f, 6f, 2.5f, 15),
                ProjectileType.Shuriken => new ProjectileData(ProjectileType.Shuriken, 500f, 10f, 2f, 12),
                _ => new ProjectileData(ProjectileType.Normal, 400f, 6f, 2f, 10),
            };
        }
    }
}
