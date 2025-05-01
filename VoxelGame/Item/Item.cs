namespace VoxelGame.Item
{
    public enum ItemType
    {
        None,
        Weapon,
        Armor,
        Tile,
        Tool,
        Material
    }
    public class Item
    {
        private Craft? _craft;

        /// <summary>
        /// Тип предмета
        /// </summary>
        public ItemType Type { get; set; } = ItemType.None;

        public ItemList ItemList {  get; set; }

        /// <summary>
        /// Имя предмета
        /// </summary>
        public string Name { get; set; } = "Item";

        /// <summary>
        /// Описание предмета
        /// </summary>
        public string Description { get; set; } = "Description of item";

        /// <summary>
        /// Прочность предмета
        /// </summary>
        public float Strength { get; set; } = 1.0f;

        /// <summary>
        /// Урон предмета
        /// </summary>
        public float Damage { get; set; } = 1.0f;

        /// <summary>
        /// Скорость предмета
        /// </summary>
        public float Speed { get; set; } = 1.0f;

        /// <summary>
        /// Максимальное количество предметов в стаке
        /// </summary>
        public int MaxCoutnInStack { get; private set; } = 64;

        /// <summary>
        /// Индекс спрайта предмета
        /// </summary>
        public int SpriteIndex { get; set; } = 0;

        /// <summary>
        /// Конструктор копирования
        /// </summary>
        /// <param name="infoItem"> Копируемый предмет </param>
        public Item(Item infoItem)
        {
            Type = infoItem.Type;
            Name = infoItem.Name;
            Description = infoItem.Description;
            Strength = infoItem.Strength;
            Damage = infoItem.Damage;
            MaxCoutnInStack = infoItem.MaxCoutnInStack;
            SpriteIndex = infoItem.SpriteIndex;
        }

        /// <summary>
        /// Конструктор предмета
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="strength"></param>
        /// <param name="damage"></param>
        /// <param name="spriteIndex"></param>
        /// <param name="maxCountInStack"></param>
        public Item(ItemType type, string name, string description, float strength, float damage, int spriteIndex, int maxCountInStack = 64)
        {
            Type = type;
            Name = name;
            Description = description;
            Strength = strength;
            Damage = damage;
            SpriteIndex = spriteIndex;
            MaxCoutnInStack = maxCountInStack;
        }

        /// <summary>
        /// Конструктор предмета
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="spriteIndex"></param>
        /// <param name="maxCountInStack"></param>
        public Item(ItemType type, string name, int spriteIndex, int maxCountInStack = 64)
        {
            Type = type;
            Name = name;
            SpriteIndex = spriteIndex;
            MaxCoutnInStack = maxCountInStack;
        }

        public Item SetCraft(Craft craft)
        {
            _craft = craft;

            return this;
        }

        public Item SetCraft(int outCount, CraftTool tool = CraftTool.None, params CraftElement[] craftElements)
        {
            _craft = new Craft(outCount, tool, craftElements);

            return this;
        }

        public Craft? GetCraft()
        {
            return _craft;
        }

        public Item SetItem(ItemList itemList)
        {
            ItemList = itemList;

            return this;
        }

        public virtual void Use()
        {

        }
    }
}
