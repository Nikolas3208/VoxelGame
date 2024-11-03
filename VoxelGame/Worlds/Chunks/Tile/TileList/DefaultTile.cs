using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoxelGame.Worlds.Chunks.Tile;

namespace VoxelGame.Worlds.Chunks.Tile.TileList
{
    public class DefaultTile : InfoTile
    {
        public DefaultTile(Chunk chunk, TileType type) : base(chunk, type)
        {
        }

        public DefaultTile(Chunk chunk, InfoTile tile) : base(chunk, tile)
        {
        }

        public DefaultTile(Chunk chunk, TileType type, Vector2i tileSize) : base(chunk, type, tileSize)
        {
        }

        public override bool OnTileUse()
        {
            return base.OnTileUse();
        }
    }
}
