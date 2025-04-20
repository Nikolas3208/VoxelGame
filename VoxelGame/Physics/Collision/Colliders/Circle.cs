using SFML.System;

namespace VoxelGame.Physics.Collision.Colliders;

public class Circle : Collider
{
    /// <summary>
    /// Радиус окружности
    /// </summary>
    public float Radius;

    /// <summary>
    /// Центр окружности
    /// </summary>
    public Vector2f Center;

    /// <summary>
    /// Центр окружности с учетом позиции
    /// </summary>
    public Vector2f CenterWhithPosition;

    /// <summary>
    /// Окружность
    /// </summary>
    /// <param name="radius"> Радиус </param>
    public Circle(float radius) : base(ColliderType.Circle)
    {
        Radius = radius;
        Center = new Vector2f(Radius, Radius);
    }

    /// <summary>
    /// Окружность
    /// </summary>
    /// <param name="radius"> Радиус </param>
    /// <param name="center"> Центр </param>
    public Circle(float radius, Vector2f center) : base(ColliderType.Circle)
    {
        Radius = radius;
        Center = center;
    }

    /// <summary>
    /// Обновление центра окружности
    /// </summary>
    /// <param name="position"> Позиция </param>
    public override Circle Transform(Vector2f position)
    {
        CenterWhithPosition = Center + position;

        return this;
    }

    /// <summary>
    /// Обновление центра окружности с учетом вращения
    /// </summary>
    /// <param name="sin"></param>
    /// <param name="cos"></param>
    /// <param name="position"></param>
    public override Circle Transform(float sin, float cos, Vector2f position)
    {
        CenterWhithPosition = Center + position;

        return this;
    }
}
