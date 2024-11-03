using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoxelGame.Entitys;
using VoxelGame.Worlds.Chunks;
using VoxelGame.Worlds.Chunks.Tile;

namespace VoxelGame.Worlds
{
    public class World : Transformable, Drawable
    {
        private List<Entity>? entities;
        private Chunk[,]? chunks;
        private DebugRender? debugRender;
        private static Perlin2d perlin;

        public const int WorldWidth = 10;
        public const int WorldHeight = 4;

        public World()
        {
            Init();
        }

        public void Init()
        {
            float[] heightMap = new float[WorldWidth * ChunkInfo.ChunckSize.X];

            perlin = new Perlin2d();
            entities = new List<Entity>();
            entities.Add(new Player(this));
            chunks = new Chunk[WorldWidth, WorldHeight];
            debugRender = new DebugRender();
            for (int x = 0; x < WorldWidth; x++)
            {
                for (int y = 2; y < WorldHeight; y++)
                {
                    SetChunk(x, y);
                    GetChunk(x, y)!.GenChunk();
                }
            }
        }

        public float[] Interpolate(float[] map, float octaves = 1)
        {
            float[] interpolateMap = new float[map.Length];

            for (int i = 0; i < octaves; i++)
            {
                for (int x = 1; x < WorldWidth * InfoTile.MinTileSize - 1; x++)
                {
                    interpolateMap[x] = (map[x - 1] + map[x + 1] + map[x]) / 3;
                }
            }

            return interpolateMap;
        }

        public void SetChunk(int x, int y)
        {
            if (x < 0 || y < 0 || x >= WorldWidth || y >= WorldHeight)
                return;

            var chunk = new Chunk(this);
            chunk.Position = new Vector2f(x * ChunkInfo.ChunckSize.X, y * ChunkInfo.ChunckSize.Y) * InfoTile.MinTileSize + Position;

            chunks![x, y] = chunk;
        }

        public void SetChunkByWorldPosition(float x, float y)
        {
            if (x < 0 || y < 0 || x >= WorldWidth * ChunkInfo.ChunckSize.X || y >= WorldHeight * ChunkInfo.ChunckSize.Y)
                return;

            int x2 = (int)MathF.Floor(x / ChunkInfo.ChunckSize.X);
            int y2 = (int)MathF.Floor(y / ChunkInfo.ChunckSize.X);

            var chunk = new Chunk(this);

            chunk.Position = new Vector2f(x2 * ChunkInfo.ChunckSize.X, y2 * ChunkInfo.ChunckSize.Y) * InfoTile.MinTileSize + Position;

            chunks![x2, y2] = chunk;
        }

        public void SetTileByChunk(Chunk chunk, TileType type, int x, int y)
        {
            if (chunk == null)
                return;

            chunk.SetTile(type, ((x * InfoTile.MinTileSize) - (int)chunk.Position.X) / 16, ((y * InfoTile.MinTileSize) - (int)chunk.Position.Y) / 16);
        }

        public Chunk? GetChunk(int x, int y)
        {
            if(x < 0 || y < 0 || x >= WorldWidth || y >= WorldHeight) 
                return null;

            return chunks![x, y];
        }

        public Chunk? GetChunkByWorldPosition(float x, float y)
        {
            int x2 = (int)MathF.Floor(x / ChunkInfo.ChunckSize.X);
            int y2 = (int)MathF.Floor(y / ChunkInfo.ChunckSize.X);

            return GetChunk(x2, y2);
        }

        public InfoTile? GetTileByWorldPosition(float x, float y)
        {
            return GetTileByWorldPosition(new Vector2f(x, y));
        }

        public InfoTile? GetTileByWorldPosition(Vector2f position)
        {
            int x = (int)MathF.Floor(position.X / ChunkInfo.ChunckSize.X);
            int y = (int)MathF.Floor(position.Y / ChunkInfo.ChunckSize.X);

            var chunk = GetChunk(x, y);
            if (chunk != null)
            {
                x = (int)position.X;
                y = (int)position.Y;

                return GetTile(chunk, x, y);
            }

            return null;
        }

        public InfoTile? GetTile(Chunk chunk, int x, int y)
        {
            if (chunk != null)
                return chunk.GetTile(((x * InfoTile.MinTileSize) - (int)chunk.Position.X) / 16, ((y * InfoTile.MinTileSize) - (int)chunk.Position.Y) / 16);

            return null;
        }

        public void Update(RenderWindow window, float deltaTime)
        {
            if(Keyboard.IsKeyPressed(Keyboard.Key.R))
            {
                Init();
            }

            foreach (var entity in entities!)
            {
                entity.Update(deltaTime);
            }
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            states.Transform *= Transform;

            var chunkPos = new Vector2i((int)(Game.ViewPosition.X / ChunkInfo.ChunkSizeByPixel.X), (int)(Game.ViewPosition.Y / ChunkInfo.ChunkSizeByPixel.Y));
            var chunkPerScreen = new Vector2i(Game.WindowSize.X / (int)ChunkInfo.ChunkSizeByPixel.X, Game.WindowSize.Y / (int)ChunkInfo.ChunkSizeByPixel.Y);
            var LeftMostChunkPos = chunkPos.X - chunkPerScreen.X / 2;
            var TopMostChunkPos = chunkPos.Y - chunkPerScreen.Y / 2;

            for (int x = LeftMostChunkPos - 1; x < LeftMostChunkPos + chunkPerScreen.X + 1; x++)
            {
                for (int y = TopMostChunkPos - 1; y < TopMostChunkPos + chunkPerScreen.Y + 1; y++)
                {
                    var chunk = GetChunk(x, y);
                    if (chunk != null)
                    {
                        target.Draw(chunk, states);
                        DebugRender.AddRectangle(chunk.GetFloatRect(), Color.Red);
                    }
                }
            }

            foreach (var entity in entities!)
            {
                entity.Draw(target, states);
            }

            debugRender!.Draw(target, states);
        }

        public static Perlin2d GetPerlin() => perlin;
    }
}
