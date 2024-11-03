using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using VoxelGame.Worlds.Chunks;
using VoxelGame.Worlds.Chunks.Tile;

namespace VoxelGame
{
    public class Mesh
    {
        private Dictionary<TileType, Vertex[]>? vertexArray;

        public Mesh()
        {
            vertexArray = new Dictionary<TileType, Vertex[]>();
        }
        public void SetVertex(ref InfoTile tile, int x, int y, bool destroy = false)
        {
            if (tile != null && !destroy)
            {
                int index = tile.TileId;
                var tileSize = tile.TileSize;
                var tileType = tile.Type;

                int x2 = (int)tile.TextureCoord.X;
                int y2 = (int)tile.TextureCoord.Y;

                if (vertexArray!.ContainsKey(tileType) && index < (ChunkInfo.ChunckSize.X * ChunkInfo.ChunckSize.Y * 6) - 5)
                {
                    vertexArray[tileType][index + 0] = new Vertex(new Vector2f(x, y), new Vector2f(x2, y2));
                    vertexArray[tileType][index + 1] = new Vertex(new Vector2f(tileSize.X + x, y), new Vector2f(tileSize.X + x2, y2));
                    vertexArray[tileType][index + 2] = new Vertex(new Vector2f(x, tileSize.Y + y), new Vector2f(x2, tileSize.Y + y2));
                    vertexArray[tileType][index + 3] = (new Vertex(new Vector2f(tileSize.X + x, tileSize.Y + y), new Vector2f(tileSize.X + x2, tileSize.Y + y2)));
                    vertexArray[tileType][index + 4] = (new Vertex(new Vector2f(tileSize.X + x, y), new Vector2f(tileSize.X + x2, y2)));
                    vertexArray[tileType][index + 5] = (new Vertex(new Vector2f(x, tileSize.Y + y), new Vector2f(x2, tileSize.Y + y2)));
                }
                else if(index < (ChunkInfo.ChunckSize.X * ChunkInfo.ChunckSize.Y * 6) - 5)
                {
                    Vertex[] vertices = new Vertex[ChunkInfo.ChunckSize.X * ChunkInfo.ChunckSize.Y * 6];

                    vertices[index + 0] = new Vertex(new Vector2f(x, y), new Vector2f(x2, x2));
                    vertices[index + 1] = new Vertex(new Vector2f(tileSize.X + x, y), new Vector2f(tileSize.X + x2, y2));
                    vertices[index + 2] = new Vertex(new Vector2f(x, tileSize.Y + y), new Vector2f(x2, tileSize.Y + y2));
                    vertices[index + 3] = new Vertex(new Vector2f(tileSize.X + x, tileSize.Y + y), new Vector2f(tileSize.X + x2, tileSize.Y + y2));
                    vertices[index + 4] = new Vertex(new Vector2f(tileSize.X + x, y), new Vector2f(tileSize.X + x2, y2));
                    vertices[index + 5] = new Vertex(new Vector2f(x, tileSize.Y + y), new Vector2f(x2, tileSize.Y + y2));

                    vertexArray!.Add(tileType, vertices);
                }
            }
            else if (tile != null && destroy)
            {
                int index = tile.TileId;
                if (vertexArray!.ContainsKey(tile.Type))
                {
                    if (index < vertexArray[tile.Type].Length - 5)
                    {
                        vertexArray[tile.Type][index + 0] = new Vertex();
                        vertexArray[tile.Type][index + 1] = new Vertex();
                        vertexArray[tile.Type][index + 2] = new Vertex();
                        vertexArray[tile.Type][index + 3] = new Vertex();
                        vertexArray[tile.Type][index + 4] = new Vertex();
                        vertexArray[tile.Type][index + 5] = new Vertex();
                    }
                }
            }
        }

        public Dictionary<TileType, Vertex[]>? GetVetices() => vertexArray!;

        public bool UpdateTextureCoord(InfoTile tile, int x, int y)
        {
            //UpdatePositionCoord(tile, (int)tile.Position.X, (int)tile.Position.Y);
            if (vertexArray!.ContainsKey(tile.Type))
            {
                vertexArray[tile.Type][tile.TileId + 0].TexCoords = new Vector2f(x, y);
                vertexArray[tile.Type][tile.TileId + 1].TexCoords = new Vector2f(x + tile.TileSize.X, y);
                vertexArray[tile.Type][tile.TileId + 2].TexCoords = new Vector2f(x, y + tile.TileSize.Y);
                vertexArray[tile.Type][tile.TileId + 3].TexCoords = new Vector2f(x + tile.TileSize.X, y + tile.TileSize.Y);
                vertexArray[tile.Type][tile.TileId + 4].TexCoords = new Vector2f(x + tile.TileSize.X, y);
                vertexArray[tile.Type][tile.TileId + 5].TexCoords = new Vector2f(x, y + tile.TileSize.Y);

                return true;
            }

            return false;
        }

        public bool UpdatePositionCoord(InfoTile tile, Vector2f pos, bool LeftOrRight = false)
        {
            if (vertexArray!.ContainsKey(tile.Type))
            {
                float x = pos.X;
                float y = pos.Y;

                if(!LeftOrRight)
                {
                    vertexArray[tile.Type][tile.TileId + 0].Position = new Vector2f(x, y);
                    vertexArray[tile.Type][tile.TileId + 1].Position = new Vector2f(x + tile.TileSize.X, y);
                    vertexArray[tile.Type][tile.TileId + 2].Position = new Vector2f(x, y + tile.TileSize.Y);
                    vertexArray[tile.Type][tile.TileId + 3].Position = new Vector2f(x + tile.TileSize.X, y + tile.TileSize.Y);
                    vertexArray[tile.Type][tile.TileId + 4].Position = new Vector2f(x + tile.TileSize.X, y);
                    vertexArray[tile.Type][tile.TileId + 5].Position = new Vector2f(x, y + tile.TileSize.Y);
                }
                else
                {
                    vertexArray[tile.Type][tile.TileId + 0].Position = new Vector2f(x, y);
                    vertexArray[tile.Type][tile.TileId + 1].Position = new Vector2f(x - tile.TileSize.X, y);
                    vertexArray[tile.Type][tile.TileId + 2].Position = new Vector2f(x, y + tile.TileSize.Y);
                    vertexArray[tile.Type][tile.TileId + 3].Position = new Vector2f(x - tile.TileSize.X, y + tile.TileSize.Y);
                    vertexArray[tile.Type][tile.TileId + 4].Position = new Vector2f(x - tile.TileSize.X, y);
                    vertexArray[tile.Type][tile.TileId + 5].Position = new Vector2f(x, y + tile.TileSize.Y);
                }

                return true;
            }

            return false;
        }
    }
}
