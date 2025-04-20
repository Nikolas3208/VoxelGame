using SFML.System;

namespace VoxelGame.Worlds.Tile;

public class InfoTile
{
    /// <summary>
    /// ����������� ������ ������
    /// </summary>
    public const int TileSize = 32;

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

}
