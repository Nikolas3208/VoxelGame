using SFML.System;

namespace VoxelGame.Physics.Collision.Colliders;

public class Polygon : Collider
{
    /// <summary>
    /// Обновленный массив вершин
    /// </summary>
    private Vector2f[] _vertices;

    /// <summary>
    /// Исходный массив вершин
    /// </summary>
    private Vector2f[] _originalVertices;

    /// <summary>
    /// Центр полигона
    /// </summary>
    public Vector2f Center { get; private set; }

    /// <summary>
    /// Размер полигона
    /// </summary>
    public Vector2f Size { get; private set; }

    /// <summary>
    /// Полигон
    /// </summary>
    /// <param name="vertices"> Вершины </param>
    public Polygon(Vector2f[] vertices) : base(ColliderType.Poligon)
    {
        _originalVertices = vertices;
        _vertices = vertices;
        Center = new Vector2f(0, 0);
    }

    /// <summary>
    /// Обновление вершин
    /// </summary>
    /// <param name="position"> Позиия </param>
    public override Polygon Transform(Vector2f position)
    {
        for (int i = 0; i < _vertices.Length; i++)
        {
            _vertices[i] = _originalVertices[i] + position;
        }

        return this;
    }

    /// <summary>
    /// Обновление вершин с учетом вращения
    /// </summary>
    /// <param name="sin"> Синус </param>
    /// <param name="cos"> Косинус </param>
    /// <param name="position"> Позиция </param>
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
    /// Получить массив вершин
    /// </summary>
    /// <returns> Массив вершин </returns>
    public Vector2f[] GetVertices()
    {
        return _vertices;
    }

    /// <summary>
    /// Получить центр полигона
    /// </summary>
    /// <param name="vertices"></param>
    /// <returns> Vector2f центр </returns>
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
    /// Создать прямоугольный полигон
    /// </summary>
    /// <param name="width"> Ширина </param>
    /// <param name="height"> Высота </param>
    /// <returns> Полигон </returns>
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
