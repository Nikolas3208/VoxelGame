namespace VoxelGame.Worlds.Tile
{
    public static class TileByTypes
    {
        public static InfoTile GetTileByType(TileType type)
        {
            switch (type)
            {
                case TileType.None:
                    return null!;
                case TileType.Door:
                    return new DoorTile();
                default:
                    return new InfoTile(type);
            }
        }
    }
}
