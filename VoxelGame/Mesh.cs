using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoxelGame.Worlds.Chunks.Tiles;

namespace VoxelGame
{
    public class Mesh
    {
        private Dictionary<TileType, Vertex[]>? vertexArray;

        public Mesh()
        {
            vertexArray = new Dictionary<TileType, Vertex[]>();
        }

        public void SetVertex(Tile tile, int x, int y, bool destroy = false)
        {
            if (tile != null && !destroy)
            {
                var tileSize = tile.GetTileInfo().Size;
                int index = tile.GetTileInfo().Id;
                if (vertexArray!.ContainsKey(tile.GetTileInfo().Type))
                {
                    vertexArray[tile.GetTileInfo().Type][index + 0] = new Vertex(new Vector2f(x, y), new Vector2f(0, 0));
                    vertexArray[tile.GetTileInfo().Type][index + 1] = new Vertex(new Vector2f(tileSize.X + x, y), new Vector2f(tileSize.X, 0));
                    vertexArray[tile.GetTileInfo().Type][index + 2] = new Vertex(new Vector2f(x, tileSize.Y + y), new Vector2f(0, tileSize.Y));
                    vertexArray[tile.GetTileInfo().Type][index + 3] = new Vertex(new Vector2f(tileSize.X + x, tileSize.Y + y), new Vector2f(tileSize.X, tileSize.Y));
                    vertexArray[tile.GetTileInfo().Type][index + 4] = new Vertex(new Vector2f(tileSize.X + x, y), new Vector2f(tileSize.X, 0));
                    vertexArray[tile.GetTileInfo().Type][index + 5] = new Vertex(new Vector2f(x, tileSize.Y + y), new Vector2f(0, tileSize.Y));
                }
                else
                {
                    Vertex[] vertices = new Vertex[256 * 6];

                    vertices[index + 0] = new Vertex(new Vector2f(x, y), new Vector2f(0, 0));
                    vertices[index + 1] = new Vertex(new Vector2f(tileSize.X + x, y), new Vector2f(tileSize.X, 0));
                    vertices[index + 2] = new Vertex(new Vector2f(x, tileSize.Y + y), new Vector2f(0, tileSize.Y));
                    vertices[index + 3] = new Vertex(new Vector2f(tileSize.X + x, tileSize.Y + y), new Vector2f(tileSize.X, tileSize.Y));
                    vertices[index + 4] = new Vertex(new Vector2f(tileSize.X + x, y), new Vector2f(tileSize.X, 0));
                    vertices[index + 5] = new Vertex(new Vector2f(x, tileSize.Y + y), new Vector2f(0, tileSize.Y));

                    vertexArray!.Add(tile.GetTileInfo().Type, vertices);
                }
            }
            else if (tile != null && destroy)
            {
                int index = tile.GetTileInfo().Id;
                if (vertexArray!.ContainsKey(tile.GetTileInfo().Type))
                {
                    vertexArray[tile.GetTileInfo().Type][index + 0] = new Vertex();
                    vertexArray[tile.GetTileInfo().Type][index + 1] = new Vertex();
                    vertexArray[tile.GetTileInfo().Type][index + 2] = new Vertex();
                    vertexArray[tile.GetTileInfo().Type][index + 3] = new Vertex();
                    vertexArray[tile.GetTileInfo().Type][index + 4] = new Vertex();
                    vertexArray[tile.GetTileInfo().Type][index + 5] = new Vertex();
                }
            }
        }

        public Dictionary<TileType, Vertex[]>? GetVetices() => vertexArray!;
    }
}
