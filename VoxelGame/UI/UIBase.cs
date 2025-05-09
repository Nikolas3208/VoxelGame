using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace VoxelGame.UI
{
    public abstract class UIBase : Transformable, Drawable
    {
        protected RectangleShape rect;

        protected List<UIBase> childs;

        public string StrId { get; private set; } = string.Empty;
        public string Name { get; set; } = nameof(UIBase);
        public UIBase? Perent { get; set; }
        public UIBase? OldPerent { get; set; }

        public bool IsVisible { get; set; } = true;
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

        public new Vector2f Origin
        {
            get => rect.Origin;
            set
            {
                rect.Origin = value;
                base.Origin = value;
            }
        }

        public Vector2f DragOffset { get; private set; }

        protected UIBase()
        {
            rect = new RectangleShape();
            childs = new List<UIBase>();

            StrId = Guid.NewGuid().ToString();
        }

        public Vector2f GetGlobalPosition()
        {
            if (Perent != null)
            {
                return Perent.GetGlobalPosition() + Position;
            }
            else
                return Position;
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
            if(!childs.Contains(child))
            {
                childs.Add(child);

                return true;
            }

            return false;
        } 

        public virtual UIBase? GetChild(string strId)
        {
            return childs.Find(c => c.StrId == strId);
        }

        public virtual bool RemoveChild(UIBase child)
        {
            return childs.Remove(child);
        }

        public FloatRect GetFloatRect() => new FloatRect(GetGlobalPosition() - rect.Origin + rect.Position, Size);

        public virtual void Update(float deltaTime)
        {
            foreach (var c in childs)
                c.Update(deltaTime);
        }

        public virtual void UpdateOver(float deltaTime)
        {
            IsHovered = GetFloatRect().Contains(UIManager.MousePosition);

            if (IsHovered)
            {
                if (UIManager.Drag == null)
                {
                    if (CanDrag && Mouse.IsButtonPressed(Mouse.Button.Left))
                    {
                        UIManager.Drag = this;
                        DragOffset = UIManager.MousePosition - GetGlobalPosition();
                        OnDrag();
                    }
                }

                if(UIManager.Drag != this && UIManager.Drag != null)
                {
                    UIManager.Drop = this;
                }

                for(int i = 0; i < childs.Count; i++)
                    childs[i]?.UpdateOver(deltaTime);
            }
        }

        public virtual void Draw(RenderTarget target, RenderStates states)
        {
            if(!IsVisible) 
                return;

            states.Transform *= Transform;

            target.Draw(rect, states);

            foreach (var child in childs)
            {
                child.Draw(target, states);
            }
        }
    }
}
