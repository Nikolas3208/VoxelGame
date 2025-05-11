using VoxelGame.Worlds.Tile;

namespace VoxelGame.Item
{
    public enum ItemList
    {
        None,
        WoodPickaxe,
        WoodAxe,
        WoodShovel,
        StonePickaxe,
        StoneAxe,
        StoneShovel,
        IronPickaxe,
        IronAxe,
        IronShovel,
        Ground,
        GroundWall,
        Grass,
        Stone,
        StoneWall,
        IronOre,
        Oak,
        Birch,
        Leaves,
        OakBoard,
        OakBoardWall,
        BirchBoard,
        BirchBoardWall,
        Stick,
        Workbench,
        Chest,
        Coal,
        Torch,
        IronIngot
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
                //Деревянная кирка
                ItemList.WoodPickaxe => new ItemTool(ItemType.Pickaxe, "Wood Pickaxe", 10, 3, 2, 96)
                .SetCrafts(
                    new Craft(1, items, CraftTool.Workbench, new CraftElement(ItemList.Stick, 4), new CraftElement(ItemList.OakBoard, 6)),
                    new Craft(1, items, CraftTool.Workbench, new CraftElement(ItemList.Stick, 4), new CraftElement(ItemList.BirchBoard, 6)))
                .SetItem(items),

                //Деревянный топор
                ItemList.WoodAxe => new ItemTool(ItemType.Axe, "Wood Axe", 10, 2, 3, 112)
                .SetCrafts(
                    new Craft(1, items, CraftTool.Workbench, new CraftElement(ItemList.Stick, 4), new CraftElement(ItemList.OakBoard, 6)),
                    new Craft(1, items, CraftTool.Workbench, new CraftElement(ItemList.Stick, 4), new CraftElement(ItemList.BirchBoard, 6)))
                .SetItem(items),

                //Деревянная лопата
                ItemList.WoodShovel => new ItemTool(ItemType.Shovel, "Wood Shovel", 10, 3, 2, 80)
                .SetCrafts(
                    new Craft(1, items, CraftTool.Workbench, new CraftElement(ItemList.Stick, 2), new CraftElement(ItemList.OakBoard, 4)),
                    new Craft(1, items, CraftTool.Workbench, new CraftElement(ItemList.Stick, 2), new CraftElement(ItemList.BirchBoard, 4)))
                .SetItem(items),

                //Каменная кирка
                ItemList.StonePickaxe => new ItemTool(ItemType.Pickaxe, "Stone Pickaxe", 10, 5, 2, 97)
                .SetCrafts(
                    new Craft(1, items, CraftTool.Workbench, new CraftElement(ItemList.Stick, 4), new CraftElement(ItemList.Stone, 6)))
                .SetItem(items),

                //Каменная топор
                ItemList.StoneAxe => new ItemTool(ItemType.Axe, "Stone Axe", 10, 4, 3, 113)
                .SetCrafts(
                    new Craft(1, items, CraftTool.Workbench, new CraftElement(ItemList.Stick, 4), new CraftElement(ItemList.Stone, 6)))
                .SetItem(items),

                //Каменная лопата
                ItemList.StoneShovel => new ItemTool(ItemType.Shovel, "Stone Shovel", 10, 5, 2, 81)
                .SetCrafts(
                    new Craft(1, items, CraftTool.Workbench, new CraftElement(ItemList.Stick, 2), new CraftElement(ItemList.Stone, 4)))
                .SetItem(items),

                //Железная кирка
                ItemList.IronPickaxe => new ItemTool(ItemType.Pickaxe, "Iron Pickaxe", 10, 7, 2, 98)
                .SetCrafts(
                    new Craft(1, items, CraftTool.Workbench, new CraftElement(ItemList.Stick, 4), new CraftElement(ItemList.IronIngot, 6)))
                .SetItem(items),

                //Железная топор
                ItemList.IronAxe => new ItemTool(ItemType.Axe, "Iron Axe", 10, 6, 3, 114)
                .SetCrafts(
                    new Craft(1, items, CraftTool.Workbench, new CraftElement(ItemList.Stick, 4), new CraftElement(ItemList.IronIngot, 6)))
                .SetItem(items),

                //Железная лопата
                ItemList.IronShovel => new ItemTool(ItemType.Shovel, "Iron Shovel", 10, 7, 2, 82)
                .SetCrafts(
                    new Craft(1, items, CraftTool.Workbench, new CraftElement(ItemList.Stick, 2), new CraftElement(ItemList.IronIngot, 4)))
                .SetItem(items),

                //Земля
                ItemList.Ground => GetItemTileByTile(TileType.Ground)
                .SetCrafts(new Craft(1, items, CraftTool.None, new CraftElement(ItemList.GroundWall, 1)))
                ,

                //Земляная стена
                ItemList.GroundWall => GetItemTileByTile(TileType.Ground, true)
                .SetCrafts(new Craft(1, items, CraftTool.None, new CraftElement(ItemList.Ground, 1)))
                .SetItem(items),
                //Трава
                ItemList.Grass => GetItemTileByTile(TileType.Grass)
                .SetItem(items),

                //Камень
                ItemList.Stone => GetItemTileByTile(TileType.Stone)
                .SetCrafts(new Craft(1, items, CraftTool.None, new CraftElement(ItemList.StoneWall, 1)))
                .SetItem(items),

                //Каменная стена
                ItemList.StoneWall => GetItemTileByTile(TileType.Stone, true)
                .SetCrafts(new Craft(1, items, CraftTool.None, new CraftElement(ItemList.Stone, 1)))
                .SetItem(items),

                //Железная руда
                ItemList.IronOre => GetItemTileByTile(TileType.IronOre)
                .SetItem(items),

                ItemList.IronIngot => new Item(ItemType.Material, "Iron Ingot", "Iron Ingot", 1, 1, 23)
                .SetCrafts(new Craft(1, items, CraftTool.Stove, new CraftElement(ItemList.IronOre, 1)))
                .SetItem(items),

                //Доска
                ItemList.OakBoard => GetItemTileByTile(TileType.OakBoard)
                .SetCrafts(new Craft(4, items, CraftTool.None, new CraftElement(ItemList.Oak, 1)))
                .SetItem(items),

                //Доска стена
                ItemList.OakBoardWall => GetItemTileByTile(TileType.OakBoard, true)
                .SetCrafts(new Craft(1, items, CraftTool.None, new CraftElement(ItemList.OakBoard, 1)))
                .SetItem(items),

                //Доска
                ItemList.BirchBoard => GetItemTileByTile(TileType.BirchBoard)
                .SetCrafts(new Craft(4, items, CraftTool.None, new CraftElement(ItemList.Birch, 1)))
                .SetItem(items),

                //Доска стена
                ItemList.BirchBoardWall => GetItemTileByTile(TileType.BirchBoard, true)
                .SetCrafts(new Craft(1, items, CraftTool.None, new CraftElement(ItemList.BirchBoard, 1)))
                .SetItem(items),

                //Палка
                ItemList.Stick => new Item(ItemType.Material, "Stick", "Wood Stick", 1, 1, 3 * 16 + 5)
                .SetCrafts(
                    new Craft(4, items, CraftTool.None, new CraftElement(ItemList.OakBoard, 1)),
                    new Craft(4, items, CraftTool.None, new CraftElement(ItemList.BirchBoard, 1)))
                .SetItem(items),

                //Дерево
                ItemList.Oak => GetItemTileByTile(TileType.Oak)
                .SetItem(items),

                ItemList.Birch => GetItemTileByTile(TileType.Birch)
               .SetItem(items),

                //Листва
                ItemList.Leaves => GetItemTileByTile(TileType.Leaves)
                .SetItem(items),

                //Сундук
                ItemList.Chest => new ItemTile(TileType.Chest, GetItemTextureIndexByTileType(TileType.Chest))
                .SetCrafts(
                    new Craft(1, items, CraftTool.Workbench, new CraftElement(ItemList.OakBoard, 8)),
                    new Craft(1, items, CraftTool.Workbench, new CraftElement(ItemList.BirchBoard, 8)))
                .SetItem(items),

                //Верстак
                ItemList.Workbench => new ItemTile(TileType.Workbench, GetItemTextureIndexByTileType(TileType.Workbench), 1)
                .SetCrafts(
                    new Craft(1, items, CraftTool.None, new CraftElement(ItemList.OakBoard, 4)),
                    new Craft(1, items, CraftTool.None, new CraftElement(ItemList.BirchBoard, 4)))
                .SetItem(items),

                //Факел
                ItemList.Torch => GetItemTileByTile(TileType.Torch)
                .SetCrafts(new Craft(4, items, CraftTool.None, new CraftElement(ItemList.Stick, 2), new CraftElement(ItemList.Coal, 1)))
                .SetItem(items),

                //Другой тип
                _ => new Item(ItemType.Tile, "None", "None", 1, 1, 0)
            };
        }
            

        /// <summary>
        /// Получить предмет пилитку по типу плитки
        /// </summary>
        /// <param name="type"> Тип плитки </param>
        /// <param name="isWall"> Это стена? </param>
        /// <returns></returns>
        public static ItemTile GetItemTileByTile(TileType type, bool isWall = false)
        {
            return new ItemTile(type, GetItemTextureIndexByTileType(type), 64, isWall);
        }

        /// <summary>
        /// Получить идентефикатор спрайта на листе, по типу плитки
        /// </summary>
        /// <param name="type"> Тип плитки </param>
        /// <returns></returns>
        public static int GetItemTextureIndexByTileType(TileType type)
        {
            return type switch
            { 
                TileType.Ground => 2,
                TileType.Grass => 3,
                TileType.Stone => 1,
                TileType.Oak => 20,
                TileType.Birch => 117,
                TileType.Leaves => 53,
                TileType.IronOre => 33,
                TileType.OakBoard => 4,
                TileType.BirchBoard => 214,
                TileType.Chest => 27,
                TileType.Workbench => 59,
                TileType.Torch => 80,
                _ => -1
            };
        }
    }
}
