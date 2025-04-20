using VoxelGame.Worlds;
using VoxelGame.Worlds.Tile;

namespace VoxelGame.Item
{
    public static class ItemTileByTile
    {
        public static InfoItem GetInfoItemByTile(InfoTile tile)
        {
            return tile.Type switch
            {
                TileType.Ground => new TileItem("Ground", 2),
                TileType.Grass => new TileItem("Grass", 3),
                TileType.Stone => new TileItem("Stone", 1),
                TileType.Wood => new TileItem("Wood", 20),
                TileType.Leaves => new TileItem("Leaves", 53),
                TileType.IronOre => new TileItem("Iron Ore", 33),
                _ => new TileItem("Unknown", 0),
            };
        }
    }
}
