using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoxelGame.Widgets
{
    public class Grid : Transformable, Drawable
    {
        private List<Widget> widgets;

        public Grid()
        {
            widgets = new List<Widget>();
        }

        public void AddWidget(Widget widget) => widgets.Add(widget);
        public Widget GetWidgetById(int id) => widgets[id];
        public Widget GetWidgetByName(string nameWidget)
        {
            foreach (var widget in widgets)
            {
                if(widget.Name == nameWidget)
                    return widget;
            }

            return null;
        }

        public void Update()
        {
            foreach (var widget in widgets)
            {
                widget.Update();
            }
        }
        public void Draw(RenderTarget target, RenderStates states)
        {
            foreach (Widget widget in widgets)
            {
                widget.Draw(target, states);
            }
        }
    }
}
