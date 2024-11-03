using SFML.Graphics;
using SFML.System;
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
    public abstract class Entity : Transformable
    {
        protected World World;
        protected RigidBody? RigidBody;
        protected RectangleShape? rect;

        protected Vector2f velocity;

        public Entity(World world)
        {
            World = world;
        }

        public virtual void Start()
        {

        }

        public virtual void Update(float deltaTime)
        {
            velocity.X *= 0.99f;
            velocity.Y += 0.55f;

            var size = rect!.Size.X / InfoTile.MinTileSize;

            for (int i = 0; i < size; i++)
            {
                var tile = World.GetTileByWorldPosition(new Vector2f((Position.X) + i * InfoTile.MinTileSize, Position.Y + rect.Size.Y) / InfoTile.MinTileSize);

                if (tile != null && !tile.IsWall)
                {
                    var tileRect = tile.GetFloatRect();
                    DebugRender.AddRectangle(tileRect, Color.Blue);

                    if (GetFloatRect().Intersects(tileRect))
                    {
                        float offset = MathHelper.GetDistance(Position, tileRect.Position);

                        velocity.Y = 0;
                        velocity.Y -= offset / InfoTile.MinTileSize;
                    }
                }
            }

            Position += velocity * deltaTime;
        }

        public FloatRect GetFloatRect() => new FloatRect(new Vector2f(Position.X, Position.Y + rect!.Size.Y), rect.Size);

        public abstract void Draw(RenderTarget target, RenderStates states);
    }
}
