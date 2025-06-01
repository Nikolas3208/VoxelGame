using SFML.Graphics;
using VoxelGame.Resources;

namespace VoxelGame.UI.Widgets
{
    public class UILabel : UIWidget
    {
        public UILabel(string text) : base()
        {
            this.text = new Text(text, TextureManager.GetFont("Arial"));
            this.text.FillColor = Color.Black;

            Size = this.text.GetGlobalBounds().Size;

            StartSize = Size;
            StartCharacterSize = this.text.CharacterSize;
        }
    }
}
