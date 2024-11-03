using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoxelGame.Widgets.WidgetList
{
    public class Button : Widget
    {
        private bool IsClick = false;
        private bool IsSelected = false;
        public Button(string name, string text) : base(name)
        {
            Size = new Vector2f(50, 100);
            rect = new RectangleShape(Size);
        }

        public override void Update()
        {
            if(Game.GetGlobalMouseFloatRect().Intersects(GetFloatRect()))
                IsSelected = true;
            else
                IsSelected = false;

            if (IsSelected && Mouse.IsButtonPressed(Mouse.Button.Left))
                IsClick = true;
            else
                IsClick = false;

            UpdateView();
        }

        public override void Draw(RenderTarget target, RenderStates states)
        {
            if (rect != null)
                target.Draw(rect, states);
            if (text != null)
                target.Draw(text, states);
        }
    }
}
