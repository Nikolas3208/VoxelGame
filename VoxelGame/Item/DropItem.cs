using SFML.Graphics;
using SFML.System;
using VoxelGame.Graphics;
using VoxelGame.Physics;
using VoxelGame.Resources;

namespace VoxelGame.Item
{
    public class DropItem : Transformable, Drawable
    {
        private Sprite _sprite;
        private RigidBody _body;

        public InfoItem Item { get; private set; }

        public new Vector2f Position
        {
            get => _body.Position;
            set => _body.Position = value;
        }

        public DropItem(InfoItem item, RigidBody body)
        {
            Item = item;
            _body = body;
            _body.Layer = CollisionLayer.Item;
            _body.CollidesWith = CollisionLayer.Ground;

            SpriteSheet ss;

            switch (item.Type)
            {
                case ItemType.Tile:
                    ss = AssetManager.GetSpriteSheet("terrain");
                    break;
                default:
                    ss = AssetManager.GetSpriteSheet("items");
                    break;
            }

            _sprite = ss.Sprite;
            _sprite.TextureRect = ss.GetTextureRect(item.SpriteIndex);

            _sprite.Origin = new Vector2f(ss.SubWidth, ss.SubHeight) / 2;
        }

        public void Update(float deltaTime)
        {
            base.Position = Position;
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            states.Transform *= Transform;

            target.Draw(_sprite, states);
        }

        public RigidBody GetBody()
        {
            return _body;
        }
    }
}
