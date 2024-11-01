using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoxelGame.Entitys.Physics;
using VoxelGame.Worlds;
using VoxelGame.Worlds.Chunks;

namespace VoxelGame.Entitys
{
    public class Player : Entity, Drawable
    {
        private float maxSpeed = 120;
        private float speed = 40;
        public Player(World world) : base(world)
        {
            rect = new RectangleShape(new Vector2f(32, 48));
            rect.Texture = new Texture("Contents\\Textures\\Items\\Item_0.png");
            rect.Origin = new Vector2f(rect.Size.X / 2, 0);
        }

        public override void Update(float deltaTime)
        {
            if (Keyboard.IsKeyPressed(Keyboard.Key.A))
            {
                rect!.Scale = new Vector2f(-1, 1);
                if (velocity.X > -maxSpeed)
                    velocity.X += -speed;
            }
            else if (Keyboard.IsKeyPressed(Keyboard.Key.D))
            {
                rect!.Scale = new Vector2f(1, 1);
                if (velocity.X < maxSpeed)
                    velocity.X += speed;
            }
            else
                velocity.X = 0;
            int x = Game.GetMousePosition().X / (ChunkInfo.ChunckSize.X * ChunkInfo.ChunckSize.Y);
            int y = Game.GetMousePosition().Y / (ChunkInfo.ChunckSize.X * ChunkInfo.ChunckSize.Y);

            var chunk = World.GetChunk(x, y);

            if (Mouse.IsButtonPressed(Mouse.Button.Right))
            {
                if(chunk != null)
                {
                    x = Mouse.GetPosition().X / ChunkInfo.ChunckSize.X;
                    y = Mouse.GetPosition().Y / ChunkInfo.ChunckSize.Y - 2;

                    chunk.SetTile(Worlds.Chunks.Tiles.TileType.Ground, ((x * 16) - (int)chunk.Position.X) / 16, ((y * 16) - (int)chunk.Position.Y) / 16);
                }
            }
            if(Mouse.IsButtonPressed(Mouse.Button.Left))
            {
                if (chunk != null)
                {
                    x = Mouse.GetPosition().X / ChunkInfo.ChunckSize.X;
                    y = Mouse.GetPosition().Y / ChunkInfo.ChunckSize.Y;

                    chunk.SetTile(Worlds.Chunks.Tiles.TileType.None, ((x * 16) - (int)chunk.Position.X) / 16, ((y * 16) - (int)chunk.Position.Y) / 16);
                }
            }

            base.Update(deltaTime);
        }

        public override void Draw(RenderTarget target, RenderStates states)
        {
            states.Transform *= Transform;

            if(rect != null)
                rect.Draw(target, states);
        }
    }
}
