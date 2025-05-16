using SFML.Graphics;
using System.Security.Cryptography.X509Certificates;

namespace VoxelGame.Item
{
    public enum ItemType
    {
        None,
        Weapon,
        Armor,
        Tile,
        Wall,
        Axe,
        Pickaxe,
        Hammer,
        Sword,
        Material,
        All
    }
    public class Item
    {
        /// <summary>
        /// Рецепт этого предмета 
        /// </summary>
        private Craft[] _crafts;

        public Craft? Craft { get; private set; }

        /// <summary>
        /// Действие предмета
        /// </summary>
        protected Action<Item>? action;

        /// <summary>
        /// Тип предмета
        /// </summary>
        public ItemType Type { get; set; } = ItemType.None;

        /// <summary>
        /// Что это за предмет
        /// </summary>
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
        /// Спрайт предмета
        /// </summary>
        public string SpriteName { get; set; }

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
            ItemList = infoItem.ItemList;
            _crafts = infoItem._crafts;
        }

        /// <summary>
        /// Конструктор предмета
        /// </summary>
        /// <param name="type"> Тип </param>
        /// <param name="name"> Имя </param>
        /// <param name="description"> Описание </param>
        /// <param name="strength"> Прочность </param>
        /// <param name="damage"> Урон </param>
        /// <param name="spriteIndex"> Идентификатор спрайта на листе </param>
        /// <param name="maxCountInStack"> Максимальное количество в стаке </param>
        public Item(ItemType type, string name, string description, float strength, float damage, int maxCountInStack = 64)
        {
            Type = type;
            Name = name;
            Description = description;
            Strength = strength;
            Damage = damage;
            MaxCoutnInStack = maxCountInStack;
            SpriteName = "Item_" + ItemList.ToString();
        }

        public Item(ItemType type, string name, int maxCountInStack = 64)
        {
            Type = type;
            Name = name;
            MaxCoutnInStack = maxCountInStack;
            SpriteName = "Item_" + ItemList.ToString();
        }

        /// <summary>
        /// Установить действие
        /// </summary>
        /// <param name="action"> Действие </param>
        public void SetAction(Action<Item> action)
        {
            this.action = action;
        }

        /// <summary>
        /// Получить действие
        /// </summary>
        /// <returns></returns>
        public Action<Item>? GetAction()
        {
            return action;
        }

        /// <summary>
        /// Установить рецепт
        /// </summary>
        /// <param name="craft"> Рецепт </param>
        /// <returns></returns>
        public Item SetCrafts(params Craft[] craft)
        {
            _crafts = craft;

            return this;
        }

        /// <summary>
        /// Получить крафт (рецепт)
        /// </summary>
        /// <returns></returns>
        public Craft[]? GetCrafts()
        {
            return _crafts;
        }

        public void SetCraft(Craft craft) => Craft = craft;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemList"></param>
        /// <returns></returns>
        public Item SetItem(ItemList itemList)
        {
            ItemList = itemList;

            SpriteName = "Item_" + itemList.ToString();

            return this;
        }

        public Item SetSpriteName(string spriteName)
        {
            SpriteName = spriteName;
            return this;
        }

        /// <summary>
        /// При использовании предмета
        /// </summary>
        public void Use()
        {
            action?.Invoke(this);
        }
    }
}
