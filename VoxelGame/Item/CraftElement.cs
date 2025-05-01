namespace VoxelGame.Item
{
    public class CraftElement
    {
        /// <summary>
        /// Предмет
        /// </summary>
        public ItemList Item { get; }

        /// <summary>
        /// Количество предметов
        /// </summary>
        public int Count { get; }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="item"></param>
        /// <param name="count"></param>
        public CraftElement(ItemList item, int count)
        {
            Item = item;
            Count = count;
        }
    }
}
