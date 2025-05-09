using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoxelGame.Worlds.Tile
{
    public static class Tiles
    {
        public static InfoTile GetTile(TileType tileType, bool isWall = false)
        {
            if (!isWall)
            {
                return tileType switch
                {
                    TileType.Ground => new InfoTile(tileType, 0.5f),
                    TileType.Grass => new InfoTile(tileType, 0.5f),
                    TileType.Stone => new InfoTile(tileType, 1.5f),
                    TileType.Wood => new InfoTile(tileType, 1),
                    TileType.Leaves => new InfoTile(tileType, 0.2f),
                    TileType.Board => new InfoTile(tileType, 0.78f),
                    TileType.IronOre => new InfoTile(tileType, 2),
                    _ => new InfoTile(tileType)
                };
            }
            else
            {
                return tileType switch
                {
                    TileType.Ground => new Wall(TileType.Ground),
                    TileType.Stone => new Wall(TileType.Stone),
                    TileType.Board => new Wall(TileType.Board),
                    _ => new Wall(tileType)
                };
            }
        }
    }
}
