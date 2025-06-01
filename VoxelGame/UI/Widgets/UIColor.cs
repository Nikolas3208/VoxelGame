using SFML.Graphics;
using SFML.System;

namespace VoxelGame.UI.Widgets
{
    public class UIColor : UIWidget
    {
        public UIColor(Vector2f size, Color color) : base(size)
        {
            Color = color;
            BorderColor = Color.Black;
            BorderThickness = 2;
        }
    }
}
