using SFML.Graphics;
using SFML.System;
using VoxelGame.Worlds.Tile;

namespace VoxelGame.Worlds
{
    public class ChunkMesh
    {
        private Chunk _chunk;
        private Vertex[] _mesh;

        public int OneTileVerticesCount { get; } = 6;


        public ChunkMesh(Chunk chunk)
        {
            _chunk = chunk;

            _mesh = new Vertex[Chunk.ChunkSize * Chunk.ChunkSize * OneTileVerticesCount];
        }

        public void SetTileToMesh(int x, int y, InfoTile tile, Color color)
        {
            if (x < 0 || y < 0 || x >= Chunk.ChunkSize || y >= Chunk.ChunkSize) 
                return;

            int index = x * Chunk.ChunkSize * OneTileVerticesCount + y * OneTileVerticesCount;

            if (tile != null && tile.Type != TileType.None)
            {
                var texCoords = TexCoordsByTileType.GetTexCoords(tile.Type);

                _mesh[index + 0] = new Vertex(new Vector2f(x * tile.Size.X, y * tile.Size.Y), color, new Vector2f(texCoords.Item1, texCoords.Item2));
                _mesh[index + 1] = new Vertex(new Vector2f(x * tile.Size.X, y * tile.Size.Y + tile.Size.Y), color, new Vector2f(texCoords.Item1, 16 + texCoords.Item2));
                _mesh[index + 2] = new Vertex(new Vector2f(x * tile.Size.X + tile.Size.X, y * tile.Size.Y), color, new Vector2f(16 + texCoords.Item1, texCoords.Item2));
                _mesh[index + 3] = new Vertex(new Vector2f(x * tile.Size.X + tile.Size.X, y * tile.Size.Y), color, new Vector2f(16 + texCoords.Item1, texCoords.Item2));
                _mesh[index + 4] = new Vertex(new Vector2f(x * tile.Size.X, y * tile.Size.Y + tile.Size.Y), color, new Vector2f(texCoords.Item1, 16 + texCoords.Item2));
                _mesh[index + 5] = new Vertex(new Vector2f(x * tile.Size.X + tile.Size.X, y * tile.Size.Y + tile.Size.Y), color, new Vector2f(16 + texCoords.Item1, 16 + texCoords.Item2));
            }
            else
            {
                _mesh[index + 0] = new Vertex();
                _mesh[index + 1] = new Vertex();
                _mesh[index + 2] = new Vertex();
                _mesh[index + 3] = new Vertex();
                _mesh[index + 4] = new Vertex();
                _mesh[index + 5] = new Vertex();
            }
        }

        public void UpdateTileColor(Color color, int x, int y)
        {
            if (x < 0 || y < 0 || x >= Chunk.ChunkSize || y >= Chunk.ChunkSize)
                return;

            int index = x * Chunk.ChunkSize * OneTileVerticesCount + y * OneTileVerticesCount;

            _mesh[index + 0].Color = color;
            _mesh[index + 1].Color = color;
            _mesh[index + 2].Color = color;
            _mesh[index + 3].Color = color;
            _mesh[index + 4].Color = color;
            _mesh[index + 5].Color = color;
        }

        public Color GetTileColor(int x, int y)
        {
            int index = x * Chunk.ChunkSize * OneTileVerticesCount + y * OneTileVerticesCount;

            if (index >= _mesh.Length)
                return Color.Black;

            return _mesh[index + 0].Color;
        }

        public Vertex[] GetChunkMesh()
        {
            return _mesh;
        }
    }
}
