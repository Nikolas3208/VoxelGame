using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoxelGame.Worlds.Chunks.Tile;
using VoxelGame.Worlds.Chunks.Tile.TileList;

namespace VoxelGame.Worlds.Chunks
{
    public class Chunk : Transformable, Drawable
    {
        private Mesh chunkMesh;
        private InfoTile[,] tiles;
        private InfoTile tile;
        private World world;

        private int vertexSize = 6;

        private float[] heightMap = new float[ChunkInfo.ChunckSize.X]; 
        public Chunk(World world)
        {
            this.world = world;

            chunkMesh = new Mesh();
            tiles = new InfoTile[ChunkInfo.ChunckSize.X,ChunkInfo.ChunckSize.Y];
        }

        public void GenChunk()
        {
            for (int x = 0; x < ChunkInfo.ChunckSize.X; x++)
            {
                float p = World.GetPerlin().Noise((x + (Position.X / InfoTile.MinTileSize)) * 0.043f, (x + (Position.X / InfoTile.MinTileSize)) * 0.0237f);

                float p1 = (World.GetPerlin().Noise(0.24f / (ChunkInfo.ChunckSize.X * 0.24f), ((x + (Position.X / InfoTile.MinTileSize)) * 0.24f) / (ChunkInfo.ChunckSize.X * 0.24f), 2) * 0.5f + 0.5f) * 21;
                float p2 = (World.GetPerlin().Noise(0.02f / (ChunkInfo.ChunckSize.X * 0.02f), ((x + (Position.X / InfoTile.MinTileSize)) * 0.02f) / (ChunkInfo.ChunckSize.X * 0.02f), 4) * 0.5f + 0.5f) * 0.6f;

                if (p > 0)
                    p2 *= p;

                heightMap[x] = p1 + p2;
            }

            if (Position.Y <= 512)
            {
                for (int x = 0; x < ChunkInfo.ChunckSize.X; x++)
                {
                    for (int y = (int)heightMap[x]; y < ChunkInfo.ChunckSize.Y; y++)
                    {
                        if (y >= 0)
                        {
                            SetTile(TileType.Ground, x, y);
                        }
                    }
                }
            }
            else
            {
                for (int x = 0; x < ChunkInfo.ChunckSize.X; x++)
                {
                    for (int y = 0; y < ChunkInfo.ChunckSize.Y; y++)
                    {
                        if (y >= 0)
                        {
                            SetTile(TileType.Ground, x, y);
                        }
                    }
                }
            }
        }

        public bool UpdateTextureCoordInTile(InfoTile tile, int x, int y)
        {
            return chunkMesh.UpdateTextureCoord(tile, x, y);
        }

        public bool UpdatePositionCoordTile(InfoTile tile, Vector2f pos, bool leftOrRight = false)
        {
            return chunkMesh.UpdatePositionCoord(tile, pos, leftOrRight);
        }

        public void SetTileByMousePos(TileType type, Vector2i mousePos)
        {
            int x = (int)Math.Floor((float)(mousePos.X / ChunkInfo.ChunckSize.X));
            int y = (int)Math.Floor((float)(mousePos.Y / ChunkInfo.ChunckSize.Y));

            SetTile(type, x, y);
        }

        public void SetTile(TileType type, int x, int y)
        {
            if (x < 0 || x >= ChunkInfo.ChunckSize.X || y < 0 || y >= ChunkInfo.ChunckSize.Y)
                return;

            if(type == TileType.None)
            {
                chunkMesh.SetVertex(ref tiles[x, y], x, y, destroy: true);
                tiles[x, y] = null;
                return;
            }
            else if (type == TileType.Door)
            {
                if (GetTile(x, y + 1) != null && GetTile(x, y) == null)
                {
                    for (int y2 = 0; y2 < 3; y2++)
                    {
                        if (GetTile(x, y - y2) != null)
                            return;
                    }
                    if (GetTile(x, y - 3) == null)
                        return;

                    tile = new DoorTile(this, type, new Vector2i(6, 48));
                    tile.TileId = (x * 96) + (y * 6);
                    tile.Position = new Vector2f(x * InfoTile.MinTileSize, (y - 2) * InfoTile.MinTileSize) + Position;

                    chunkMesh.SetVertex(ref tile, (x * 16), (y - 2) * 16);
                    tiles[x, y - 2] = tile;

                    for (int y2 = 0; y2 < 3; y2++)
                    {
                        tiles[x, y - y2] = tile;
                    }

                    return;
                }

                return;
            }

            if (GetTile(x, y) == null)
                tile = Tiles.TileByTileType(this, type);
            else if (GetTile(x, y)!.GetPerentTile() == null)
                tile = Tiles.TileByTileType(this, type);
            else
                return;

            tile.SetPerentChunk(this);
            tile.Position = new Vector2f(x * InfoTile.MinTileSize, y * InfoTile.MinTileSize) + Position;

            if (tile.TileSize.X > InfoTile.MinTileSize || tile.TileSize.Y > InfoTile.MinTileSize
                && GetTile(x, y) == null)
            {
                

                int sizeX = (int)(tile.TileSize.X / InfoTile.MinTileSize);
                int sizeY = (int)(tile.TileSize.Y / InfoTile.MinTileSize);

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
                            if (GetTile(x + x2, y + y2) == null)
                            {
                                tiles[x + x2, y + y2] = tile;
                                //tiles[x + x2, y + y2].Position = new Vector2f(x * tile.TileSize.X, y * tile.TileSize.Y) + Position;
                            }
                            else
                            {
                                tile = null;
                                return;
                            }
                        }
                    }
                }
            }
            tile.TileId = (x * ChunkInfo.ChunckSize.Y * vertexSize) + (y * vertexSize);

            chunkMesh.SetVertex(ref tile, x * InfoTile.MinTileSize, y * InfoTile.MinTileSize);

            tiles[x, y] = tile;
        }

        public InfoTile? GetTile(int x, int y)
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
                states.Texture = ContentManager.GetTextureByTileType(vertices.Key);

                target.Draw(vertices.Value, PrimitiveType.Triangles, states);
            }
        }

        public FloatRect GetFloatRect() => new FloatRect(Position, ChunkInfo.ChunkSizeByPixel);
    }
}
