using SFML.Graphics;
using SFML.System;
using SFML.Window;
using VoxelGame.Resources;
using VoxelGame.UI;
using VoxelGame.UI.Menus;
using VoxelGame.Worlds;

namespace VoxelGame
{
    /// <summary>
    /// Основной класс игры, управляющий её жизненным циклом, состоянием и взаимодействием с игровым миром.
    /// </summary>
    public class Game : Transformable
    {
        /// <summary>
        /// Ссылка на текущий игровой мир.
        /// </summary>
        private static World? _world;

        /// <summary>
        /// Текущая позиция мыши в игровом окне.
        /// </summary>
        private static Vector2f _mousePos;

        /// <summary>
        /// Размер игрового окна.
        /// </summary>
        private static Vector2u _windowSize;

        /// <summary>
        /// Позиция камеры в игровом мире.
        /// </summary>
        private static Vector2f _cameraPosition;

        /// <summary>
        /// Текущий уровень масштабирования камеры.
        /// </summary>
        private static float _zoom = 1f;

        /// <summary>
        /// Ссылка на окно рендера SFML.
        /// </summary>
        public static RenderWindow Window { get; set; } = null!;

        /// <summary>
        /// Флаг, указывающий, включён ли режим отладки.
        /// </summary>
        public static bool IsDebugMode { get; set; } = true;

        /// <summary>
        /// Флаг, указывающий, загружен ли игровой мир.
        /// </summary>
        public static bool WorldLoaded { get; set; } = false;

        /// <summary>
        /// Флаг, указывающий, находится ли игра в паузе.
        /// </summary>
        public static bool IsPaused { get; set; } = false;

        /// <summary>
        /// Конструктор игры. Инициализирует окно, загружает ресурсы и добавляет главное меню.
        /// </summary>
        /// <param name="mode">Режим окна (размер).</param>
        /// <param name="title">Заголовок окна.</param>
        public Game(VideoMode mode, string title)
        {
            Window = new RenderWindow(mode, title);

            // Закрытие окна
            Window.Closed += (sender, e) => { AudioManager.Dispose(); Window.Close(); };

            // Изменение размера окна
            Window.Resized += (sender, e) =>
            {
                Window.SetView(new View(new FloatRect(0, 0, e.Width, e.Height)));
                _windowSize = new Vector2u(e.Width, e.Height);
            };

            // Обработка нажатий клавиш для включения/выключения отладочных режимов
            Window.KeyPressed += (sender, e) =>
            {
                switch (e.Code)
                {
                    case Keyboard.Key.F3:
                        World.BaseDrawDebug = !World.BaseDrawDebug;
                        break;
                    case Keyboard.Key.F4:
                        World.ChunkBorderDrawDebug = !World.ChunkBorderDrawDebug;
                        break;
                    case Keyboard.Key.F5:
                        World.EntitiesDrawDebug = !World.EntitiesDrawDebug;
                        break;
                    case Keyboard.Key.F6:
                        World.CollideDrawDebug = !World.CollideDrawDebug;
                        break;
                }
            };

            _windowSize = Window.Size;

            // Загрузка текстур и аудио
            TextureManager.LoadAll();
            AudioManager.LoadAll();

            // Установка иконки окна
            Window.SetIcon(512, 512, TextureManager.GetTexture("Terraria-Icon").CopyToImage().Pixels);

            // Добавление главного меню
            UIManager.AddWindow(new UIMainMenu((Vector2f)_windowSize, "MainMenu"));
        }

        /// <summary>
        /// Запускает игровой цикл.
        /// </summary>
        public void Run()
        {
            var clock = new Clock();
            Window.SetFramerateLimit(60);

            float deltaTime = 0f;

            while (Window.IsOpen)
            {
                Window.DispatchEvents(); // Обработка событий окна

                deltaTime = clock.Restart().AsSeconds();

                // Ограничение слишком больших значений deltaTime
                if (deltaTime > 0.5)
                    deltaTime = 0.5f;

                Update(deltaTime); // Обновление состояния игры

                Window.Clear(Color.Cyan); // Очистка экрана

                Draw(Window, RenderStates.Default); // Отрисовка игры

                Window.Display(); // Отображение кадра
            }
        }

        /// <summary>
        /// Обновляет состояние игры.
        /// </summary>
        /// <param name="deltaTime">Время между кадрами.</param>
        private void Update(float deltaTime)
        {
            // Если окно не в фокусе, останавливаем звуки
            if (!Window.HasFocus())
            {
                AudioManager.StopAllSound();
                return;
            }

            // Обновляем позицию камеры
            Position = Window.GetView().Center - GetWindowSizeWithZoom() / 2;

            // Если игра на паузе или мир не загружен, обновляем только UI
            if (IsPaused || !WorldLoaded)
            {
                UIManager.Update(deltaTime);
                AudioManager.Cleanup();
                return;
            }

            // Обновление мира и UI
            if (WorldLoaded)
            {
                _world!.Update(deltaTime);
                UIManager.Update(deltaTime);
                AudioManager.Cleanup();
            }
        }

        /// <summary>
        /// Отрисовывает игру.
        /// </summary>
        /// <param name="target">Цель отрисовки.</param>
        /// <param name="states">Состояния рендера.</param>
        public void Draw(RenderTarget target, RenderStates states)
        {
            // Если окно не в фокусе, ничего не рисуем
            if (!Window.HasFocus())
                return;

            // Настройка камеры
            var cameraCenterPosition = _cameraPosition - (Vector2f)Window.Size / 2;
            var view = new View(new FloatRect(cameraCenterPosition, (Vector2f)Window.Size));

            // Управление масштабом камеры
            if (Keyboard.IsKeyPressed(Keyboard.Key.R))
            {
                _zoom = 0.5f;
            }
            else if (Keyboard.IsKeyPressed(Keyboard.Key.Q))
            {
                _zoom = 2;
            }
            else
            {
                _zoom = 1f;
            }

            view.Zoom(_zoom);
            Window.SetView(view);

            // Обновляем позицию мыши
            _mousePos = (Vector2f)Mouse.GetPosition(Window);

            // Если игра на паузе или мир не загружен, рисуем только UI
            if (IsPaused || !WorldLoaded)
            {
                states.Transform *= Transform;
                UIManager.Draw(target, states);
                DebugRender.Draw(target, states);
                return;
            }

            // Отрисовка мира, UI и отладочной информации
            if (WorldLoaded)
            {
                _world!.Draw(target, states);
                DebugRender.Draw(target, states);

                states.Transform *= Transform;
                UIManager.Draw(target, states);
            }
        }

        /// <summary>
        /// Закрывает игру.
        /// </summary>
        public static void Close()
        {
            AudioManager.Dispose();
            Window.Close();
        }

        /// <summary>
        /// Создаёт новый игровой мир.
        /// </summary>
        public static void CreateWorld()
        {
            _world = WorldGenerator.GenerateWorld(600, 500);
            _world.GenColliders();
        }

        /// <summary>
        /// Устанавливает позицию камеры.
        /// </summary>
        /// <param name="position">Позиция камеры.</param>
        public static void SetCameraPosition(Vector2f position)
        {
            _cameraPosition = position;
        }

        /// <summary>
        /// Возвращает текущий уровень масштабирования камеры.
        /// </summary>
        public static float GetZoom() => _zoom;

        /// <summary>
        /// Устанавливает уровень масштабирования камеры.
        /// </summary>
        public static void SetZoom(float zoom) => _zoom = zoom;

        /// <summary>
        /// Возвращает размер окна с учётом масштаба.
        /// </summary>
        public static Vector2f GetWindowSize() => (Vector2f)_windowSize * _zoom;

        /// <summary>
        /// Возвращает размер окна с учётом масштаба.
        /// </summary>
        public static Vector2f GetWindowSizeWithZoom() => (Vector2f)_windowSize * _zoom;

        /// <summary>  
        /// Возвращает текущую позицию мыши в координатах окна.  
        /// </summary>  
        /// <returns>Позиция мыши в координатах окна, масштабированная на текущий уровень масштабирования.</returns>  
        public static Vector2f GetMousePosition() => _mousePos * _zoom;

        /// <summary>
        /// Возвращает позицию мыши в мировых координатах.
        /// </summary>
        public static Vector2f GetMousePositionByWorld() => (_mousePos * GetZoom()) + (_cameraPosition - GetWindowSizeWithZoom() / 2);

        /// <summary>
        /// Возвращает текущую позицию камеры.
        /// </summary>
        public static Vector2f GetCameraPosition() => _cameraPosition;

        /// <summary>
        /// Возвращает центр экрана в мировых координатах.
        /// </summary>
        public static Vector2f GetCenterScreenByWorld() => _cameraPosition - (Vector2f)_windowSize / 2;
    }
}