using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoxelGame.Item;

namespace VoxelGame.Worlds.Chunks.Tile.TileList
{
    public class DoorTile : InfoTile
    {
        private bool doorIsOpen = false;
        public DoorTile(Chunk chunk, TileType type, Vector2i tileSize) : base(chunk, type, tileSize)
        {
            Origin = new Vector2f(TileSize.X / 2, TileSize.Y);
        }

        public override bool OnTileUse()
        {
            int x = (int)GetPositionByChunk().X / MinTileSize;
            int y = (int)GetPositionByChunk().X / MinTileSize;
            if (!doorIsOpen)
            {
                for (int x2 = 1; x2 < 3; x2++)
                    for (int y2 = 0; y2 < 3; y2++)
                    {
                        if (perentChunk.GetTile(x - x2, y - y2) != null)
                            return false;
                    }
                TileSize = new Vector2i(32, 48);
                Origin = new Vector2f(TileSize.X / 2, TileSize.Y);
                perentChunk.UpdatePositionCoordTile(this, GetPositionByChunk(), true);
                perentChunk.UpdateTextureCoordInTile(this, 39, 0);
                doorIsOpen = true;
                IsWall = true;
                return true;
            }
            else if (doorIsOpen)
            {
                TileSize = new Vector2i(6, 48);
                Origin = new Vector2f(TileSize.X / 2, TileSize.Y);
                perentChunk.UpdatePositionCoordTile(this, GetPositionByChunk(), true);
                perentChunk.UpdateTextureCoordInTile(this, 33, 0);
                doorIsOpen = false;
                IsWall = false;
                return true;
            }

            return false;
        }
    }
}
