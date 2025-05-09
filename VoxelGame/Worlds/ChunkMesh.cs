using SFML.Graphics;
using SFML.System;
using VoxelGame.Worlds.Tile;

namespace VoxelGame.Worlds
{
    public class ChunkMesh
    {
        private Chunk _chunk;

        public int OneTileVerticesCount { get; } = 6;

        public Vertex[] Mesh;
        public InfoTile[] Tiles;

        public ChunkMesh(Chunk chunk)
        {
            _chunk = chunk;

            Mesh = new Vertex[Chunk.ChunkSize * Chunk.ChunkSize * OneTileVerticesCount];
            Tiles = new InfoTile[Chunk.ChunkSize * Chunk.ChunkSize];
        }

        public void SetTileToMesh(int x, int y, InfoTile tile, Color color)
        {
            if (x < 0 || y < 0 || x >= Chunk.ChunkSize || y >= Chunk.ChunkSize) 
                return;

            int index = x * Chunk.ChunkSize * OneTileVerticesCount + y * OneTileVerticesCount;

            color = tile?.Type == TileType.Leaves ? Color.Green : color;
            color = tile?.Type == TileType.Wood ? Color.White: color;

            if (tile != null && tile.Type != TileType.None)
            {
                var texCoords = TexCoordsByTileType.GetTexCoords(tile.Type);

                Mesh[index + 0] = new Vertex(new Vector2f(x * tile.Size.X, y * tile.Size.Y), color, new Vector2f(texCoords.Item1, texCoords.Item2));
                Mesh[index + 1] = new Vertex(new Vector2f(x * tile.Size.X, y * tile.Size.Y + tile.Size.Y), color, new Vector2f(texCoords.Item1, 16 + texCoords.Item2));
                Mesh[index + 2] = new Vertex(new Vector2f(x * tile.Size.X + tile.Size.X, y * tile.Size.Y), color, new Vector2f(16 + texCoords.Item1, texCoords.Item2));
                Mesh[index + 3] = new Vertex(new Vector2f(x * tile.Size.X + tile.Size.X, y * tile.Size.Y), color, new Vector2f(16 + texCoords.Item1, texCoords.Item2));
                Mesh[index + 4] = new Vertex(new Vector2f(x * tile.Size.X, y * tile.Size.Y + tile.Size.Y), color, new Vector2f(texCoords.Item1, 16 + texCoords.Item2));
                Mesh[index + 5] = new Vertex(new Vector2f(x * tile.Size.X + tile.Size.X, y * tile.Size.Y + tile.Size.Y), color, new Vector2f(16 + texCoords.Item1, 16 + texCoords.Item2));
            }
            else
            {

                Mesh[index + 0] = new Vertex();
                Mesh[index + 1] = new Vertex();
                Mesh[index + 2] = new Vertex();
                Mesh[index + 3] = new Vertex();
                Mesh[index + 4] = new Vertex();
                Mesh[index + 5] = new Vertex();
            }
        }

        public void UpdateTileColor(Color color, int x, int y)
        {
            if (x < 0 || y < 0 || x >= Chunk.ChunkSize || y >= Chunk.ChunkSize)
                return;

            int index = x * Chunk.ChunkSize * OneTileVerticesCount + y * OneTileVerticesCount;

            Mesh[index + 0].Color = color;
            Mesh[index + 1].Color = color;
            Mesh[index + 2].Color = color;
            Mesh[index + 3].Color = color;
            Mesh[index + 4].Color = color;
            Mesh[index + 5].Color = color;
        }

        public Color GetTileColor(int x, int y)
        {
            int index = x * Chunk.ChunkSize * OneTileVerticesCount + y * OneTileVerticesCount;

            if (index >= Mesh.Length)
                return Color.Black;

            return Mesh[index + 0].Color;
        }

        public Vertex[] GetChunkMesh()
        {
            return Mesh;
        }
    }
}
