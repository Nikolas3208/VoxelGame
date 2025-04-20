using SFML.Graphics;

namespace VoxelGame.UI.Inventory
{
    public class UIInventoryCell : UIBase
    {
        public UIItemStack? ItemStack { get; set; }

        public UIInventoryCell()
        {

        }

        public override void Update(float deltaTime)
        {
            if (ItemStack != null)
            {
                ItemStack.Update(deltaTime);
            }
        }

        public override void Draw(RenderTarget target, RenderStates states)
        {
            if (ItemStack != null)
            {
                ItemStack.Draw(target, states);
            }
        }

        public void SetItemStack(UIItemStack itemStack)
        {
            ItemStack = itemStack;
        }

        public UIItemStack GetItemStack()
        {
            return ItemStack!;
        }
    }
}
