using Raylib_cs;
using System.Numerics;
using VampireSurvivorsClone.Data;
using VampireSurvivorsClone.Engine;

namespace VampireSurvivorsClone.Entities;

public class Player
{
    public Vector2 Position { get; set; }
    private float baseSpeed = 200f;
    public float Speed
    {
        get => baseSpeed + (Agility - 1) * 20f; // Speed increases with Agility
        set => baseSpeed = value;
    }
    private static int size = 32;
    public static int SizeValue { get => size; }
    private float baseFireCooldown = 0.5f; // Cooldown time in seconds
    private float fireTimer = 0f; // Timer for firing projectiles
    private int health = 50; // Player's health
    private int strength = 10;
    private int agility = 1;
    private int dexterity = 1;
    private int level = 1; // Player's level
    public int Level { get => level; set => level = value; }  // Property for level
    public int Strength { get => strength; set => strength = value; }  // Property for strength
    public int Damage => 10 + Strength * 2; // Damage calculation based on strength
    public int Agility { get => agility; set => agility = value; }  // Property for agility
    public int Dexterity { get => dexterity; set => dexterity = value; }  // Property for dexterity
    public int Health { get => health; set => health = value; }  // Property for health
    private List<Projectile> projectiles = new();
    private List<Melee> meleeAttacks = new();
    public List<Melee> MeleeAttacks => meleeAttacks;
    private int xp = 0; // Player's experience points
    public int XP { get => xp; set => xp = value; }  // Property for XP
    //public int GoldPoints { get; set; } = 0; // Points to spend on upgrades - FUTURE UPDATE
    private List<WeaponInventoryItem> weaponInventory = new();
    public IReadOnlyList<WeaponInventoryItem> WeaponInventory => weaponInventory;
    public int MaxWeapons { get; private set; } = 8;
    private float baseMeleeCooldown = 1f;
    private float meleeTimer = 0f;
    public float FireCooldown => Math.Max(0.1f, baseFireCooldown - (Dexterity - 1) * 0.05f);
    public float MeleeCooldown => Math.Max(0.2f, baseMeleeCooldown - (Dexterity - 1) * 0.07f);
    public int Luck { get; set; } = 0;
    private float statBoostTimer = 0f;
    private bool isStatBoosted = false;

    public Player(DifficultyPreset preset)
    {
        // Center the player
        Position = new Vector2(1280 / 2, 720 / 2);

        // Set player attributes based on difficulty preset
        Health = (int)preset.PlayerHealth;
        Strength = (int)preset.PlayerStrength;
        Agility = (int)preset.PlayerAgility;
        Dexterity = (int)preset.PlayerDexterity;

        // Initialize the player with default values
        weaponInventory.Add(new WeaponInventoryItem { Name = "Normal Projectile", Level = 1 });

    }

    public List<Projectile> Projectiles => projectiles;

    public void TakeDamage(int amount)
    {
        Health -= amount;
        if (Health < 0) Health = 0;  // Prevent health from going below 0
    }

    public Vector2 FacingDirection { get; private set; } = new Vector2(0, -1);  // Default: facing upwards

    public bool IsDead { get; private set; } = false;

    public void Update()
    {
        Vector2 direction = Input.GetMovementDirection();

        if (direction != Vector2.Zero)
        {
            direction = Vector2.Normalize(direction);
            Position += direction * Speed * Raylib.GetFrameTime();
            FacingDirection = direction;  // Update facing direction
        }

        // Auto-fire logic
        fireTimer -= Raylib.GetFrameTime();
        if (fireTimer <= 0f)
        {
            FireProjectile();
            fireTimer = FireCooldown;
        }

        // Melee attack logic
        meleeTimer -= Raylib.GetFrameTime();
        if (meleeTimer <= 0f)
        {
            FireMelee();
            meleeTimer = MeleeCooldown;
        }

        foreach (var m in meleeAttacks)
        {
            m.Update(Raylib.GetFrameTime());
        }

        // Remove dead ones
        projectiles.RemoveAll(p => !p.IsAlive);

        // Check for game over if health <= 0
        if (Health <= 0)
        {
            Health = 0;  // Ensure health isn't negative
            IsDead = true;  // Set death flag instead of exiting
        }

        if (isStatBoosted)
        {
            statBoostTimer -= Raylib.GetFrameTime();
            if (statBoostTimer <= 0f)
            {
                Strength -= 5;
                Agility -= 2;
                Dexterity -= 2;
                isStatBoosted = false;
            }
        }
    }

    // Reset player state for new game
    public void Reset(DifficultyPreset preset)
    {
        // Center the player
        Position = new Vector2(1280 / 2, 720 / 2);
        
        // Reset stats based on difficulty
        Health = (int)preset.PlayerHealth;
        Strength = (int)preset.PlayerStrength;
        Agility = (int)preset.PlayerAgility;
        Dexterity = (int)preset.PlayerDexterity;
        Level = 1;
        XP = 0;
        
        // Clear inventory and add default weapon
        weaponInventory.Clear();
        weaponInventory.Add(new WeaponInventoryItem { Name = "Normal Projectile", Level = 1 });
        
        // Reset flags
        IsDead = false;
        isStatBoosted = false;
    }

    private void FireProjectile()
    {
        foreach (var weapon in weaponInventory)
        {
            switch (weapon.Name)
            {
                case "Normal Projectile":
                    FireNormalProjectile(weapon.Level);
                    break;
                case "Homing Projectile":
                    FireHomingProjectile(weapon.Level);
                    break;
                case "Explosive Projectile":
                    FireExplosiveProjectile(weapon.Level);
                    break;
                case "Shuriken":
                    FireShuriken(weapon.Level);
                    break;
            }
        }
    }

    // Normal projectile : Level 1 = 1 proj, Level 2 = 2 proj, Level 3 = 3 proj, Level 4+ = 4 proj. Level 5 = size + dmg
    private void FireNormalProjectile(int level)
    {
        var start = new Vector2(Position.X + size / 2, Position.Y);
        var dir = FacingDirection;

        float projSize = 6f;
        if (level == 5) projSize = 14f;

        void AddProj(Vector2 d)
        {
            var p = new Projectile(start, d, ProjectileType.Normal);
            if (level == 5) p.SizeValue = projSize;
            projectiles.Add(p);
        }

        AddProj(dir);
        if (level >= 2) AddProj(-dir);
        if (level >= 3) AddProj(new Vector2(-dir.Y, dir.X)); // perpendicular
        if (level >= 4) AddProj(new Vector2(dir.Y, -dir.X));
    }

    // Homing projectile : Level 1 = 1 homing, Level 2+ = 2 homing
    private void FireHomingProjectile(int level)
    {
        var start = new Vector2(Position.X + size / 2, Position.Y);
        projectiles.Add(new Projectile(start, FacingDirection, ProjectileType.Homing));
        if (level >= 2)
            projectiles.Add(new Projectile(start, -FacingDirection, ProjectileType.Homing));
    }

    // Explosive projectile : Level 1 = 1 expl, Level 2+ = 2 expl
    private void FireExplosiveProjectile(int level)
    {
        var start = new Vector2(Position.X + size / 2, Position.Y);
        var proj = new Projectile(start, FacingDirection, ProjectileType.Explosive);
        if (level >= 2)
        {
            proj.SizeValue *= 1.5f;
            proj.DamageValue += 10;
        }
        projectiles.Add(proj);
    }

    private void FireShuriken(int level)
    {
        var start = new Vector2(Position.X + size / 2, Position.Y + size / 2);
        Vector2 moveDir = Input.GetMovementDirection();
        if (moveDir == Vector2.Zero) moveDir = -FacingDirection; // default direction
        else moveDir = -Vector2.Normalize(moveDir); // reverse direction

        void AddShuriken(Vector2 d)
        {
            var p = new Projectile(start, d, ProjectileType.Shuriken);
            p.PierceCount = 3; // piercing
            projectiles.Add(p);
        }

        AddShuriken(moveDir);

        if (level >= 3)
        {
            // Multishot: two more - ±20°
            float angle = 20 * (float)(Math.PI / 180);
            Vector2 left = new Vector2(
                moveDir.X * (float)Math.Cos(angle) - moveDir.Y * (float)Math.Sin(angle),
                moveDir.X * (float)Math.Sin(angle) + moveDir.Y * (float)Math.Cos(angle)
            );
            Vector2 right = new Vector2(
                moveDir.X * (float)Math.Cos(-angle) - moveDir.Y * (float)Math.Sin(-angle),
                moveDir.X * (float)Math.Sin(-angle) + moveDir.Y * (float)Math.Cos(-angle)
            );
            AddShuriken(left);
            AddShuriken(right);
        }
    }

    private void FireMelee()
    {
        foreach (var weapon in weaponInventory)
        {
            if (weapon.Name == "Melee Attack")
            {
                // Calculate melee attack properties based on weapon level
                float range = 50f + 10f * (weapon.Level - 1);
                float damage = 20f + 5f * (weapon.Level - 1);
                meleeAttacks.Add(new Melee(Position, FacingDirection) { Range = range, Damage = damage });
            }
        }
    }

    // Add a projectile to the player's list of projectiles - FOR FUTURE REFERENCES
    public void addProjectile(Projectile projectile)
    {
        projectiles.Add(projectile);
    }

    // Add a melee attack to the player's list of melee attacks - FOR FUTURE REFERENCES
    public void addMelee(Melee melee)
    {
        meleeAttacks.Add(melee);
    }

    public void Draw()
    {
        Raylib.DrawRectangle((int)Position.X, (int)Position.Y, size, size, Color.BLUE);

        foreach (var p in projectiles)
        {
            p.Draw();
        }

        // Garlic aura
        int garlicLevel = GetWeaponLevel("Garlic");
        if (garlicLevel > 0)
        {
            int circleCount = garlicLevel >= 5 ? 3 : (garlicLevel >= 3 ? 2 : 1);
            float[] radii = circleCount switch
            {
                1 => new[] { 60f },
                2 => new[] { 60f, 90f },
                3 => new[] { 60f, 90f, 120f },
                _ => new[] { 60f }
            };
            for (int i = 0; i < circleCount; i++)
            {
                Raylib.DrawCircleLines((int)Position.X + size / 2, (int)Position.Y + size / 2, radii[i], Color.LIGHTGRAY);
            }
        }
    }

    public int GetWeaponLevel(string weaponName)
    {
        var weapon = weaponInventory.FirstOrDefault(w => 
            string.Equals(w.Name, weaponName, StringComparison.OrdinalIgnoreCase));
        return weapon?.Level ?? 0;
    }

    public void AddOrUpgradeWeapon(string weaponName)
    {
        var weapon = weaponInventory.FirstOrDefault(w => w.Name == weaponName);
        if (weapon == null && weaponInventory.Count < MaxWeapons)
        {
            weaponInventory.Add(new WeaponInventoryItem { Name = weaponName, Level = 1 });
        }
        else if (weapon != null)
        {
            weapon.Level++;
        }
        else
        {
            // Handle case when inventory is full (e.g., show message or replace an existing weapon)
            // For now, we just ignore the upgrade
            Console.WriteLine("[DEBUG] Inventory full! Cannot add or upgrade weapon.");
        }
    }

    public void IncreaseInventorySlot() => MaxWeapons++;
    
    public void BoostStats(float duration)
    {
        if (!isStatBoosted)
        {
            Strength += 5;
            Agility += 2;
            Dexterity += 2;
            Health += 50;
            isStatBoosted = true;
            statBoostTimer = duration;
        }
    }

    // Add this method to your Player class
    public void ClearWeapons()
    {
        weaponInventory.Clear();
    }
}