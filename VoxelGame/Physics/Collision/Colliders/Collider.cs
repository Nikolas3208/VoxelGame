using SFML.System;

namespace VoxelGame.Physics.Collision.Colliders
{
    /// <summary>
    /// Тип коллайдера
    /// </summary>
    public enum ColliderType
    {
        Circle,
        Poligon
    }

    public abstract class Collider
    {
        /// <summary>
        /// Тип коллайдера
        /// </summary>
        public ColliderType Type { get; set; }

        /// <summary>
        /// Ссылка на тело
        /// </summary>
        public RigidBody? Body { get; set; }

        /// <summary>
        /// Коллайдер
        /// </summary>
        public Collider(ColliderType type)
        {
            Type = type;
        }

        /// <summary>
        /// Обновление трансформации коллайдера
        /// </summary>
        /// <param name="position"> Новая позиция </param>
        /// <returns></returns>
        public abstract Collider Transform(Vector2f position);

        /// <summary>
        /// Обновление трансформации коллайдера с учетом вращения
        /// </summary>
        /// <param name="sin"> Новый угол, синус </param>
        /// <param name="cos"> Новый угол, косинус </param>
        /// <param name="position"> Новая позиция </param>
        /// <returns></returns>
        public abstract Collider Transform(float sin, float cos, Vector2f position);
    }
}
