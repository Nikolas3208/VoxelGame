using SFML.System;
using VoxelGame.UI.Widgets;

namespace VoxelGame.UI.Menus
{
    public class UIMenu : UIWindow
    {

        public UIMenu(Vector2f size, string title) : base(size, title)
        {
            TitleBarIsVisible = false;
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
            // Add logic for updating the start menu, such as handling button clicks or animations
        }

        public virtual void Follows()
        {
            Game.Window.Resized += OnWindowResized;
        }

        public virtual void Unfollows()
        {
            Game.Window.Resized -= OnWindowResized;

            foreach (var widget in Childs)
            {
                (widget as UIWidget)?.Unfollows(Game.Window);
            }
        }

        private void OnWindowResized(object? sender, SFML.Window.SizeEventArgs e)
        {
            Size = new Vector2f(e.Width, e.Height);
            Origin = new Vector2f(e.Width, e.Height) / 2;
        }

        public void AddWidget(UIWidget widget)
        {
            Childs.Add(widget);
            widget.Perent = this;
            widget.OldPerent = this;
            widget.Follows(Game.Window);
        }

        public bool RemoveWidget(UIWidget widget)
        {
            if (Childs.Contains(widget))
            {
                Childs.Remove(widget);
                widget.Perent = null;
                widget.OldPerent = null;
                return true;
            }

            return false;
        }

        public UIWidget? GetWidget(string strId)
        {
            return Childs.Find(c => c.StrId == strId) as UIWidget;
        }
    }
}
