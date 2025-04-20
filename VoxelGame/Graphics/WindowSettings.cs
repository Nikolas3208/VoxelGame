using SFML.Window;

namespace VoxelGame.Graphics
{
    public struct WindowSettings
    {
        public string Title { get; set; } = string.Empty;

        public VideoMode VideoMode { get; set; } = VideoMode.DesktopMode;
        public ContextSettings ContextSettings { get; set; }
        public Styles Styles { get; set; } = Styles.Default;

        public WindowSettings(string title, VideoMode videoMode, Styles styles)
        {
            Title = title;
            VideoMode = videoMode;
            Styles = styles;
        }

        public WindowSettings Defauld => new WindowSettings("Window", VideoMode.DesktopMode, Styles.Default);
    }
}
