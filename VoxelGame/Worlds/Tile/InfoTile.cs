using SFML.System;

namespace VoxelGame.Worlds.Tile;

public class InfoTile
{
    /// <summary>
    /// Минимальный размер плитки
    /// </summary>
    public const int TileSize = 32;

    /// <summary>
    /// Идентификатор плитки
    /// </summary>
    public int Id;

    /// <summary>
    /// Тип плитки
    /// </summary>
    public TileType Type { get; set; }

    /// <summary>
    /// Размер плитки
    /// </summary>
    public Vector2f Size { get; set; } = new Vector2f(TileSize, TileSize);

    /// <summary>
    /// Плитка
    /// </summary>
    /// <param name="type"> Тип плитки </param>
    public InfoTile(TileType type)
    {
        Type = type;
    }

    /// <summary>
    /// Плитка
    /// </summary>
    /// <param name="type"> Тип плитки </param>
    /// <param name="size"> Размер плитки </param>
    public InfoTile(TileType type, Vector2f size) : this(type)
    {
        Size = size;
    }

}
