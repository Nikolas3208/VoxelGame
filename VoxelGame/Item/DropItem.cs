using SFML.Graphics;
using SFML.System;
using VoxelGame.Graphics;
using VoxelGame.Meths;
using VoxelGame.Physics;
using VoxelGame.Resources;
using VoxelGame.Worlds;

namespace VoxelGame.Item
{
    public class DropItem : Entity
    {
        /// <summary>
        /// Предмет
        /// </summary>
        public Item Item { get; private set; }

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

            rect.Texture = AssetManager.GetTexture(item.SpriteName);
        }

        float animAngle = 0f;


        /// <summary>
        /// Обновление предмета
        /// </summary>
        /// <param name="deltaTime"> Время кадра </param>
        public override void Update(float deltaTime)
        {
            animAngle += deltaTime * 3f;

            rect.Position = new Vector2f(0, MathF.Cos(animAngle) * 4f);

            var playerPosition = world.GetPlayer()?.Position ?? new Vector2f(0, 0);

            if(MathHelper.DistanceSquared(Position, playerPosition) < 25000)
            {
                var direction = playerPosition - Position;
                velocity += direction * 0.5f;
            }
        }

        /// <summary>
        /// Отрисовка предмета
        /// </summary>
        /// <param name="target"></param>
        /// <param name="states"></param>
        public override void Draw(RenderTarget target, RenderStates states)
        {
            base.Draw(target, states);
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
