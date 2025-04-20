using SFML.System;

namespace VoxelGame.Physics.Collision.Colliders;

public class Polygon : Collider
{
    /// <summary>
    /// ����������� ������ ������
    /// </summary>
    private Vector2f[] _vertices;

    /// <summary>
    /// �������� ������ ������
    /// </summary>
    private Vector2f[] _originalVertices;

    /// <summary>
    /// ����� ��������
    /// </summary>
    public Vector2f Center { get; private set; }

    /// <summary>
    /// ������ ��������
    /// </summary>
    public Vector2f Size { get; private set; }

    /// <summary>
    /// �������
    /// </summary>
    /// <param name="vertices"> ������� </param>
    public Polygon(Vector2f[] vertices) : base(ColliderType.Poligon)
    {
        _originalVertices = vertices;
        _vertices = vertices;
        Center = new Vector2f(0, 0);
    }

    /// <summary>
    /// ���������� ������
    /// </summary>
    /// <param name="position"> ������ </param>
    public override Polygon Transform(Vector2f position)
    {
        for (int i = 0; i < _vertices.Length; i++)
        {
            _vertices[i] = _originalVertices[i] + position;
        }

        return this;
    }

    /// <summary>
    /// ���������� ������ � ������ ��������
    /// </summary>
    /// <param name="sin"> ����� </param>
    /// <param name="cos"> ������� </param>
    /// <param name="position"> ������� </param>
    public override Polygon Transform(float sin, float cos, Vector2f position)
    {
        for (int i = 0; i < _vertices.Length; i++)
        {
            float x = _originalVertices[i].X * cos - _originalVertices[i].Y * sin;
            float y = _originalVertices[i].X * sin + _originalVertices[i].Y * cos;

            _vertices[i] = new Vector2f(x, y) + position;
        }

        return this;
    }

    /// <summary>
    /// �������� ������ ������
    /// </summary>
    /// <returns> ������ ������ </returns>
    public Vector2f[] GetVertices()
    {
        return _vertices;
    }

    /// <summary>
    /// �������� ����� ��������
    /// </summary>
    /// <param name="vertices"></param>
    /// <returns> Vector2f ����� </returns>
    public Vector2f GetCenterPolygon()
    {
        float sumX = 0f;
        float sumY = 0f;

        for (int i = 0; i < _vertices.Length; i++)
        {
            Vector2f v = _vertices[i];
            sumX += v.X;
            sumY += v.Y;
        }

        return new Vector2f(sumX, sumY) / _vertices.Length;
    }

    /// <summary>
    /// ������� ������������� �������
    /// </summary>
    /// <param name="width"> ������ </param>
    /// <param name="height"> ������ </param>
    /// <returns> ������� </returns>
    public static Polygon BoxPolygon(float width, float height)
    {
        Vector2f[] vertices = new Vector2f[4];
        vertices[0] = new Vector2f(-width / 2, -height / 2);
        vertices[1] = new Vector2f(-width / 2, height / 2);
        vertices[2] = new Vector2f(width / 2, height / 2);
        vertices[3] = new Vector2f(width / 2, -height / 2);

        return new Polygon(vertices) { Size = new Vector2f(width, height) };
    }
}
