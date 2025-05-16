using SFML.Graphics;
using SFML.System;
using System;
using System.Reflection;
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

        public void SetToMesh(int x, int y, Vertex[] vertices)
        {
            if (x < 0 || y < 0 || x >= Chunk.ChunkSize || y >= Chunk.ChunkSize)
                return;

            int index = x * Chunk.ChunkSize * OneTileVerticesCount + y * OneTileVerticesCount;

            for (int i = 0; i < vertices.Length; i++)
                _mesh[index + i] = vertices[i];
        }

        public void UpdateViewMesh(int x, int y, Vertex[] vertices)
        {
            if (x < 0 || y < 0 || x >= Chunk.ChunkSize || y >= Chunk.ChunkSize)
                return;

            int index = x * Chunk.ChunkSize * OneTileVerticesCount + y * OneTileVerticesCount;

            for (int i = 0; i < vertices.Length; i++)
                _mesh[index + i].TexCoords = vertices[i].TexCoords;
        }

        public void UpdateTileColor(Color color, int x, int y)
        {
            if (x < 0 || y < 0 || x >= Chunk.ChunkSize || y >= Chunk.ChunkSize)
                return;

            int index = x * Chunk.ChunkSize * OneTileVerticesCount + y * OneTileVerticesCount;

            for(int i = 0; i < OneTileVerticesCount; i++)
                _mesh[index + i].Color = color;
        }

        public Color GetTileColor(int x, int y)
        {
            int index = x * Chunk.ChunkSize * OneTileVerticesCount + y * OneTileVerticesCount;

            if (index >= _mesh.Length || index < 0)
                return Color.Black;

            return _mesh[index + 0].Color;
        }

        public Vertex[] GetChunkMesh()
        {
            return _mesh;
        }
    }
}
