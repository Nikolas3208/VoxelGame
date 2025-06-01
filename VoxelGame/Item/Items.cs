using VoxelGame.Worlds.Tile;

namespace VoxelGame.Item
{
    /// <summary>
    /// Перечисление всех доступных предметов в игре.
    /// </summary>
    public enum ItemList
    {
        /// <summary>
        /// Отсутствие предмета.
        /// </summary>
        None,

        /// <summary>
        /// Медная кирка — инструмент для добычи ресурсов.
        /// </summary>
        CopperPickaxe,

        /// <summary>
        /// Медный топор — инструмент для рубки деревьев.
        /// </summary>
        CopperAxe,

        /// <summary>
        /// Медный меч — оружие для ближнего боя.
        /// </summary>
        CopperSword,

        /// <summary>
        /// Деревянный меч — начальное оружие для ближнего боя.
        /// </summary>
        OakWoodSword,

        /// <summary>
        /// Земля — базовый строительный материал.
        /// </summary>
        Ground,

        /// <summary>
        /// Земляная стена — декоративный элемент или защита.
        /// </summary>
        GroundWall,

        /// <summary>
        /// Трава — декоративный элемент или поверхность.
        /// </summary>
        Grass,

        /// <summary>
        /// Камень — строительный материал или ресурс.
        /// </summary>
        Stone,

        /// <summary>
        /// Каменная стена — декоративный элемент или защита.
        /// </summary>
        StoneWall,

        /// <summary>
        /// Железная руда — ресурс для создания железных предметов.
        /// </summary>
        IronOre,

        /// <summary>
        /// Медная руда — ресурс для создания медных предметов.
        /// </summary>
        CopperOre,

        /// <summary>
        /// Дерево — ресурс для создания досок и других предметов.
        /// </summary>
        Oak,

        /// <summary>
        /// Листва — декоративный элемент.
        /// </summary>
        Leaves,

        /// <summary>
        /// Доска — строительный материал.
        /// </summary>
        OakBoard,

        /// <summary>
        /// Доска стена — декоративный элемент или защита.
        /// </summary>
        OakBoardWall,

        /// <summary>
        /// Верстак — инструмент для создания сложных предметов.
        /// </summary>
        Workbench,

        /// <summary>
        /// Печка — инструмент для обработки руды.
        /// </summary>
        Stove,

        /// <summary>
        /// Сундук — хранилище для предметов.
        /// </summary>
        Chest,

        /// <summary>
        /// Дверь — элемент для создания проходов.
        /// </summary>
        Door,

        /// <summary>
        /// Факел — источник света.
        /// </summary>
        Torch,

        /// <summary>
        /// Железный слиток — обработанный ресурс для создания предметов.
        /// </summary>
        IronIngot,

        /// <summary>
        /// Медный слиток — обработанный ресурс для создания предметов.
        /// </summary>
        CopperIngot,

        /// <summary>
        /// Слизь — ресурс для создания факелов и других предметов.
        /// </summary>
        Gel
    }

    /// <summary>
    /// Класс для работы с предметами в игре.
    /// Содержит методы для получения предметов и их создания.
    /// </summary>
    public static class Items
    {
        /// <summary>
        /// Получить предмет по его типу.
        /// </summary>
        /// <param name="items">Тип предмета из перечисления <see cref="ItemList"/>.</param>
        /// <returns>Экземпляр предмета.</returns>
        public static Item GetItem(ItemList items)
        {
            return items switch
            {
                // Медная кирка
                ItemList.CopperPickaxe => new ItemTool(ItemType.Pickaxe, "Copper Pickaxe", 10, 1, 1, 2)
                .SetCrafts(
                    new Craft(1, items, CraftTool.Workbench, new CraftElement(ItemList.OakBoard, 4), new CraftElement(ItemList.CopperIngot, 6)))
                .SetItem(items)
                .SetSpriteName("Item_Pickaxe"),

                // Медный топор
                ItemList.CopperAxe => new ItemTool(ItemType.Axe, "Copper Axe", 10, 1, 1, 3)
                .SetCrafts(
                    new Craft(1, items, CraftTool.Workbench, new CraftElement(ItemList.OakBoard, 4), new CraftElement(ItemList.CopperIngot, 6)))
                .SetItem(items)
                .SetSpriteName("Item_Axe"),

                // Медный меч
                ItemList.CopperSword => new ItemTool(ItemType.Sword, "Copper Sword", 10, 1, 1, 5)
                .SetCrafts(
                    new Craft(1, items, CraftTool.Workbench, new CraftElement(ItemList.OakBoard, 4), new CraftElement(ItemList.CopperIngot, 6)))
                .SetItem(items)
                .SetSpriteName("Item_Sword"),

                // Деревянный меч
                ItemList.OakWoodSword => new ItemTool(ItemType.Sword, "Wood Sword", 10, 1, 1, 3)
                .SetCrafts(new Craft(1, ItemList.OakWoodSword, CraftTool.Workbench, new CraftElement(ItemList.OakBoard, 12)))
                .SetItem(items)
                .SetSpriteName("Item_WoodSword"),

                // Земля
                ItemList.Ground => GetItemTileByTile(TileType.Ground)
                .SetCrafts(new Craft(1, items, CraftTool.All, new CraftElement(ItemList.GroundWall, 1)))
                .SetItem(items),

                // Земляная стена
                ItemList.GroundWall => GetItemTileByWall(WallType.GroundWall)
                .SetCrafts(new Craft(1, items, CraftTool.All, new CraftElement(ItemList.Ground, 1)))
                .SetItem(items),

                // Трава
                ItemList.Grass => GetItemTileByTile(TileType.Grass)
                .SetItem(items),

                // Камень
                ItemList.Stone => GetItemTileByTile(TileType.Stone)
                .SetCrafts(new Craft(1, items, CraftTool.All, new CraftElement(ItemList.StoneWall, 1)))
                .SetItem(items),

                // Каменная стена
                ItemList.StoneWall => GetItemTileByWall(WallType.StoneWall)
                .SetCrafts(new Craft(1, items, CraftTool.All, new CraftElement(ItemList.Stone, 1)))
                .SetItem(items),

                // Железная руда
                ItemList.IronOre => GetItemTileByTile(TileType.IronOre)
                .SetItem(items),

                // Железный слиток
                ItemList.IronIngot => new Item(ItemType.Material, "Iron Ingot", "Iron Ingot", 1, 1)
                .SetCrafts(new Craft(1, items, CraftTool.Stove, new CraftElement(ItemList.IronOre, 1)))
                .SetItem(items),

                // Доска
                ItemList.OakBoard => GetItemTileByTile(TileType.OakBoard)
                .SetCrafts(new Craft(4, items, CraftTool.All, new CraftElement(ItemList.Oak, 1)))
                .SetItem(items),

                // Доска стена
                ItemList.OakBoardWall => GetItemTileByWall(WallType.OakBoardWall)
                .SetCrafts(new Craft(1, items, CraftTool.All, new CraftElement(ItemList.OakBoard, 1)))
                .SetItem(items),

                // Дерево
                ItemList.Oak => GetItemTileByTile(TileType.Oak)
                .SetItem(items),

                // Листва
                ItemList.Leaves => GetItemTileByTile(TileType.Leaves)
                .SetItem(items),

                // Сундук
                ItemList.Chest => GetItemTileByTile(TileType.Chest)
                .SetCrafts(
                    new Craft(1, items, CraftTool.Workbench, new CraftElement(ItemList.OakBoard, 8), new CraftElement(ItemList.IronIngot, 5)))
                .SetItem(items),

                // Верстак
                ItemList.Workbench => new ItemTile(TileType.Workbench, 1)
                .SetCrafts(
                    new Craft(1, items, CraftTool.All, new CraftElement(ItemList.OakBoard, 4)))
                .SetItem(items),

                // Печка
                ItemList.Stove => GetItemTileByTile(TileType.Stove, 1)
                .SetCrafts(new Craft(1, items, CraftTool.Workbench, new CraftElement(ItemList.Stone, 15)))
                .SetItem(items),

                // Дверь
                ItemList.Door => GetItemTileByTile(TileType.Door)
                .SetCrafts(new Craft(1, items, CraftTool.Workbench, new CraftElement(ItemList.OakBoard, 6)))
                .SetItem(items),

                // Факел
                ItemList.Torch => GetItemTileByTile(TileType.Torch)
                .SetCrafts(new Craft(4, items, CraftTool.All, new CraftElement(ItemList.OakBoard, 1), new CraftElement(ItemList.Gel, 1)))
                .SetItem(items),

                // Медная руда
                ItemList.CopperOre => new Item(ItemType.Material, items.ToString())
                .SetItem(items),

                // Медный слиток
                ItemList.CopperIngot => GetItemTileByTile(TileType.CopperIngot)
                .SetItem(items),

                // Слизь
                ItemList.Gel => new Item(ItemType.Material, items.ToString())
                .SetItem(items),

                // Другой тип
                _ => throw new Exception($"Item is not found. {items}")
            };
        }

        /// <summary>
        /// Получить предмет плитку по типу плитки.
        /// </summary>
        /// <param name="type">Тип плитки.</param>
        /// <param name="maxCountInStack">Максимальное количество предметов в стаке.</param>
        /// <returns>Экземпляр предмета плитки.</returns>
        public static ItemTile GetItemTileByTile(TileType type, int maxCountInStack = 64)
        {
            return new ItemTile(type, maxCountInStack);
        }

        /// <summary>
        /// Получить предмет стенку по типу стены.
        /// </summary>
        /// <param name="type">Тип стены.</param>
        /// <param name="maxCountInStack">Максимальное количество предметов в стаке.</param>
        /// <returns>Экземпляр предмета стены.</returns>
        public static ItemTile GetItemTileByWall(WallType type, int maxCountInStack = 64)
        {
            return new ItemTile(type, maxCountInStack);
        }
    }
}