using SFML.Graphics;

namespace VoxelGame.UI
{
    public class UIWindow : UIBase
    {
        protected List<UIBase> childs;

        public UIWindow()
        {
            childs = new List<UIBase>();
        }

        public void AddChild(UIBase child)
        {
            childs.Add(child);
            child.Perent = this;
        }

        public UIBase? GetChildByStrId(string strId)
        {
            return childs.Find(c => c.StrId == strId);
        }

        public T GetChildByStrId<T>(string strId) where T : UIBase
        {
            return (T)childs.Find(c => c.StrId == strId)!;
        }

        public T GetChild<T>() where T : UIBase
        {
            return (T)childs.OfType<T>();
        }

        public bool RemoveChild(UIBase child)
        {
            if (childs.Contains(child))
            {
                child.Perent = null;
                return childs.Remove(child);
            }

            return false;
        }

        public override void Update(float deltaTime)
        {
            foreach (var child in childs)
                child.Update(deltaTime);
        }

        public override void Draw(RenderTarget target, RenderStates states)
        {
            states.Transform *= Transform;

            foreach (var child in childs)
                child.Draw(target, states);
        }
    }
}
