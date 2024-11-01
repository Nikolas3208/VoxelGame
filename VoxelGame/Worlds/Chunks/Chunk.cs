using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoxelGame.Worlds.Chunks.Tiles;

namespace VoxelGame.Worlds.Chunks
{
    public class Chunk : Transformable, Drawable
    {
        private Mesh chunkMesh;
        private Tile[,] tiles;
        private World world;

        public Chunk(World world)
        {
            this.world = world;

            chunkMesh = new Mesh();
            tiles = new Tile[ChunkInfo.ChunckSize.X,ChunkInfo.ChunckSize.Y];
        }

        public void Init()
        {
            for (int x = 0; x < ChunkInfo.ChunckSize.X; x++)
            {
                for (int y = 0; y < ChunkInfo.ChunckSize.Y; y++)
                {
                    if (Random.Shared.Next(0, 4) == 1)
                        SetTile(TileType.GroundWall, x, y);
                    else
                        SetTile(TileType.Ground, x, y);
                }
            }
        }

        

        public void SetTileByMousePos(TileType type, Vector2i mousePos)
        {
            int x = (int)Math.Floor((float)(mousePos.X / ChunkInfo.ChunckSize.X));
            int y = (int)Math.Floor((float)(mousePos.Y / ChunkInfo.ChunckSize.Y));

            SetTile(type, x, y);
        }
        private int index = 0;

        public void SetTile(TileType type, int x, int y)
        {
            if (x < 0 || x >= ChunkInfo.ChunckSize.X || y < 0 || y >= ChunkInfo.ChunckSize.Y)
                return;

            if(type == TileType.None)
            {
                chunkMesh.SetVertex(tiles[x, y], x, y, destroy: true);
                tiles[x, y] = null;
                return;
            }

            Tile tile;

            if (GetTile(x, y)! == null)
                tile = new Tile(type);
            else if (GetTile(x, y)!.GetTile() == null)
                tile = new Tile(type);
            else
            {
                return;
            }

            tile.SetChunk(this);
            tile.Position = new Vector2f(x * TileInfo.MinTileSize, y * TileInfo.MinTileSize) + Position;

            if (tile.GetTileInfo().Size.X > TileInfo.MinTileSize || tile.GetTileInfo().Size.Y > TileInfo.MinTileSize
                && GetTile(x, y) == null)
            {
                

                int sizeX = (int)(tile.GetTileInfo().Size.X / TileInfo.MinTileSize);
                int sizeY = (int)(tile.GetTileInfo().Size.Y / TileInfo.MinTileSize);

                if (x % sizeX != 0 || y % sizeY != 0)
                {
                    return;
                }

                for (int x2 = 0; x2 < sizeX; x2++)
                {
                    for (int y2 = 0; y2 < sizeY; y2++)
                    {
                        if (sizeY == 1)
                            y2 = 0;
                        if(sizeX == 1)
                            x2 = 0;
                        if (x + x2 < ChunkInfo.ChunckSize.X && y + y2 < ChunkInfo.ChunckSize.Y && (x2 != 0 || y2 != 0))
                        {
                            if (GetTile(x + x2, y + y2) != null)
                                return;
                            tiles[x + x2, y + y2] = new Tile(tile);
                            tiles[x + x2, y + y2].Position = new Vector2f(x * tile.GetTileInfo().Size.X, y * tile.GetTileInfo().Size.Y) + Position;
                        }
                    }
                }
            }

            tile.GetTileInfo().Id = index;

            chunkMesh.SetVertex(tile, x * TileInfo.MinTileSize, y * TileInfo.MinTileSize);

            tiles[x, y] = tile;

            index += 6;
        }

        public Tile? GetTile(int x, int y)
        {
            if (x < 0 || y < 0 || x >= ChunkInfo.ChunckSize.X || y >= ChunkInfo.ChunckSize.Y)
                return null;

            return tiles[x, y];
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            states.Transform *= Transform;

            foreach(var vertices in chunkMesh.GetVetices()!)
            {
                states.Texture = TileInfo.GetTextureByTileType(vertices.Key);

                target.Draw(vertices.Value, PrimitiveType.Triangles, states);
            }
        }

        public FloatRect GetFloatRect() => new FloatRect(Position, ChunkInfo.ChunkSizeByPixel);
    }
}
