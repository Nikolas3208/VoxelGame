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
        /// <summary>
        /// Колайдер сушности
        /// </summary>
        private AABB _aabb;

        /// <summary>
        /// Мир (родитель)
        /// </summary>
        protected World world;

        /// <summary>
        /// Прямоугольник
        /// </summary>
        protected RectangleShape rect;

        /// <summary>
        /// Скорость
        /// </summary>
        protected Vector2f velocity;

        /// <summary>
        /// Сушность падает?
        /// </summary>
        protected bool isFall = false;

        /// <summary>
        /// Тип сушности, в физике
        /// </summary>
        public EntityType Type { get; set; } = EntityType.Dynamic;

        /// <summary>
        /// Слой сушности
        /// </summary>
        public CollisionLayer Layer { get; set; }

        /// <summary>
        /// Слои с которыми может пересекаться
        /// </summary>
        public CollisionLayer CollidesWith { get; set; } = CollisionLayer.All;

        /// <summary>
        /// Прямоугольник видно?
        /// </summary>
        public bool Visible { get; set; } = true;

        /// <summary>
        /// Цвет прямоугольника
        /// </summary>
        public Color Color
        {
            get => rect.FillColor;
            set => rect.FillColor = value;
        }

        /// <summary>
        /// Сушность
        /// </summary>
        /// <param name="world"> Мир </param>
        /// <param name="aabb"> Колайдер </param>
        public Entity(World world, AABB aabb)
        {
            this.world = world;

            _aabb = aabb;
            _aabb.Entity = this;
            rect = new RectangleShape(aabb.Max);
        }

        /// <summary>
        /// Шаг в физике
        /// </summary>
        /// <param name="deltaTime"> Время кадра </param>
        /// <param name="gravity"> Сила гравитации </param>
        public void Step(float deltaTime, Vector2f gravity)
        {
            velocity += gravity * deltaTime;

            Position += velocity * deltaTime;
        }

        /// <summary>
        /// Сместить на вектор
        /// </summary>
        /// <param name="offset"> Вектор смешения </param>
        public void Move(Vector2f offset)
        {
            if (Type == EntityType.Static) return;

            Position += offset;
        }

        /// <summary>
        /// Добавить скорость
        /// </summary>
        /// <param name="offset"></param>
        public void AddVelocity(Vector2f offset)
        {
            if (Type == EntityType.Static) return;

            velocity += offset;
        }

        /// <summary>
        /// При соприкосновении
        /// </summary>
        /// <param name="other"> Сушность с которой столкнулся </param>
        /// <param name="normal"> Вектор направления </param>
        /// <param name="depth"> Глубина </param>
        public virtual void OnCollided(Entity other, Vector2f normal, float depth)
        {
            if(normal.Y < 0)
            {
                isFall = false;
            }
        }

        /// <summary>
        /// Рисовать
        /// </summary>
        /// <param name="target"></param>
        /// <param name="states"></param>
        public virtual void Draw(RenderTarget target, RenderStates states)
        {
            states.Transform *= Transform;

            if(Visible)
            {
                target.Draw(rect, states);
            }
        }

        /// <summary>
        /// Получить скорость
        /// </summary>
        /// <returns></returns>
        public Vector2f GetVelocity() => velocity;

        /// <summary>
        /// Получить колайдер
        /// </summary>
        /// <returns></returns>
        public AABB GetAABB() => _aabb.Transform(Position);

        /// <summary>
        /// Обноеление
        /// </summary>
        /// <param name="deltaTime"> Вермя кадраы </param>
        public abstract void Update(float deltaTime);
    }
}
