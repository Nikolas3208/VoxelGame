using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace VoxelGame.UI
{
    public static class UIManager
    {
        private static List<UIWindow> _uIWindows = new();

        public static Vector2f MousePosition;

        public static UIBase? Drag = null;
        public static UIBase? Drop = null;
        public static UIBase? Taken = null;


        public static void AddWindow(UIWindow window)
        {
            _uIWindows.Add(window);
        }

        public static void RemoveWindow(UIWindow window)
        {
            _uIWindows.Remove(window);
        }

        public static UIWindow? GetWindowByStrId(string strId)
        {
            return _uIWindows.Find(w => w.StrId == strId);
        }

        public static void Update(float deltaTime)
        {
            Drop = null;

            foreach(var window in _uIWindows)
            {
                window.UpdateOver(deltaTime);
            }

            MousePosition = Game.GetMousePositionByWorld();

            if (Drag != null)
            {
                if (Mouse.IsButtonPressed(Mouse.Button.Left) || Mouse.IsButtonPressed(Mouse.Button.Right))
                {
                    Drag.Position = MousePosition - Drag.DragOffset;
                }
                else
                {
                    if (Drop != null)
                    {
                        Drop.OnDrop(Drag);
                    }
                    else
                    {
                        Drag.OnCancelDrag();
                    }

                    Drag = null;
                }

            }

            foreach (var window in _uIWindows)
            {
                window.Update(deltaTime);
            }
        }

        public static void Draw(RenderTarget target, RenderStates states)
        {
            foreach (var window in _uIWindows)
            {
                if (window != Drag)
                    target.Draw(window, states);
            }

            if(Drag != null)
                target.Draw(Drag, states);
        }
    }
}
