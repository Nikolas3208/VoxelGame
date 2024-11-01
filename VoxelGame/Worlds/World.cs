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
using VoxelGame.Worlds.Chunks.Tiles;

namespace VoxelGame.Worlds
{
    public class World : Transformable, Drawable
    {
        private List<Entity>? entities;
        private Chunk[,]? chunks;
        private DebugRender? debugRender;

        public const int WorldWidth = 16;
        public const int WorldHeight = 16;

        public World()
        {
            Init();
        }

        public void Init()
        {
            entities = new List<Entity>();
            entities.Add(new Player(this));
            chunks = new Chunk[WorldWidth, WorldHeight];
            debugRender = new DebugRender();
            for (int x = 0; x < WorldWidth; x++)
            {
                for (int y = 2; y < WorldHeight; y++)
                {
                    SetChunk(x, y);
                }
            }
        }

        public void SetChunk(int x, int y)
        {
            if (x < 0 || y < 0 || x >= WorldWidth || y >= WorldHeight)
                return;

            var chunk = new Chunk(this);
            chunk.Position = new Vector2f(x * ChunkInfo.ChunckSize.X, y * ChunkInfo.ChunckSize.Y) * TileInfo.MinTileSize + Position;
            chunk.Init();

            chunks![x, y] = chunk;
        }

        public Chunk? GetChunk(int x, int y)
        {
            if(x < 0 || y < 0 || x >= WorldWidth || y >= WorldHeight) 
                return null;

            return chunks![x, y];
        }

        public Tile? GetTileByWorldPosition(Vector2f position)
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

        public Tile? GetTile(Chunk chunk, int x, int y)
        {
            if (chunk != null)
                return chunk.GetTile(((x * 16) - (int)chunk.Position.X) / 16, ((y * 16) - (int)chunk.Position.Y) / 16);

            return null;
        }

        public void Update(RenderWindow window, float deltaTime)
        {
            foreach (var entity in entities!)
            {
                entity.Update(deltaTime);
            }
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            states.Transform *= Transform;

            for (int x = 0; x < WorldWidth; x++)
            {
                for (int y = 0; y < WorldWidth; y++)
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
    }
}
