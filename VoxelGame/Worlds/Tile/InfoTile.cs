using SFML.System;
using VoxelGame.Item;

namespace VoxelGame.Worlds.Tile;

public class InfoTile
{
    /// <summary>
    /// Размер плитки
    /// </summary>
    public const int TileSize = 16;

    /// <summary>
    /// Идентификатор плитки
    /// </summary>
    public int Id;

    /// <summary>
    /// Тип плитки
    /// </summary>
    public TileType Type { get; set; } = TileType.Ground;

    public ItemType SpecificTool { get; set; } = ItemType.None;

    /// <summary>
    /// Размер плитки
    /// </summary>
    public Vector2f Size { get; set; } = new Vector2f(TileSize, TileSize);

    /// <summary>
    /// Чанк родитель
    /// </summary>
    public Chunk? Chunk { get; set; }

    /// <summary>
    /// Прочность плитки
    /// </summary>
    public float Strength { get; set; } = 1.0f;

    /// <summary>
    /// С плиткой можно столкнуться?
    /// </summary>
    public bool IsCollide { get; set; } = true;

    /// <summary>
    /// Это дерево?
    /// </summary>
    public bool IsTree { get; set; } = false;

    /// <summary>
    /// Это стенка?
    /// </summary>
    public bool IsWall { get; set; } = false;

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

    /// <summary>
    /// Плитка
    /// </summary>
    /// <param name="type"> Тип </param>
    /// <param name="strength"> Прочность </param>
    public InfoTile(TileType type, float strength, ItemType specificTool = ItemType.None) : this(type)
    {
        Strength = strength;
        SpecificTool = specificTool;
    }

    /// <summary>
    /// Ломаем плитку (наносим урон)
    /// </summary>
    /// <param name="damage"> Урон (сила)  </param>
    /// <returns> Плитка, если прочтность 0 тогда null </returns>
    public InfoTile? BreakingTail(float damage, ItemType tool)
    {
        if (tool == SpecificTool)
        {
            Strength -= damage;
        }
        else
        {
            Strength -= damage * 0.5f;
        }

        if (Strength <= 0)
        {
            return null;
        }

        return this;
    }
}
