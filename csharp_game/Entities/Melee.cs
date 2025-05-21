using Raylib_cs;
using System.Numerics;

namespace VampireSurvivorsClone.Entities
{
    public class Melee
    {
        public Vector2 Position;
        public Vector2 Direction;
        public float Range = 50f;
        public float Damage = 20f;
        public float Size = 20f;
        public bool ShowHitEffect = false;
        private float hitEffectTimer = 0f;

        public Melee(Vector2 startPosition, Vector2 direction)
        {
            Position = startPosition;
            Direction = Vector2.Normalize(direction);
        }

        public void Update(float deltaTime)
        {
            if (ShowHitEffect)
            {
                hitEffectTimer -= deltaTime;
                if (hitEffectTimer <= 0f)
                    ShowHitEffect = false;
            }
        }

        // This method is called when the melee attack hits an enemy
        public void TriggerHitEffect()
        {
            ShowHitEffect = true;
            hitEffectTimer = 0.15f; // Duration of the hit effect
        }

        public void Draw()
        {
            Color color = Damage > 30 ? Color.RED : (Damage > 20 ? Color.ORANGE : Color.BROWN);
            Vector2 hitPos = Position + Direction * Range;
            Raylib.DrawCircleV(hitPos, Size, color);

            // Extra hit effect
            if (ShowHitEffect)
            {
                Raylib.DrawCircleLines((int)hitPos.X, (int)hitPos.Y, Size + 8, Color.WHITE);
            }
        }
    }
}