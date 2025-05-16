using SFML.Graphics;
using VoxelGame.Resources;
using VoxelGame.Worlds.Tile;

namespace VoxelGame.Item
{
    public enum ItemList
    {
        None,
        CopperPickaxe,
        CopperAxe,
        CopperSword,
        OakWoodSword,
        Ground,
        GroundWall,
        Grass,
        Stone,
        StoneWall,
        IronOre,
        CopperOre,
        Oak,
        Leaves,
        OakBoard,
        OakBoardWall,
        Workbench,
        Stove,
        Chest,
        Torch,
        IronIngot,
        CopperIngot,
        Mucus
    }

    public static class Items
    {
        /// <summary>
        /// Получить предмет по его типу
        /// </summary>
        /// <param name="items"> Тип предмета </param>
        /// <returns></returns>
        public static Item GetItem(ItemList items)
        {
            return items switch
            {
                //Медная кирка
                ItemList.CopperPickaxe => new ItemTool(ItemType.Pickaxe, "Copper Pickaxe", 10, 3, 1, 2)
                .SetCrafts(
                    new Craft(1, items, CraftTool.Workbench, new CraftElement(ItemList.OakBoard, 4), new CraftElement(ItemList.CopperIngot, 6)))
                .SetItem(items)
                .SetSpriteName("Item_Pickaxe"),

                //Медный топор
                ItemList.CopperAxe => new ItemTool(ItemType.Axe, "Copper Axe", 10, 2, 1, 3)
                .SetCrafts(
                    new Craft(1, items, CraftTool.Workbench, new CraftElement(ItemList.OakBoard, 4), new CraftElement(ItemList.CopperIngot, 6)))
                .SetItem(items)
                .SetSpriteName("Item_Axe"),

                //Медный меч
                ItemList.CopperSword => new ItemTool(ItemType.Sword, "Copper Sword", 10, 1, 1, 5)
                .SetCrafts(
                    new Craft(1, items, CraftTool.Workbench, new CraftElement(ItemList.OakBoard, 4), new CraftElement(ItemList.CopperIngot, 6)))
                .SetItem(items)
                .SetSpriteName("Item_Sword"),

                //Деревянный меч
                ItemList.OakWoodSword => new ItemTool(ItemType.Sword, "Wood Sword", 10, 1, 1, 3)
                .SetCrafts(new Craft(1, ItemList.OakWoodSword, CraftTool.Workbench, new CraftElement(ItemList.OakBoard, 12)))
                .SetItem(items)
                .SetSpriteName("Item_WoodSword"),

                //Земля
                ItemList.Ground => GetItemTileByTile(TileType.Ground)
                .SetCrafts(new Craft(1, items, CraftTool.All, new CraftElement(ItemList.GroundWall, 1)))
                .SetItem(items),

                //Земляная стена
                ItemList.GroundWall => GetItemTileByWall(WallType.GroundWall)
                .SetCrafts(new Craft(1, items, CraftTool.All, new CraftElement(ItemList.Ground, 1)))
                .SetItem(items),

                //Трава
                ItemList.Grass => GetItemTileByTile(TileType.Grass)
                .SetItem(items),

                //Камень
                ItemList.Stone => GetItemTileByTile(TileType.Stone)
                .SetCrafts(new Craft(1, items, CraftTool.All, new CraftElement(ItemList.StoneWall, 1)))
                .SetItem(items),

                //Каменная стена
                ItemList.StoneWall => GetItemTileByWall(WallType.StoneWall)
                .SetCrafts(new Craft(1, items, CraftTool.All, new CraftElement(ItemList.Stone, 1)))
                .SetItem(items),

                //Железная руда
                ItemList.IronOre => GetItemTileByTile(TileType.IronOre)
                .SetItem(items),

                //Железный слиток
                ItemList.IronIngot => new Item(ItemType.Material, "Iron Ingot", "Iron Ingot", 1, 1)
                .SetCrafts(new Craft(1, items, CraftTool.Stove, new CraftElement(ItemList.IronOre, 1)))
                .SetItem(items),

                //Доска
                ItemList.OakBoard => GetItemTileByTile(TileType.OakBoard)
                .SetCrafts(new Craft(4, items, CraftTool.All, new CraftElement(ItemList.Oak, 1)))
                .SetItem(items),

                //Доска стена
                ItemList.OakBoardWall => GetItemTileByWall(WallType.OakBoardWall)
                .SetCrafts(new Craft(1, items, CraftTool.All, new CraftElement(ItemList.OakBoard, 1)))
                .SetItem(items),

                //Дерево
                ItemList.Oak => GetItemTileByTile(TileType.Oak)
                .SetItem(items),

                //Листва
                ItemList.Leaves => GetItemTileByTile(TileType.Leaves)
                .SetItem(items),

                //Сундук
                ItemList.Chest => GetItemTileByTile(TileType.Chest)
                .SetCrafts(
                    new Craft(1, items, CraftTool.Workbench, new CraftElement(ItemList.OakBoard, 8), new CraftElement(ItemList.IronIngot, 5)))
                .SetItem(items),

                //Верстак
                ItemList.Workbench => new ItemTile(TileType.Workbench, 1)
                .SetCrafts(
                    new Craft(1, items, CraftTool.All, new CraftElement(ItemList.OakBoard, 4)))
                .SetItem(items),

                //Печка
                ItemList.Stove => GetItemTileByTile(TileType.Stove)
                .SetCrafts(new Craft(1, items, CraftTool.Workbench, new CraftElement(ItemList.Stone, 15)))
                .SetItem(items),

                //Факел
                ItemList.Torch => GetItemTileByTile(TileType.Torch)
                .SetCrafts(new Craft(4, items, CraftTool.All, new CraftElement(ItemList.OakBoard, 1), new CraftElement(ItemList.Mucus, 1)))
                .SetItem(items),

                //Медная руда
                ItemList.CopperOre => new Item(ItemType.Material, items.ToString())
                .SetItem(items),

                //Медный слиток
                ItemList.CopperIngot => GetItemTileByTile(TileType.CopperIngot)
                .SetItem(items),

                //Слизь
                ItemList.Mucus => new Item(ItemType.Material, items.ToString())
                .SetItem(items),

                //Другой тип
                _ => throw new Exception($"Item is not found. {items}")
            };
        }
            

        /// <summary>
        /// Получить предмет пилитку по типу плитки
        /// </summary>
        /// <param name="type"> Тип плитки </param>
        /// <param name="isWall"> Это стена? </param>
        /// <returns></returns>
        public static ItemTile GetItemTileByTile(TileType type)
        {
            return new ItemTile(type,  64);
        }

        public static ItemTile GetItemTileByWall(WallType type)
        {
            return new ItemTile(type, 64);
        }
    }
}
