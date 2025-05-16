namespace VoxelGame.Item
{
    public enum CraftTool
    {
        None = 1,
        Workbench = 2 << 1,
        Stove = 3 << 3,
        Anvil = 4 << 4,
        All = None | Workbench | Stove | Anvil
    }

    public class Craft
    {
        /// <summary>
        /// Элементы крафта
        /// </summary>
        public CraftElement[] Items { get; }

        public ItemList OutCraft { get; }

        /// <summary>
        /// Количество предметов, получаемых в результате крафта
        /// </summary>
        public int OutCount { get; } = 1;

        /// <summary>
        /// Инструмент для крафта ( печка, верстак )
        /// </summary>
        public CraftTool Tool { get; } = CraftTool.All;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="itemCrafts"></param>
        public Craft(int outCount, ItemList outCraft, CraftTool tool, params CraftElement[] itemCrafts)
        {
            OutCraft = outCraft;
            OutCount = outCount;
            Tool = tool;
            Items = itemCrafts;
        }
    }
}
