using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoxelGame.Worlds.Chunks.Tile.TileList;

namespace VoxelGame.Worlds.Chunks.Tile
{
    public class Tiles
    {
        public static InfoTile TileByTileType(Chunk chunk, TileType type)
        {
            switch(type)
            {
                default:
                    return new DefaultTile(chunk, type);
                case TileType.Door:
                    return new DoorTile(chunk, type, new Vector2i(6, InfoTile.MinTileSize * 3));
            }
        }
    }
}
