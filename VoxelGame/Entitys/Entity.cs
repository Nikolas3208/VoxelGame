using SFML.Graphics;
using SFML.System;
using VoxelGame.Physics;
using VoxelGame.Worlds;

namespace VoxelGame.Npc
{
    public abstract class Entity : Drawable
    {
        private Transform _transform;
        private Vector2f _oldPosition;

        protected World world;
        protected RigidBody body;
        protected Shape shape;

        protected bool isAir = false;

        public Vector2f Position
        { 
            get => body.Position;
            set
            {
                _oldPosition = body.Position;
                body.Position = value;
            }
        }

        public Entity(RigidBody rigidBody, World world)
        {
            this.world = world;

            body = rigidBody;
            body.Layer = CollisionLayer.Entity;
            body.CollidesWith = CollisionLayer.Ground | CollisionLayer.Enemy;
            body.Collided += OnCollision;

            if (body.ColliderType == ColliderType.Circle)
            {
                shape = new CircleShape(body.GetCircle().Radius);
            }
            else
            {
                shape = new RectangleShape(body.GetPolygon().Size);
                shape.Origin = body.GetPolygon().Size / 2;
            }

            _transform = new Transform(1, 0, 0, 0, 1, 0, 0, 0, 1);
        }

        public virtual void OnCollision(BodyCollidedEvent e)
        {
            if (e.Body.Layer == CollisionLayer.Ground && e.Normal.Y != 0)
            {
                isAir = false;
            }
        }

        public virtual void Draw(RenderTarget target, RenderStates states)
        {
            states.Transform *= new Transform(1, 0, Position.X, 0, 1, Position.Y, 0, 0, 1);

            shape.Draw(target, states);
        }

        public RigidBody GetBody()
        {
            return body;
        }

        public abstract void Update(float deltaTime);
    }
}
