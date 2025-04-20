using SFML.System;

namespace VoxelGame.Physics.Collision.Colliders;

public class Circle : Collider
{
    /// <summary>
    /// ������ ����������
    /// </summary>
    public float Radius;

    /// <summary>
    /// ����� ����������
    /// </summary>
    public Vector2f Center;

    /// <summary>
    /// ����� ���������� � ������ �������
    /// </summary>
    public Vector2f CenterWhithPosition;

    /// <summary>
    /// ����������
    /// </summary>
    /// <param name="radius"> ������ </param>
    public Circle(float radius) : base(ColliderType.Circle)
    {
        Radius = radius;
        Center = new Vector2f(Radius, Radius);
    }

    /// <summary>
    /// ����������
    /// </summary>
    /// <param name="radius"> ������ </param>
    /// <param name="center"> ����� </param>
    public Circle(float radius, Vector2f center) : base(ColliderType.Circle)
    {
        Radius = radius;
        Center = center;
    }

    /// <summary>
    /// ���������� ������ ����������
    /// </summary>
    /// <param name="position"> ������� </param>
    public override Circle Transform(Vector2f position)
    {
        CenterWhithPosition = Center + position;

        return this;
    }

    /// <summary>
    /// ���������� ������ ���������� � ������ ��������
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
