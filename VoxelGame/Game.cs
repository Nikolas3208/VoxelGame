using SFML.Graphics;
using SFML.System;
using SFML.Window;
using VoxelGame.UI;
using VoxelGame.Worlds;

namespace VoxelGame
{
    public class Game
    {
        private RenderWindow _window;
        private World _world;

        private static Vector2f _mousePos;
        private static Vector2u _windowSize;
        private static Vector2f _cameraPosition;

        public Game(VideoMode mode, string title)
        {
            _window = new RenderWindow(mode, title);
            _window.Closed += (sender, e) => { _window.Close(); };
            _window.Resized += (sender, e) =>
            {
                _window.SetView(new View(new FloatRect(0, 0, e.Width, e.Height)));
                _windowSize = new Vector2u(e.Width, e.Height);
            };

            _windowSize = _window.Size;

            _world = WorldGenerator.GenerateWorld(200, 200);
        }

        public void Run()
        {
            var clock = new Clock();

            _window.SetFramerateLimit(60);

            while (_window.IsOpen)
            {
                _window.DispatchEvents();

                Update(clock.Restart().AsSeconds());

                _window.Clear(Color.Cyan);

                Draw(_window, RenderStates.Default);

                _window.Display();
            }
        }


        private void Update(float deltaTime)
        {
            _mousePos = (Vector2f)Mouse.GetPosition(_window);
            _world.Update(deltaTime);

            UIManager.Update(deltaTime);

            DebugRender.AddRectangle(new FloatRect(GetMousePosition(), new Vector2f(16, 16)), Color.Green);
            DebugRender.AddRectangle(new FloatRect(GetMousePositionByWorld(), new Vector2f(16, 16)), Color.Green);
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            var cameraCenterPosition = _cameraPosition - (Vector2f)_window.Size / 2;
            var view = new View(new FloatRect(cameraCenterPosition, (Vector2f)_window.Size));
            if (Keyboard.IsKeyPressed(Keyboard.Key.R))
                view.Zoom(2);
            else
                view.Zoom(0.5f);
            _window.SetView(view);

            _world.Draw(target, states);

            UIManager.Draw(target, states);
            DebugRender.Draw(target, states);
        }

        public static void SetCameraPosition(Vector2f position)
        {
            _cameraPosition = position;
        }

        public static Vector2u GetWindowSize() => _windowSize;
        public static Vector2f GetMousePosition() => _mousePos;
        public static Vector2f GetMousePositionByWorld() => (_mousePos * 0.5f) + (_cameraPosition - ((Vector2f)_windowSize / 2 * 0.5f));
        public static Vector2f GetCameraPosition() => _cameraPosition;
        public static Vector2f GetCenterScreenByWorld() => _cameraPosition - (Vector2f)_windowSize / 2;
    }
}
