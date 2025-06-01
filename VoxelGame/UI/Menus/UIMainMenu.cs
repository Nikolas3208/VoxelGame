using SFML.Graphics;
using SFML.System;
using VoxelGame.Resources;
using VoxelGame.UI.Widgets;

namespace VoxelGame.UI.Menus
{
    public class UIMainMenu : UIMenu
    {
        private UIImage _logoImage;
        private UIButton _startButton;
        private UIButton _setingsButton;
        private UIButton _endButton;

        public UIMainMenu(Vector2f size, string title) : base(size, title)
        {
            backgroundTexture = TextureManager.GetTexture("Background_Menu");
            AudioManager.PlaySuond("Title-Screen");

            _logoImage = new UIImage(TextureManager.GetTexture("Terraria-Logo"));
            _logoImage.Size = _logoImage.Size * 2; // Увеличиваем размер логотипа
            _logoImage.Position = Game.GetWindowSizeWithZoom() / 2 - new Vector2f(0, Game.GetWindowSizeWithZoom().Y / 2 - _logoImage.Size.Y / 2);
            _logoImage.Origin = _logoImage.Size / 2;

            _startButton = new UIButton("Играть")
            {
                StrId = "StartButton",
                Color = Color.Transparent,
                TextColor = Color.White,
                BorderColorText = Color.Black,
                BorderThicknessText = 2,
                Position = Game.GetWindowSizeWithZoom() / 2 - new Vector2f(0, 75)
            };
            _startButton.Origin = _startButton.Size / 2;

            _startButton.OnHovered += OnStartButtonHovered;
            _startButton.OnClick += OnStartButtonClick;

            _setingsButton = new UIButton("Настройки")
            {
                StrId = "SettingsButton",
                Color = Color.Transparent,
                TextColor = Color.White,
                BorderColorText = Color.Black,
                BorderThicknessText = 2,
                Position = Game.GetWindowSizeWithZoom() / 2 - new Vector2f(0, 0)
            };
            _setingsButton.Origin = _setingsButton.Size / 2;

            _setingsButton.OnClick += OnSettingsButtonClick;
            _setingsButton.OnHovered += OnSettingsButtonHovered;

            _endButton = new UIButton("Выход")
            {
                StrId = "ExitButton",
                Color = Color.Transparent,
                TextColor = Color.White,
                BorderColorText = Color.Black,
                BorderThicknessText = 2,
                Origin = new Vector2f(150, 50) / 2,
                Position = Game.GetWindowSizeWithZoom() / 2 - new Vector2f(0, -75)
            };
            _endButton.Origin = _endButton.Size / 2;

            _endButton.OnClick += OnEndButtonClick;
            _endButton.OnHovered += OnEndButtonHovered;

            AddWidget(_logoImage);
            AddWidget(_startButton);
            AddWidget(_setingsButton);
            AddWidget(_endButton);
        }

        private void OnEndButtonHovered(bool value)
        {
            if (value)
            {
                _endButton.CharacterSize = 45;
                _endButton.TextColor = _endButton.HoveredText;
            }
            else
            {
                _endButton.CharacterSize = _endButton.StartCharacterSize;
                _endButton.TextColor = _endButton.DefaultTextColor;
            }
        }

        private void OnEndButtonClick()
        {
            Game.Close();
        }

        private void OnSettingsButtonHovered(bool value)
        {
            if (value)
            {
                _setingsButton.CharacterSize = 45;
                _setingsButton.TextColor = _setingsButton.HoveredText;
            }
            else
            {
                _setingsButton.CharacterSize = _setingsButton.StartCharacterSize;
                _setingsButton.TextColor = _setingsButton.DefaultTextColor;
            }
        }

        private void OnSettingsButtonClick()
        {
            Unfollows();
            UIManager.AddWindow(new UISettingsMenu(Game.GetWindowSizeWithZoom(), "Настройки"));
            UIManager.RemoveWindow(this);
        }

        private void OnStartButtonHovered(bool value)
        {
            if (value)
            {
                _startButton.CharacterSize = 45;
                _startButton.TextColor = _startButton.HoveredText;
            }
            else
            {
                _startButton.CharacterSize = _startButton.StartCharacterSize;
                _startButton.TextColor = _startButton.DefaultTextColor;
            }
        }

        private void OnStartButtonClick()
        {
            Game.CreateWorld();
            Unfollows();
            UIManager.RemoveWindow(this);
            AudioManager.StopSound("Title-Screen");
        }

        private bool _rotationLeft = true;

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            var scale = MathF.Abs(_logoImage.Rotation) / 10f + 0.5f;


            if (_rotationLeft)
            {
                _logoImage.Rotation -= deltaTime * 1.5f;


                if (_logoImage.Rotation <= -1)
                {
                    _rotationLeft = false;
                }
            }
            else
            {
                _logoImage.Rotation += deltaTime * 1.5f;
                if (_logoImage.Rotation >= 1)
                {
                    _rotationLeft = true;
                }
            }

            _logoImage.Scale = new Vector2f(scale, scale);
        }

        public override void Follows()
        {
            base.Follows();
        }
    }
}
