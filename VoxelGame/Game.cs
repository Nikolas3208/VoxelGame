using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoxelGame.Worlds;

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

        public void Init()
        {
            World = new World();
            clock = new Clock();
        }

        public void Run()
        {
            //window.SetFramerateLimit(60);

            while(window.IsOpen)
            {
                window.DispatchEvents();

                window.Clear();

                float deltaTime = clock.Restart().AsSeconds();
                window.SetTitle($"Fps: {(int)(1 / deltaTime)}, deltaTime: {deltaTime}");

                World!.Update(window, deltaTime);
                World!.Draw(window, RenderStates.Default);

                window.Display();
            }
        }

        public static Vector2i GetMousePosition() => Mouse.GetPosition(window);

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
