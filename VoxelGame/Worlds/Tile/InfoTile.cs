using SFML.System;
using VoxelGame.Item;

namespace VoxelGame.Worlds.Tile;

public class InfoTile
{
    /// <summary>
    /// ������ ������
    /// </summary>
    public const int TileSize = 16;

    /// <summary>
    /// ������������� ������
    /// </summary>
    public int Id;

    /// <summary>
    /// ��� ������
    /// </summary>
    public TileType Type { get; set; } = TileType.Ground;

    public ItemType SpecificTool { get; set; } = ItemType.None;

    /// <summary>
    /// ������ ������
    /// </summary>
    public Vector2f Size { get; set; } = new Vector2f(TileSize, TileSize);

    /// <summary>
    /// ���� ��������
    /// </summary>
    public Chunk? Chunk { get; set; }

    /// <summary>
    /// ��������� ������
    /// </summary>
    public float Strength { get; set; } = 1.0f;

    /// <summary>
    /// � ������� ����� �����������?
    /// </summary>
    public bool IsCollide { get; set; } = true;

    /// <summary>
    /// ��� ������?
    /// </summary>
    public bool IsTree { get; set; } = false;

    /// <summary>
    /// ��� ������?
    /// </summary>
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

    /// <summary>
    /// ������
    /// </summary>
    /// <param name="type"> ��� </param>
    /// <param name="strength"> ��������� </param>
    public InfoTile(TileType type, float strength, ItemType specificTool = ItemType.None) : this(type)
    {
        Strength = strength;
        SpecificTool = specificTool;
    }

    /// <summary>
    /// ������ ������ (������� ����)
    /// </summary>
    /// <param name="damage"> ���� (����)  </param>
    /// <returns> ������, ���� ���������� 0 ����� null </returns>
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
