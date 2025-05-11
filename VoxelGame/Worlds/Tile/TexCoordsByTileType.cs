namespace VoxelGame.Worlds.Tile
{
    public static class TexCoordsByTileType
    {
        public static (int,int) GetTexCoords(TileType type)
        {
            return type switch
            {
                TileType.Ground => (2 * 16, 0),
                TileType.Grass => (3 * 16, 0),
                TileType.Stone => (16, 0),
                TileType.Oak => (4 * 16, 16),
                TileType.Birch => (5 * 16, 7 * 16),
                TileType.Door => (16, 5 * 16),
                TileType.Leaves => (4 * 16, 3 * 16),
                TileType.IronOre => (1 * 16, 2 * 16),
                TileType.OakBoard => (4 * 16, 0),
                TileType.BirchBoard => (6 * 16, 13 * 16),
                TileType.Chest => (11 * 16, 16),
                TileType.Workbench => (Random.Shared.Next(0, 2) == 1 ? 11 * 16 : 12 * 16, 3 * 16),
                TileType.Torch => (0, 5 * 16),
                _ => throw new NotImplementedException()
            };
        }
    }
}
