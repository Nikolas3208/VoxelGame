using SFML.Graphics;
using SFML.System;

namespace VoxelGame.UI
{
    public abstract class UIBase : Transformable, Drawable
    {
        private RectangleShape _rect;

        public string StrId { get; private set; } = string.Empty;
        public string Name { get; set; } = nameof(UIBase);
        public UIBase? Perent { get; set; }

        public Vector2f Size
        {
            get => _rect.Size;
            set
            {
                _rect.Size = value;
            }
        }

        protected UIBase()
        {
            _rect = new RectangleShape();

            StrId = Guid.NewGuid().ToString();
        }

        public Vector2f GetGlobalPosition()
        {
            if (Perent != null)
            {
                return Perent.GetGlobalPosition() + Position;
            }
            else
                return Position;
        }

        public abstract void Update(float deltaTime);
        public abstract void Draw(RenderTarget target, RenderStates states);
    }
}
