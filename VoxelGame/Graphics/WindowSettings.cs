using SFML.Window;

namespace VoxelGame.Graphics
{
    public struct WindowSettings
    {
        /// <summary>
        /// Заголовок окна
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Режим отображения (разрешение)
        /// </summary>
        public VideoMode VideoMode { get; set; } = VideoMode.DesktopMode;
        public ContextSettings ContextSettings { get; set; }

        /// <summary>
        /// Стиль окна
        /// </summary>
        public Styles Styles { get; set; } = Styles.Default;

        public WindowSettings(string title, VideoMode videoMode, Styles styles)
        {
            Title = title;
            VideoMode = videoMode;
            Styles = styles;
        }

        /// <summary>
        /// Стандартые настройки ("Window", DesktopMode, Default)
        /// </summary>
        public WindowSettings Defauld => new WindowSettings("Window", VideoMode.DesktopMode, Styles.Default);
    }
}
