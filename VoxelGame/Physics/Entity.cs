using SFML.Graphics;
using SFML.System;
using VoxelGame.Entitys;
using VoxelGame.Worlds;

namespace VoxelGame.Physics
{

    /// <summary>
    /// Перечисление, представляющее слои столкновений в игровом мире.
    /// </summary>
    public enum CollisionLayer
    {
        /// <summary>
        /// Отсутствие слоя столкновений.
        /// </summary>
        None = 0,

        /// <summary>
        /// Слой земли, используется для объектов, взаимодействующих с поверхностью.
        /// </summary>
        Ground = 1 << 0,

        /// <summary>
        /// Слой игрока, используется для персонажа, управляемого пользователем.
        /// </summary>
        Player = 2 << 1,

        /// <summary>
        /// Слой врагов, используется для враждебных NPC или существ.
        /// </summary>
        Enemy = 3 << 2,

        /// <summary>
        /// Слой предметов, используется для объектов, которые можно подобрать.
        /// </summary>
        Item = 4 << 3,

        /// <summary>
        /// Все слои, объединяющие игрока, врагов и предметы.
        /// </summary>
        All = Player | Enemy | Item
    }
    /// <summary>  
    /// Перечисление, представляющее тип сущности в игровом мире.  
    /// Static - статическая сущность, не подвержена физическим взаимодействиям.  
    /// Dynamic - динамическая сущность, подвержена физическим взаимодействиям.  
    /// Kinematic - кинематическая сущность, управляется напрямую, но может взаимодействовать с физическим миром.  
    /// </summary>  
    public enum EntityType
    {
        /// <summary>
        /// Статическая сущность, неподвижная и не взаимодействующая с физическим миром.
        /// </summary>
        Static,

        /// <summary>
        /// Динамическая сущность, подверженная физическим взаимодействиям, например, гравитации.
        /// </summary>
        Dynamic,

        /// <summary>
        /// Кинематическая сущность, управляемая напрямую, но способная взаимодействовать с физическим миром.
        /// </summary>
        Kinematic
    }     /// <summary>  
          /// Абстрактный класс, представляющий сущность в игровом мире.  
          /// Сущности могут быть статическими, динамическими или кинематическими,  
          /// взаимодействовать с физическим миром, иметь коллайдеры, скорость, цвет и другие свойства.  
          /// Этот класс также предоставляет базовую функциональность для обновления, перемещения,  
          /// обработки столкновений и отрисовки сущностей.  
          /// </summary>
    public abstract class Entity : Transformable, Drawable
    {
        private float _cos = 0;
        private float _sin = 0;

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
        protected bool isFall = true;

        /// <summary>
        /// Gets or sets the unique identifier for the entity.
        /// </summary>
        public int Id { get; set; } = -1;

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
        /// Gets or sets the <see cref="EntityManager"/> instance used to manage entities within the application.
        /// </summary>
        /// <remarks>Use this property to access or assign the entity management functionality for the
        /// application. Setting this property to <see langword="null"/> disables entity management.</remarks>
        public EntityManager? EntityManager { get; set; } = null;

        /// <summary>
        /// Цвет прямоугольника
        /// </summary>
        public Color Color
        {
            get => rect.FillColor;
            set => rect.FillColor = value;
        }

        /// <summary>  
        /// Свойство для получения или установки размера сущности.  
        /// Размер сущности определяется как размер её визуального представления (прямоугольника).  
        /// </summary>  
        /// <remarks>  
        /// При изменении размера прямоугольника визуальное представление сущности будет обновлено.  
        /// Это свойство не влияет на физический коллайдер сущности.  
        /// </remarks>  
        public Vector2f Size
        {
            get => rect.Size; // Возвращает текущий размер прямоугольника.  
            set
            {
                rect.Size = value; // Устанавливает новый размер прямоугольника.  
            }
        }

        /// <summary>  
        /// Переопределённое свойство Rotation для установки угла поворота.  
        /// При изменении угла поворота вычисляются значения косинуса и синуса,  
        /// которые могут быть использованы для трансформации объекта.  
        /// </summary>  
        public new float Rotation
        {
            get => base.Rotation;
            set
            {
                base.Rotation = value;
                _cos = MathF.Cos(value * MathF.PI / 180); // Вычисление косинуса угла поворота  
                _sin = MathF.Sin(value * MathF.PI / 180); // Вычисление синуса угла поворота  
            }
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
            rect = new RectangleShape(aabb.Max - aabb.Min);
            rect.Position = aabb.Min;

            _cos = MathF.Cos(0); // Вычисление косинуса угла поворота  
            _sin = MathF.Sin(0); // Вычисление синуса угла поворота  
        }

        /// <summary>
        /// Шаг в физике
        /// </summary>
        /// <param name="deltaTime"> Время кадра </param>
        /// <param name="gravity"> Сила гравитации </param>
        public void Step(float deltaTime, Vector2f gravity)
        {
            if (Type == EntityType.Static) return;

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
            if (normal.Y < 0 && other == null)
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

            if (Visible)
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
        /// Устанавливает новый коллайдер для сущности.  
        /// </summary>  
        /// <param name="aabb">Новый коллайдер типа AABB.</param>  
        public void SetAABB(AABB aabb)
        {
            _aabb = aabb;
        }

        /// <summary>
        /// Получить колайдер
        /// </summary>
        /// <returns></returns>
        public AABB GetAABB()
        {
            return _aabb.Transform(Position);
        }

        /// <summary>
        /// Обноеление
        /// </summary>
        /// <param name="deltaTime"> Вермя кадраы </param>
        public abstract void Update(float deltaTime);
    }
}
