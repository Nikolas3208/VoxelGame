using SFML.System;

namespace VoxelGame.Worlds.Tile;

public class InfoTile
{
    /// <summary>
    /// ����������� ������ ������
    /// </summary>
    public const int TileSize = 16;

    /// <summary>
    /// ������������� ������
    /// </summary>
    public int Id;

    /// <summary>
    /// ��� ������
    /// </summary>
    public TileType Type { get; set; }

    /// <summary>
    /// ������ ������
    /// </summary>
    public Vector2f Size { get; set; } = new Vector2f(TileSize, TileSize);

    public Chunk Chunk { get; set; }

    /// <summary>
    /// ��������� ������
    /// </summary>
    public float Strength { get; set; } = 1.0f;

    public bool IsWall { get; set; } = false;

    /// <summary>
    /// ������
    /// </summary>
    /// <param name="type"> ��� ������ </param>
    public InfoTile(TileType type)
    {
        Type = type;
    }

    /// <summary>
    /// ������
    /// </summary>
    /// <param name="type"> ��� ������ </param>
    /// <param name="size"> ������ ������ </param>
    public InfoTile(TileType type, Vector2f size) : this(type)
    {
        Size = size;
    }

    public InfoTile(TileType type, float strength) : this(type)
    {
        Strength = strength;
    }

    public InfoTile BreakingTail(float damage)
    {
        Strength -= damage;
        if (Strength <= 0)
        {
            return null;
        }

        return this;
    }
}
