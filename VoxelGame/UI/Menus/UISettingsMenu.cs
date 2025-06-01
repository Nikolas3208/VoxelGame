using SFML.Graphics;
using SFML.System;
using VoxelGame.Resources;
using VoxelGame.UI.Widgets;

namespace VoxelGame.UI.Menus
{
    public class UISettingsMenu : UIMenu
    {
        private UILabel _titleLabel;
        private UIButton _backButton;
        public UISettingsMenu(Vector2f size, string title) : base(size, title)
        {
            backgroundTexture = TextureManager.GetTexture("Background_Menu");

            _titleLabel = new UILabel("Настройки")
            {
                Position = new Vector2f(size.X / 2, Game.GetWindowSize().Y / 4),
                Color = Color.Transparent,
                TextColor = Color.White,
                BorderColorText = Color.Black,
                BorderThicknessText = 2,
                CharacterSize = 48
            };
            _titleLabel.Origin = _titleLabel.Size / 2;

            _backButton = new UIButton("Назад")
            {
                StrId = "BackButton",
                Color = Color.Transparent,
                TextColor = Color.White,
                BorderColorText = Color.Black,
                BorderThicknessText = 2,
                Position = Game.GetWindowSize() / 2
            };
            _backButton.Origin = _backButton.Size / 2;

            _backButton.OnClick += () => { UIManager.AddWindow(new UIMainMenu(size, "Меню")); Unfollows(); UIManager.RemoveWindow(this); };
            _backButton.OnHovered += (bool value) => {
                if (value)
                {
                    _backButton.CharacterSize = 45;
                    _backButton.TextColor = _backButton.HoveredText;
                }
                else
                {
                    _backButton.CharacterSize = _backButton.StartCharacterSize;
                    _backButton.TextColor = _backButton.DefaultTextColor;
                }
            };


            AddWidget(_titleLabel);
            AddWidget(_backButton);
        }
    }
}
