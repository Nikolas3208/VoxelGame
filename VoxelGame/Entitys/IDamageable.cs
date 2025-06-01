using SFML.System;

namespace VoxelGame.Entitys
{
    public interface IDamageable
    {
        float Health { get; set; }
        float MaxHealth { get; set; }

        float Armor { get; set; }

        void Damege(float damage, Vector2f normalAttack);

        void PushAway(Vector2f dir);
    }
}
