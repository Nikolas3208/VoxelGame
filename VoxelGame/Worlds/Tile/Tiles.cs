using SFML.System;
using VoxelGame.Item;

namespace VoxelGame.Worlds.Tile
{
    public static class Tiles
    {
        public static Tile GetTile(TileType tileType, Chunk perentChunk, Tile? upTile, Tile? downTile, Tile? leftTile, Tile? rightTile, Vector2f localPosition)
        {
            return tileType switch
            {
                TileType.Ground => new Tile(tileType, ItemList.Ground, ItemType.Pickaxe, 1, 2, true, perentChunk, upTile, downTile, leftTile, rightTile, localPosition),
                TileType.Grass => new Tile(tileType, ItemList.Ground, ItemType.Pickaxe, 1, 2, true, perentChunk, upTile, downTile, leftTile, rightTile, localPosition),
                TileType.Stone => new Tile(tileType, ItemList.Stone, ItemType.Pickaxe, 1, 3, true, perentChunk, upTile, downTile, leftTile, rightTile, localPosition),
                TileType.Oak => new TileTree(tileType, ItemList.OakBoard, ItemType.Axe, 1, 2, false, perentChunk, upTile, downTile, leftTile, rightTile, localPosition) { IsTree = true },
                TileType.Leaves => new TileTree(tileType, ItemList.OakBoard, ItemType.Axe, 0, 1, false, perentChunk, upTile, downTile, leftTile, rightTile, localPosition, true),
                TileType.OakBoard => new Tile(tileType, ItemList.OakBoard, ItemType.Pickaxe, 1, 2, true, perentChunk, upTile, downTile, leftTile, rightTile, localPosition),
                TileType.IronOre => new Tile(tileType, ItemList.IronOre, ItemType.Pickaxe, 2, 5, true, perentChunk, upTile, downTile, leftTile, rightTile, localPosition),
                TileType.Torch => new Tile(tileType, ItemList.Torch, ItemType.All, 0, 1, false, perentChunk, upTile, downTile, leftTile, rightTile, localPosition),
                TileType.Workbench => new TileWorkbench(tileType, ItemList.Workbench, ItemType.Axe, 1, 3, false, perentChunk, upTile, downTile, leftTile, rightTile, localPosition),
                TileType.Stove => new Tile(tileType, ItemList.Stove, ItemType.Pickaxe, 1, 3, false, perentChunk, upTile, downTile, leftTile, rightTile, localPosition),
                _ => throw new Exception($"Tile type is not found. {tileType}")
            };

        }

        public static WallTile GetWall(WallType wallType, Chunk perentChunk, WallTile? upTile, WallTile? downTile, WallTile? leftTile, WallTile? rightTile, Vector2f localPosition)
        {
            return wallType switch
            {
                WallType.GroundWall => new WallTile(wallType, ItemType.Pickaxe, 1, 2, false, perentChunk, upTile, downTile, leftTile, rightTile, localPosition),
                WallType.StoneWall => new WallTile(wallType, ItemType.Pickaxe, 2, 4, false, perentChunk, upTile, downTile, leftTile, rightTile, localPosition),
                WallType.OakBoardWall => new WallTile(wallType, ItemType.Pickaxe, 2, 3, false, perentChunk, upTile, downTile, leftTile, rightTile, localPosition),
                WallType.BirchBoardWall => new WallTile(wallType, ItemType.Pickaxe, 2, 3, false, perentChunk, upTile, downTile, leftTile, rightTile, localPosition),
                _ => throw new Exception($"Wall type is'n found. {wallType}") 
            };
        }

        public static TileType ItemListToTileType(ItemList itemList)
        {
            return Enum.Parse<TileType>(itemList.ToString());
        }

        public static WallType TileTypeToWallType(TileType type)
        {
            return type switch
            {
                TileType.Ground => WallType.GroundWall,
                TileType.Grass => WallType.GroundWall,
                TileType.Stone => WallType.StoneWall,
                TileType.OakBoard => WallType.OakBoardWall,
                TileType.BirchBoard => WallType.BirchBoardWall,
                _ => WallType.GroundWall
            };
        }

        public static WallType ItemListToWallType(ItemList itemList)
        {
            return Enum.Parse<WallType>(itemList.ToString());
        }
    }
}
