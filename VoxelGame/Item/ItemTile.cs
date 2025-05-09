using VoxelGame.Worlds;

namespace VoxelGame.Item
{
    public class ItemTile : Item
    {
        public bool IsWall = false;

        public TileType TileType { get; set; } = TileType.None;
        public ItemTile(TileType type, int spriteIndex, int maxCountInStack = 64, bool isWall = false) : base(ItemType.Tile, type.ToString(), spriteIndex, maxCountInStack)
        {
            TileType = type;
            IsWall = isWall;
        }

        public override void Use()
        {
        }
    }
}
