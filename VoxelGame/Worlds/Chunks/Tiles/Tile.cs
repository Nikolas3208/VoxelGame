using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoxelGame.Worlds.Chunks.Tiles
{
    public class Tile : Transformable
    {
        private TileInfo? TileInfo;
        private Chunk chunk;
        private Tile tile;

        public Tile(TileType type)
        {
            tile = this;

            TileInfo = new TileInfo(type);
        }

        public Tile(Tile tile)
        {
            this.tile = tile;
            TileInfo = tile.TileInfo;
        }

        public void SetChunk(Chunk chunk) => this.chunk = chunk;
        public Tile GetTile() => tile;
        public TileInfo GetTileInfo() => TileInfo!;
        public FloatRect GetFloatRect() => new FloatRect(tile.Position, tile.TileInfo!.Size);
    }
}
