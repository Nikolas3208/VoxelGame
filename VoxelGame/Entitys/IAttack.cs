using SFML.System;

namespace VoxelGame.Entitys
{
    public enum AttackType
    {
        CloseСombat,
        RangeCombat
    }

    public interface IAttack
    {
        /// <summary>
        /// Урон
        /// </summary>
        float Damage { get; set; }

        float RepulsiveForce { get; set; }

        /// <summary>
        /// Атака
        /// </summary>
        /// <param name="damageable"> Обект атаки </param>
        /// <param name="normalAttack"> Направление атаки </param>
        void Attack(IDamageable damageable, Vector2f normalAttack);
    }
}
