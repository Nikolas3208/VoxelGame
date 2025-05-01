using SFML.Graphics;
using SFML.System;
using VoxelGame.Worlds;

namespace VoxelGame.Physics
{

    public enum CollisionLayer
    {
        None = 0,
        Ground = 1 << 0,
        Player = 2 << 1,
        Enemy = 3 << 2,
        Item = 4 << 3,
        All = Player | Enemy | Item
    }
    public enum EntityType
    {
        Static,
        Dynamic,
        Kinematic
    }
    public abstract class Entity : Transformable, Drawable
    {
        private AABB _aabb;

        protected World world;
        protected RectangleShape rect;

        protected Vector2f velocity;

        protected bool isFall = false;

        public EntityType Type { get; set; } = EntityType.Dynamic;

        public CollisionLayer Layer { get; set; }
        public CollisionLayer CollidesWith { get; set; } = CollisionLayer.All;

        public bool Visible { get; set; } = true;

        public Color Color
        {
            get => rect.FillColor;
            set => rect.FillColor = value;
        }

        public Entity(World world, AABB aabb)
        {
            this.world = world;

            _aabb = aabb;
            _aabb.Entity = this;
            rect = new RectangleShape(aabb.Max);
        }

        public void Step(float deltaTime, Vector2f gravity)
        {
            velocity += gravity * deltaTime;

            Position += velocity * deltaTime;
        }

        public void Move(Vector2f offset)
        {
            if (Type == EntityType.Static) return;

            Position += offset;
        }

        public void AddVelocity(Vector2f offset)
        {
            if (Type == EntityType.Static) return;

            velocity += offset;
        }

        public virtual void OnCollided(Entity other, Vector2f normal, float depth)
        {
            if(normal.Y < 0)
            {
                isFall = false;
            }
        }

        public virtual void Draw(RenderTarget target, RenderStates states)
        {
            states.Transform *= Transform;

            if(Visible)
            {
                target.Draw(rect, states);
            }
        }

        public Vector2f GetVelocity() => velocity;

        public AABB GetAABB() => _aabb.Transform(Position);

        public abstract void Update(float deltaTime);
    }
}
