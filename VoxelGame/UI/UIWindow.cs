using SFML.Graphics;
using SFML.System;
using VoxelGame.Resources;

namespace VoxelGame.UI
{
    public class UIWindow : UIBase
    {
        protected RectangleShape titleBar;
        protected Text titleText;
        protected Texture backgroundTexture
        {
            get => rect.Texture;
            set => rect.Texture = value;
        }
        
        public string Title
        {
            get => titleText.DisplayedString;
            set => titleText.DisplayedString = value;
        }

        public bool TitleBarIsVisible { get; set; } = true;

        public UIWindow(Vector2f size, string title = "Window")
        {
            rect = new RectangleShape(size);
            //rect.Origin = new Vector2f(40, Size.Y / 2);
            rect.FillColor = new Color(200, 200, 200);

            titleBar = new RectangleShape(new Vector2f(size.X, 32));
            //titleBar.Origin = titleBar.Size / 2;
            titleBar.Position = new Vector2f(0, -size.Y) / 2;

            titleText = new Text(title, TextureManager.GetFont("Arial"));
            titleText.FillColor = Color.Black;
            //titleText.Origin = titleBar.Size / 2;
            titleText.Position = titleBar.Position;
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
        }

        public override void Draw(RenderTarget target, RenderStates states)
        {
            base.Draw(target, states);

            states.Transform *= Transform;

            if (TitleBarIsVisible)
            {
                target.Draw(titleBar, states);
                target.Draw(titleText, states);
            }
        }
    }
}