using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoxelGame.Widgets;
using VoxelGame.Widgets.WidgetList;
using VoxelGame.Worlds;
using VoxelGame.Worlds.Chunks.Tile;

namespace VoxelGame
{
    public class Game
    {
        private static RenderWindow window;
        private World? World;

        private Clock clock;
        public Game(WindowSettings settings)
        {
            window = new RenderWindow(settings.VideoMode, settings.Title, settings.Styles);
            window.Closed += Window_Closed;
            window.Resized += Window_Resized;

            Init();
        }
        Grid grid = new Grid();

        public void Init()
        {
            World = new World();
            clock = new Clock();
            grid.AddWidget(new Button("bt_1", "Click my"));

            ContentManager.LoadContent("Contents\\");
        }

        public void Run()
        {
            //window.SetFramerateLimit(60);

            while(window.IsOpen)
            {
                window.DispatchEvents();

                window.Clear(Color.White);

                float deltaTime = clock.Restart().AsSeconds();
                window.SetTitle($"Fps: {(int)(1 / deltaTime)}, deltaTime: {deltaTime}");

                World!.Update(window, deltaTime);
                World!.Draw(window, RenderStates.Default);

                grid.Update();
                grid.Draw(window, RenderStates.Default);

                window.Display();
            }
        }

        public static Vector2f ViewPosition;

        public static Vector2i WindowSize => (Vector2i)window.Size;

        public static void UpdateView(Vector2f position)
        {
            ViewPosition = position;

            window.SetView(new View(new FloatRect(position.X - (window.Size.X / 2), position.Y - (window.Size.Y / 2), window.Size.X, window.Size.Y)));
        }

        public static Vector2i GetMousePosition() => Mouse.GetPosition(window);
        public static Vector2i GetGlobalMousePosition() => new Vector2i(Mouse.GetPosition(window).X + (int)(ViewPosition.X - (window.Size.X / 2)), Mouse.GetPosition(window).Y + (int)(ViewPosition.Y - (window.Size.Y / 2)));

        public static FloatRect GetGlobalMouseFloatRect() => new FloatRect((Vector2f)GetGlobalMousePosition(), new Vector2f(InfoTile.MinTileSize, InfoTile.MinTileSize));
        private void Window_Resized(object? sender, SizeEventArgs e)
        {
            window.SetView(new View(new FloatRect(0, 0, e.Width, e.Height)));
        }

        private void Window_Closed(object? sender, EventArgs e)
        {
            window.Close();
        }
    }
}
