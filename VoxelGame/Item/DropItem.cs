using SFML.Graphics;
using SFML.System;
using VoxelGame.Entitys;
using VoxelGame.Graphics;
using VoxelGame.Graphics.Animations;
using VoxelGame.Meths;
using VoxelGame.Physics;
using VoxelGame.Resources;
using VoxelGame.Worlds;

namespace VoxelGame.Item
{
    public class DropItem : Entity
    {
        private bool isDrop = true;

        /// <summary>
        /// Предмет
        /// </summary>
        public Item Item { get; private set; }

        public float AnimSpeed { get; set; } = 0.3f;

        private Animator _animSprite { get; set; }

        /// <summary>
        /// Выброшеный предмет?
        /// </summary>
        public bool IsDrop 
        { 
            get => isDrop;
            set
            {
                isDrop = value;

                if(value)
                {
                    Type = EntityType.Dynamic;
                }
                else
                {
                    Type = EntityType.Static;
                    velocity = new Vector2f(0, 0);
                }
            }
        }

        /// <summary>
        /// Количество предметов
        /// </summary>
        public int ItemCount { get; private set; } = 1;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="item"> Предмет </param>
        /// <param name="aabb"> Границы (размер) </param>
        /// <param name="world"> Мир </param>
        public DropItem(Item item, AABB aabb, World world) : base(world, aabb)
        {
            Item = item;
            Layer = CollisionLayer.Item;
            CollidesWith = CollisionLayer.Ground;

            rect.Texture = TextureManager.GetTexture(item.SpriteName);

            Origin = -new Vector2f(0, aabb.Max.Y);

            _animSprite = new Animator();
            _animSprite.AddAnimation("Use", new Animation(TextureManager.GetSpriteSheet(item.SpriteName, (int)aabb.Max.X, (int)aabb.Max.Y),
                AnimationType.Tranformation,
                new AnimationFrame(0, 0, AnimSpeed, -190, new Vector2f(-10, 17)),
                new AnimationFrame(0, 0, AnimSpeed, -85, new Vector2f(23, -1)),
                new AnimationFrame(0, 0, AnimSpeed, 0, new Vector2f(33, 28)),
                new AnimationFrame(0, 0, AnimSpeed, 65, new Vector2f(16, 48)),
                new AnimationFrame(0, 0, AnimSpeed, 85, new Vector2f(4, 49))));
        }

        float animAngle = 0f;


        /// <summary>
        /// Обновление предмета
        /// </summary>
        /// <param name="deltaTime"> Время кадра </param>
        public override void Update(float deltaTime)
        {
            _animSprite.Update(deltaTime);

            if (!IsDrop)
                return;

            animAngle += deltaTime * 3f;

            rect.Position = new Vector2f(0, MathF.Cos(animAngle) * 4f);

            var playerPosition = world.GetPlayer()?.Position ?? new Vector2f(0, 0);

            if(MathHelper.DistanceSquared(Position, playerPosition) < 25000)
            {
                var direction = playerPosition - Position;
                velocity += direction * 0.5f;
            }
        }

        public override void OnCollided(Entity other, Vector2f normal, float depth)
        {
            base.OnCollided(other, normal, depth);

            if(other is Npc npc)
            {
                if(npc.NpcType == NpcType.Enemy)
                {
                    npc.Damege(Item.Damage, normal);
                }
            }
        }

        /// <summary>
        /// Отрисовка предмета
        /// </summary>
        /// <param name="target"></param>
        /// <param name="states"></param>
        public override void Draw(RenderTarget target, RenderStates states)
        {
            if (IsDrop)
                base.Draw(target, states);
            else
            {
                states.Transform *= Transform;

                _animSprite.Draw(target, states);
            }
        }

        public void UseAnimation()
        {
            if (!IsDrop)
                _animSprite.Play("Use");
        }

        /// <summary>
        /// Добавить предмет в стак
        /// </summary>
        /// <param name="item"> Предмет </param>
        /// <param name="count"> Количество </param>
        /// <returns></returns>
        public bool AddItem(Item item, int count)
        {
            if (item.GetType() == Item.GetType() && ItemCount + count <= item.MaxCoutnInStack)
            {
                ItemCount += count;
                return true;
            }

            return false;
        }
    }
}
