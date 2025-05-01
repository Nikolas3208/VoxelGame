using VoxelGame.Worlds;

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
        Ground,
        Grass,
        Stone,
        IronOre,
        Wood,
        Board,
        Stick,
        Workbench
    }

    public static class Items
    {
        public static Item GetItem(ItemList items)
        {
            return items switch
            {
                ItemList.WoodPickaxe => new ItemTool(ToolType.Pickaxe, "Wood Pickaxe", 10, 3, 2, 6 * 16)
                .SetCraft(1, CraftTool.Workbench, new CraftElement(ItemList.Stick, 4), new CraftElement(ItemList.Board, 6))
                .SetItem(items),

                ItemList.WoodAxe => new ItemTool(ToolType.Axe, "Wood Axe", 10, 1, 3, 7 * 16)
                .SetCraft(1, CraftTool.Workbench, new CraftElement(ItemList.Stick, 4), new CraftElement(ItemList.Board, 6))
                .SetItem(items),

                ItemList.WoodShovel => new ItemTool(ToolType.Shovel, "Wood Shovel", 10, 3, 2, 5 * 16)
                .SetCraft(1, CraftTool.Workbench, new CraftElement(ItemList.Stick, 2), new CraftElement(ItemList.Board, 4))
                .SetItem(items),

                ItemList.Ground => GetItemTileByTile(TileType.Ground),
                ItemList.Grass => GetItemTileByTile(TileType.Ground),
                ItemList.Stone => GetItemTileByTile(TileType.Stone),
                ItemList.IronOre => GetItemTileByTile(TileType.IronOre)
                .SetItem(ItemList.IronOre),

                ItemList.Board => GetItemTileByTile(TileType.Board)
                .SetCraft(4, craftElements: new CraftElement(ItemList.Wood, 1))
                .SetItem(items),

                ItemList.Stick => new Item(ItemType.Material, "Stick", "Wood Stick", 1, 1, 3 * 16 + 5)
                .SetCraft(4, craftElements: new CraftElement(ItemList.Board, 1))
                .SetItem(items),

                ItemList.Wood => GetItemTileByTile(TileType.Wood)
                .SetItem(ItemList.Wood),
                _ => new Item(ItemType.Tile, "None", "None", 1, 1, 0)
            };
        }
            

        public static ItemTile GetItemTileByTile(TileType type)
        {
            return new ItemTile(type, GetItemTextureIndexByTileType(type), 64);
        }

        public static int GetItemTextureIndexByTileType(TileType type)
        {
            return type switch
            { 
                TileType.Ground => 2,
                TileType.Grass => 3,
                TileType.Stone => 1,
                TileType.Wood => 20,
                TileType.Leaves => 53,
                TileType.IronOre => 33,
                TileType.Board => 4,
                _ => -1
            };
        }
    }
}
