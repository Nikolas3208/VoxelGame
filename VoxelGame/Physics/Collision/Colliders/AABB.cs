using SFML.System;

namespace VoxelGame.Physics.Collision.Colliders;

public struct AABB
{
    /// <summary>
    /// ����������� ����� AABB
    /// </summary>
    public Vector2f Min { get; }

    /// <summary>
    /// ������������ ����� AABB
    /// </summary>
    public Vector2f Max { get; }

    /// <summary>
    /// ����������� AABB
    /// </summary>
    /// <param name="min"> ����������� ������� </param>
    /// <param name="max"> ������������ ������� </param>
    public AABB(Vector2f min, Vector2f max)
    {
        Min = min;
        Max = max;
    }

    /// <summary>
    /// ����������� AABB
    /// </summary>
    /// <param name="minX"> ����������� � </param>
    /// <param name="minY"> ����������� � </param>
    /// <param name="maxX"> ������������ � </param>
    /// <param name="maxY"> ������������ � </param>
    public AABB(float minX, float minY, float maxX, float maxY)
    {
        Min = new Vector2f(minX, minY);
        Max = new Vector2f(maxX, maxY);
    }

    /// <summary>
    /// ���������� AABB �� ��������� ��������
    /// </summary>
    /// <param name="poligon"> ������� </param>
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
    /// ���������� AABB �� ��������� �����
    /// </summary>
    /// <param name="circle"> ���� </param>
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
