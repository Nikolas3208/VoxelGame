using SFML.Graphics;

namespace VoxelGame.UI
{
    public static class UIManager
    {
        private static List<UIWindow> _uIWindows = new();

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
            foreach (var window in _uIWindows)
                window.Update(deltaTime);
        }

        public static void Draw(RenderTarget target, RenderStates states)
        {
            foreach (var window in _uIWindows)
                target.Draw(window, states);
        }
    }
}
