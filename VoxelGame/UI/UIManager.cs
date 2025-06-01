using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace VoxelGame.UI
{
    /// <summary>
    /// Класс для управления пользовательским интерфейсом (UI) в игре.
    /// Позволяет добавлять, удалять, обновлять и рисовать окна интерфейса.
    /// Также реализует функциональность перетаскивания и сброса элементов UI.
    /// </summary>
    public static class UIManager
    {
        /// <summary>
        /// Список всех окон интерфейса.
        /// </summary>
        private static List<UIWindow> _uIWindows = new();

        /// <summary>
        /// Текущая позиция мыши в игровом мире.
        /// </summary>
        public static Vector2f MousePosition;

        /// <summary>
        /// Масштаб интерфейса.
        /// </summary>
        public static Vector2f UIScale = new Vector2f(1f, 1f);

        /// <summary>
        /// Элемент интерфейса, который в данный момент перетаскивается.
        /// </summary>
        public static UIBase? Drag = null;

        /// <summary>
        /// Элемент интерфейса, на который можно сбросить перетаскиваемый элемент.
        /// </summary>
        public static UIBase? Drop = null;

        /// <summary>
        /// Элемент интерфейса, который был взят для перетаскивания.
        /// </summary>
        public static UIBase? Taken = null;

        /// <summary>
        /// Количество окон интерфейса.
        /// </summary>
        public static int WindowCount => _uIWindows.Count;

        /// <summary>
        /// Добавляет окно в список окон интерфейса.
        /// </summary>
        /// <param name="window">Окно интерфейса.</param>
        public static void AddWindow(UIWindow window)
        {
            if(!_uIWindows.Contains(window))
                _uIWindows.Add(window);
        }

        /// <summary>
        /// Удаляет окно из списка окон интерфейса.
        /// </summary>
        /// <param name="window">Окно интерфейса.</param>
        /// <returns>True, если окно было успешно удалено, иначе false.</returns>
        public static bool RemoveWindow(UIWindow window)
        {
            return _uIWindows.Remove(window);
        }

        /// <summary>
        /// Получает окно интерфейса по его строковому идентификатору.
        /// </summary>
        /// <param name="strId">Строковый идентификатор окна.</param>
        /// <returns>Окно интерфейса или null, если окно не найдено.</returns>
        public static UIWindow? GetWindowByStrId(string strId)
        {
            return _uIWindows.Find(w => w.StrId == strId);
        }

        /// <summary>
        /// Обновляет состояние всех окон интерфейса.
        /// Также обрабатывает логику перетаскивания и сброса элементов.
        /// </summary>
        /// <param name="deltaTime">Время между кадрами.</param>
        public static void Update(float deltaTime)
        {
            Drop = null; // Сбрасываем элемент, на который можно сбросить
            MousePosition = Game.GetMousePosition(); // Получаем текущую позицию мыши

            // Обновляем состояние окон при наведении
            foreach (var window in _uIWindows)
            {
                window.UpdateOver(deltaTime);
            }

            // Логика перетаскивания элемента
            if (Drag != null)
            {
                if (Mouse.IsButtonPressed(Mouse.Button.Left) || Mouse.IsButtonPressed(Mouse.Button.Right))
                {
                    // Перемещаем элемент вместе с мышью
                    Drag.Position = MousePosition - Drag.DragOffset;
                }
                else
                {
                    // Если кнопка мыши отпущена, завершаем перетаскивание
                    if (Drop != null)
                    {
                        Drop.OnDrop(Drag); // Сбрасываем элемент на целевой объект
                    }
                    else
                    {
                        Drag.OnCancelDrag(); // Отменяем перетаскивание
                    }

                    Drag = null; // Сбрасываем состояние перетаскивания
                }
            }

            // Обновляем состояние всех окон
            foreach (var window in _uIWindows)
            {
                window.Update(deltaTime);
            }
        }

        /// <summary>
        /// Рисует все окна интерфейса.
        /// Перетаскиваемый элемент рисуется поверх остальных окон.
        /// </summary>
        /// <param name="target">Цель отрисовки (например, окно).</param>
        /// <param name="states">Состояния рендера.</param>
        public static void Draw(RenderTarget target, RenderStates states)
        {
            foreach (var window in _uIWindows)
            {
                if (window != Drag) // Рисуем все окна, кроме перетаскиваемого
                {
                    window.Scale = UIScale; // Применяем масштаб интерфейса
                    target.Draw(window, states);
                }
            }

            // Рисуем перетаскиваемый элемент поверх остальных окон
            if (Drag != null)
            {
                Drag.Scale = UIScale;
                target.Draw(Drag, states);
            }
        }
    }
}