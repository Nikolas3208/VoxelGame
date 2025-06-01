using SFML.Graphics;
using SFML.System;

namespace VoxelGame.UI.Widgets
{
    public class UIImage : UIWidget
    {
        public UIImage(Texture img)
        {
            rect = new RectangleShape(new Vector2f(img.Size.X, img.Size.Y))
            {
                Texture = img
            };
        }
    }
}
