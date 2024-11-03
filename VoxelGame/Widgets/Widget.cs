using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoxelGame.Widgets
{
    public enum WidgetStyle
    {
        None,
        Default,
        Disable
    }
    public abstract class Widget : Transformable, Drawable
    {
        protected RectangleShape? rect;
        protected Text? text;
        protected Vector2f Size;

        public string Name = "";

        public Widget(string name)
        {
            Name = name;
        }

        public virtual void UpdateView()
        {

        }

        public FloatRect GetFloatRect() => new FloatRect(Position, Size);

        public abstract void Update();
        public abstract void Draw(RenderTarget target, RenderStates states);
    }
}
