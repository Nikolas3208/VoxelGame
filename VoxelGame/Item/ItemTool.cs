namespace VoxelGame.Item
{
    public enum ToolType
    {
        None,
        Pickaxe,
        Axe,
        Shovel,
    }
    public class ItemTool : Item
    {
        public ToolType ToolType { get; set; } = ToolType.None;

        public ItemTool(ToolType type, string description, float strength, float speed,
            float damage, int spriteIndex) 
            : base(ItemType.Tool, type.ToString(), description, strength, damage, 
                  spriteIndex, 1)
        {
            ToolType = type;
            Speed = speed;
        }
    }
}
