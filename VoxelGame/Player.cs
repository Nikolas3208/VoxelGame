using SFML.Graphics;
using SFML.System;
using SFML.Window;
using VoxelGame.Npc;
using VoxelGame.Physics;
using VoxelGame.Resources;
using VoxelGame.Worlds;
using VoxelGame.Worlds.Tile;

namespace VoxelGame
{
    public class Player : Entity
    {
        private float maxSpeed = 100;
        public Player(RigidBody rigidBody, World world) : base(rigidBody, world)
        {
            shape.FillColor = Color.Red;
        }

        public override void Update(float deltaTime)
        {
            MouseUpdate();

            bool isRightMove = Keyboard.IsKeyPressed(Keyboard.Key.D);
            bool isLeftMove = Keyboard.IsKeyPressed(Keyboard.Key.A);
            bool isJump = Keyboard.IsKeyPressed(Keyboard.Key.Space);

            bool isMove = isLeftMove || isRightMove;
            
            if(isMove)
            {
                if (isRightMove && body.LinearVelocity.X < maxSpeed)
                {
                    body.AddLinearVelosity(new Vector2f(3, 0));
                }
                else if (isLeftMove && body.LinearVelocity.X < maxSpeed)
                {
                    body.AddLinearVelosity(new Vector2f(-3, 0));
                }
            }

            if (isJump && !isAir)
            {
                body.AddLinearVelosity(new Vector2f(0, - 5 * body.MassData.Mass));
                isAir = true;
            }
        }

        private void MouseUpdate()
        {
            Vector2i mousePos = (Vector2i)Game.GetMousePositionByWorld();

            var chunk = world.GetChunkByWorldPosition((Vector2f)mousePos);

            if (chunk != null)
            {
                DebugRender.AddText(AssetManager.GetFont("Arial"), (Vector2f)mousePos - new Vector2f(0, 50), "Chunk id:" + chunk.Id.ToString());

                var tile = chunk.GetTileByWorldPosition((Vector2f)mousePos);
                if (tile != null && Mouse.IsButtonPressed(Mouse.Button.Left))
                {
                    chunk.SetTileByWorldPosition((Vector2f)mousePos, null);
                    DebugRender.AddText(AssetManager.GetFont("Arial"), (Vector2f)mousePos - new Vector2f(0, 25), "Tile id:" + tile.Id.ToString());
                }
                else if(tile == null && Mouse.IsButtonPressed(Mouse.Button.Right))
                {
                    chunk.SetTileByWorldPosition((Vector2f)mousePos, TileType.Door);
                    DebugRender.AddText(AssetManager.GetFont("Arial"), (Vector2f)mousePos - new Vector2f(0, 25), "Tile id:" + TileType.Grass.ToString());
                }
            }

            DebugRender.AddText(AssetManager.GetFont("Arial"), (Vector2f)mousePos, "Mouse global position:" + mousePos.ToString());
            DebugRender.AddText(AssetManager.GetFont("Arial"), (Vector2f)mousePos + new Vector2f(0, 100), "Mouse global position in tiles:" + (mousePos / InfoTile.TileSize).ToString());
        }
    }
}
