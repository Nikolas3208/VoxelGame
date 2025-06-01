using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace VoxelGame.UI
{
    public abstract class UIBase : Transformable, Drawable
    {
        public delegate void UIHovered(bool value);
        public event UIHovered OnHovered;

        protected RectangleShape rect;

        public List<UIBase> Childs;

        public string StrId { get; set; } = string.Empty;
        public string Name { get; set; } = nameof(UIBase);
        public UIBase? Perent { get; set; }
        public UIBase? OldPerent { get; set; }

        public bool IsVisible { get; set; } = true;
        public bool IsRectVisible { get; set; } = true;
        public bool IsUpdate { get; set; } = true;

        public bool CanDrag { get; set; } = false;
        public bool CanDrop { get; set; } = false;
        public bool IsHovered { get; set; } = false;

        public Vector2f Size
        {
            get => rect.Size;
            set
            {
                rect.Size = value;
            }
        }

        public Vector2f GlobalPosition
        {
            get => Perent == null ? Position : Perent.GlobalPosition + Position;
        }

        public Vector2f GlobalOrigin
        {
            get => Perent == null ? Origin : Perent.GlobalOrigin + Origin;
        }

        public Vector2f DragOffset { get; private set; }

        protected UIBase()
        {
            rect = new RectangleShape();
            Childs = new List<UIBase>();

            StrId = Guid.NewGuid().ToString();
        }

        public virtual void OnDrag()
        {
            if (Perent != null)
            {
                OldPerent = Perent;
                Perent.RemoveChild(this);
                Perent = null;
            }
        }

        public virtual void OnDrop(UIBase drop)
        {
        }

        public virtual void OnCancelDrag()
        {
            if (OldPerent != null)
                OldPerent.AddChild(this);

            Perent = OldPerent;
            Position = new Vector2f();
        }


        public virtual bool AddChild(UIBase child)
        {
            if (!Childs.Contains(child))
            {
                Childs.Add(child);

                return true;
            }

            return false;
        }

        public virtual UIBase? GetChild(string strId)
        {
            return Childs.Find(c => c.StrId == strId);
        }

        public virtual bool RemoveChild(UIBase child)
        {
            return Childs.Remove(child);
        }

        public virtual void Update(float deltaTime)
        {
            foreach (var c in Childs)
                c.Update(deltaTime);
        }

        public virtual void UpdateOver(float deltaTime)
        {
            IsHovered = GetFloatRect().Contains(UIManager.MousePosition);

            OnHovered?.Invoke(false);

            if (IsHovered)
            {
                OnHovered?.Invoke(true);

                if (UIManager.Drag == null)
                {
                    if (CanDrag && Mouse.IsButtonPressed(Mouse.Button.Left))
                    {
                        UIManager.Drag = this;
                        DragOffset = UIManager.MousePosition - GlobalPosition;
                        OnDrag();
                    }
                }

                if (UIManager.Drag != this && UIManager.Drag != null)
                {
                    UIManager.Drop = this;
                }

                for (int i = 0; i < Childs.Count; i++)
                    Childs[i]?.UpdateOver(deltaTime);
            }
        }

        public virtual void Draw(RenderTarget target, RenderStates states)
        {
            if (!IsVisible)
                return;

            states.Transform *= Transform;

            if (IsRectVisible)
                target.Draw(rect, states);

            foreach (var child in Childs)
            {
                child.Draw(target, states);
            }
        }

        public FloatRect GetFloatRect()
        {
            var localBounds = rect.GetGlobalBounds();
            return new FloatRect((localBounds.Position + GlobalPosition - GlobalOrigin + rect.Position - rect.Origin), Size);
        }
    }
}
