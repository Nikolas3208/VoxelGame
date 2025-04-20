namespace VoxelGame.Item
{
    public class TileItem : InfoItem
    {
        public TileItem(string name, int spriteIndex, int maxCountInStack = 64) : base(ItemType.Tile, name, spriteIndex, maxCountInStack)
        {
        }
    }
}
