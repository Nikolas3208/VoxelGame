namespace VoxelGame.Worlds.Tile
{
    public static class Tiles
    {
        public static InfoTile GetTile(TileType tileType, bool isWall = false)
        {
            return tileType switch
            {
                TileType.Ground => new InfoTile(tileType, 0.5f, Item.ItemType.Shovel),
                TileType.Grass => new InfoTile(tileType, 0.5f, Item.ItemType.Shovel),
                TileType.Stone => new InfoTile(tileType, 1.5f, Item.ItemType.Pickaxe),
                TileType.Oak => new InfoTile(tileType, 1, Item.ItemType.Axe) { IsTree = true, IsCollide = false },
                TileType.Birch => new InfoTile(tileType, 1, Item.ItemType.Axe) { IsTree = true, IsCollide = false },
                TileType.Leaves => new InfoTile(tileType, 0.2f) { IsTree = true, IsCollide = false },
                TileType.OakBoard => new InfoTile(tileType, 0.78f, Item.ItemType.Axe),
                TileType.IronOre => new InfoTile(tileType, 2, Item.ItemType.Pickaxe),
                TileType.CoalOre => new InfoTile(tileType, 2, Item.ItemType.Pickaxe),
                TileType.Torch => new InfoTile(tileType, 0.2f, Item.ItemType.None) { IsCollide = false },
                TileType.Workbench => new InfoTile(tileType, 2, Item.ItemType.Axe) { IsCollide = false },
                _ => new InfoTile(tileType)
            };

        }
    }
}
