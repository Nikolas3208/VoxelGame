using VoxelGame.Worlds;

namespace VoxelGame.Item
{
    public class ItemTile : Item
    {
        public TileType TileType { get; set; } = TileType.None;
        public ItemTile(TileType type, int spriteIndex, int maxCountInStack = 64) : base(ItemType.Tile, type.ToString(), spriteIndex, maxCountInStack)
        {
            TileType = type;
        }

        public override void Use()
        {
        }
    }
}
