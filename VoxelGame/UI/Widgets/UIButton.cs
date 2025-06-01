using SFML.Graphics;
using SFML.System;
using SFML.Window;
using VoxelGame.Resources;

namespace VoxelGame.UI.Widgets
{
    public class UIButton : UIWidget
    {
        public event Action? OnClick;


        public UIButton(string text) : base()
        {
            this.text = new Text(text, TextureManager.GetFont("Arial"));
            this.text.FillColor = Color.Black;

            Size = this.text.GetGlobalBounds().Size;
            StartSize = Size;
            StartCharacterSize = this.text.CharacterSize;
        }

        public UIButton(Vector2f size, string text) : base(size)
        {
            this.text = new Text(text, TextureManager.GetFont("Arial"));
            this.text.FillColor = Color.Black;

            StartSize = size;
            StartCharacterSize = this.text.CharacterSize;
        }

        public override void Follows(RenderWindow window)
        {
            base.Follows(window);

            window.MouseButtonPressed += OnMouseButtonPressed;
        }

        public override void Unfollows(RenderWindow window)
        {
            base.Unfollows(window);

            window.MouseButtonPressed -= OnMouseButtonPressed;
        }

        private void OnMouseButtonPressed(object? sender, MouseButtonEventArgs e)
        {
            if(GetFloatRect().Contains(UIManager.MousePosition))
                OnClick?.Invoke();
        }

        public override void Draw(RenderTarget target, RenderStates states)
        {
            base.Draw(target, states);
        }
    }
}
