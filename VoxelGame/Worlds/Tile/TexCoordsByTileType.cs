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
                TileType.Wood => (4 * 16, 16),
                TileType.Door => (16, 5 * 16),
                TileType.Leaves => (4 * 16, 3 * 16),
                TileType.IronOre => (1 * 16, 2 * 16),
                TileType.Board => (4 * 16, 0),
                _ => throw new NotImplementedException()
            };
        }
    }
}
