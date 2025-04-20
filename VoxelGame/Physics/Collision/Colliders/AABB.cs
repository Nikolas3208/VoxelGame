using SFML.System;

namespace VoxelGame.Physics.Collision.Colliders;

public struct AABB
{
    /// <summary>
    /// Минимальная точка AABB
    /// </summary>
    public Vector2f Min { get; }

    /// <summary>
    /// Максимальная точка AABB
    /// </summary>
    public Vector2f Max { get; }

    /// <summary>
    /// Конструктор AABB
    /// </summary>
    /// <param name="min"> Минимальная позиция </param>
    /// <param name="max"> Максимальная позиция </param>
    public AABB(Vector2f min, Vector2f max)
    {
        Min = min;
        Max = max;
    }

    /// <summary>
    /// Конструктор AABB
    /// </summary>
    /// <param name="minX"> Минимальный Х </param>
    /// <param name="minY"> Минимальный У </param>
    /// <param name="maxX"> Максимальный Х </param>
    /// <param name="maxY"> Максимальный У </param>
    public AABB(float minX, float minY, float maxX, float maxY)
    {
        Min = new Vector2f(minX, minY);
        Max = new Vector2f(maxX, maxY);
    }

    /// <summary>
    /// Обновление AABB на основании полигона
    /// </summary>
    /// <param name="poligon"> Полигон </param>
    /// <returns> AABB </returns>
    public AABB Transform(Polygon poligon)
    {
        float minX = float.MaxValue;
        float minY = float.MaxValue;
        float maxX = float.MinValue;
        float maxY = float.MinValue;

        for (int i = 0; i < poligon.GetVertices().Length; i++)
        {
            Vector2f v = poligon.GetVertices()[i];

            if (v.X < minX) { minX = v.X; }
            if (v.X > maxX) { maxX = v.X; }
            if (v.Y < minY) { minY = v.Y; }
            if (v.Y > maxY) { maxY = v.Y; }
        }

        return new AABB(minX, minY, maxX, maxY);
    }

    /// <summary>
    /// Обновление AABB на основании круга
    /// </summary>
    /// <param name="circle"> Круг </param>
    /// <returns> AABB </returns>
    public AABB Transform(Circle circle)
    {
        float minX = circle.CenterWhithPosition.X - circle.Radius;
        float minY = circle.CenterWhithPosition.Y - circle.Radius;
        float maxX = circle.CenterWhithPosition.X + circle.Radius;
        float maxY = circle.CenterWhithPosition.Y + circle.Radius;

        return new AABB(minX, minY, maxX, maxY);
    }
}
