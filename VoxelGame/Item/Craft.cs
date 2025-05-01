namespace VoxelGame.Item
{
    public enum CraftTool
    {
        None,
        Workbench,
        Stove,
        Anvil
    }

    public class Craft
    {
        /// <summary>
        /// Элементы крафта
        /// </summary>
        public CraftElement[] Items { get; }

        /// <summary>
        /// Количество предметов, получаемых в результате крафта
        /// </summary>
        public int OutCount { get; } = 1;

        /// <summary>
        /// Инструмент для крафта ( печка, верстак )
        /// </summary>
        public CraftTool Tool { get; } = CraftTool.None;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="itemCrafts"></param>
        public Craft(int outCount, CraftTool tool, params CraftElement[] itemCrafts)
        {
            OutCount = outCount;
            Tool = tool;
            Items = itemCrafts;
        }
    }
}
