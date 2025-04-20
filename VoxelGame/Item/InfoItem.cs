using SFML.Graphics;

namespace VoxelGame.Item
{
    public enum ItemType
    {
        None,
        Weapon,
        Armor,
        Tile,
        Tool
    }
    public abstract class InfoItem
    {
        public ItemType Type { get; set; } = ItemType.None;
        public string Name { get; set; } = "Item";
        public int MaxCoutnInStack { get; private set; } = 64;

        public int SpriteIndex { get; set; } = 0;

        public InfoItem(ItemType type, string name, int spriteIndex, int maxCountInStack = 64)
        {
            Type = type;
            Name = name;
            SpriteIndex = spriteIndex;
            MaxCoutnInStack = maxCountInStack;
        }
    }
}
