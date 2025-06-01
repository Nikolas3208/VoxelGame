using SFML.Graphics;
using SFML.System;

namespace VoxelGame.UI.Widgets
{
    public enum Styles
    {
        Default,
        Hovered,
        Pressed
    }
    public abstract class UIWidget : UIBase
    {
        protected Text? text;

        public new Vector2f Position
        {
            get => base.Position;
            set
            {
                base.Position = value;
                if (text != null)
                {
                    text.Origin = new Vector2f(text.GetGlobalBounds().Width / 2, text.GetGlobalBounds().Height / 2);

                    text.Position = new Vector2f(rect.GetGlobalBounds().Width / 2, rect.GetGlobalBounds().Height / 2.5f);
                }
            }
        }

        public new Vector2f Size
        {
            get => base.Size;
            set
            {
                base.Size = value;
                Origin = value / 2;
                if (text != null)
                {
                    text.Origin = new Vector2f(text.GetGlobalBounds().Width / 2, text.GetGlobalBounds().Height / 2);

                    text.Position = new Vector2f(rect.GetGlobalBounds().Width / 2, rect.GetGlobalBounds().Height / 2.5f);
                }
            }
        }

        public new Vector2f Origin
        {
            get => base.Origin;
            set
            {
                base.Origin = value;
                if (text != null)
                {
                    text.Origin = new Vector2f(text.GetGlobalBounds().Width / 2, text.GetGlobalBounds().Height / 2);

                    text.Position = new Vector2f(rect.GetGlobalBounds().Width / 2, rect.GetGlobalBounds().Height / 2.5f);
                }
            }
        }

        public Vector2f StartSize { get; set; }
        public uint StartCharacterSize { get; set; }

        public string Text
        {
            get => text?.DisplayedString ?? string.Empty;
            set
            {
                if (text != null)
                {
                    text.DisplayedString = value;
                    Size = text.GetGlobalBounds().Size;
                }
            }
        }

        public uint CharacterSize
        {
            get => text?.CharacterSize ?? 0;
            set
            {
                if (text != null)
                {
                    text.CharacterSize = value;
                    Size = text.GetGlobalBounds().Size;
                }
            }
        }

        public Color TextColor
        {
            get => text?.FillColor ?? Color.White;
            set
            {
                if (text != null)
                {
                    text.FillColor = value;
                }
            }
        }

        public Color BorderColorText
        {
            get => text?.OutlineColor ?? Color.White;
            set
            {
                if (text != null)
                {
                    text.OutlineColor = value;
                }
            }
        }

        public float BorderThicknessText
        {
            get => text?.OutlineThickness ?? 0;
            set
            {
                if (text != null)
                {
                    text.OutlineThickness = value;
                }
            }
        }

        public Color DefaultTextColor = new Color(200, 200, 200);
        public Color HoveredText = new Color(255, 255, 130);
        public Color PressedText = new Color(100, 100, 100);

        public Color DefaultRect = new Color(200, 200, 200);
        public Color HoveredRect = new Color(150, 150, 150);
        public Color PressedRect = new Color(100, 100, 100);

        public Color Color
        {
            get => rect.FillColor;
            set => rect.FillColor = value;
        }

        public Color BorderColor
        {
            get => rect.OutlineColor;
            set => rect.OutlineColor = value;
        }

        public float BorderThickness
        {
            get => rect.OutlineThickness;
            set => rect.OutlineThickness = value;
        }

        public UIWidget()
        {

        }

        public UIWidget(Vector2f size)
        {
            rect = new RectangleShape(size);

            StartSize = size;
        }

        public virtual void Follows(RenderWindow window)
        {

        }

        public virtual void Unfollows(RenderWindow window)
        {

        }

        public override void Draw(RenderTarget target, RenderStates states)
        {
            base.Draw(target, states);

            states.Transform *= Transform;

            text?.Draw(target, states);
        }
    }
}
