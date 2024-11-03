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
using VoxelGame.Worlds.Chunks.Tile;

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

        TileType tileType = TileType.None;
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

            if (Keyboard.IsKeyPressed(Keyboard.Key.Space))
            {
                velocity.Y = -245;
            }

            int x = Game.GetGlobalMousePosition().X / InfoTile.MinTileSize;
            int y = Game.GetGlobalMousePosition().Y / InfoTile.MinTileSize;

            var chunk = World.GetChunkByWorldPosition(x, y);

            DebugRender.AddRectangle(x * 16, y * 16, 16, 16, Color.Green);

            if(Keyboard.IsKeyPressed(Keyboard.Key.Num0))
            {
                tileType = TileType.None;
            }
            else if(Keyboard.IsKeyPressed(Keyboard.Key.Num1))
            {
                tileType = TileType.Ground;
            }
            else if (Keyboard.IsKeyPressed(Keyboard.Key.Num2))
            {
                tileType = TileType.Grass;
            }
            else if (Keyboard.IsKeyPressed(Keyboard.Key.Num3))
            {
                tileType = TileType.Stone;
            }
            else if (Keyboard.IsKeyPressed(Keyboard.Key.Num4))
            {
                tileType = TileType.Door;
            }


            if (Mouse.IsButtonPressed(Mouse.Button.Left))
            {
                if(chunk != null)
                {
                    chunk.SetTile(tileType, ((x * 16) - (int)chunk.Position.X) / 16, ((y * 16) - (int)chunk.Position.Y) / 16);
                }
            }
            if(Mouse.IsButtonPressed(Mouse.Button.Right))
            {
                if (chunk != null)
                {
                    var tile = chunk.GetTile(((x * 16) - (int)chunk.Position.X) / 16, ((y * 16) - (int)chunk.Position.Y) / 16);

                    if (tile != null)
                        tile.OnTileUse();
                }
            }

            Game.UpdateView(Position);

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
