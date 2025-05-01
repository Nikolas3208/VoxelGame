using SFML.Graphics;
using SFML.System;
using VoxelGame.Resources;

namespace VoxelGame.UI.Inventory
{
    public class UIInventoryCell : UIBase
    {
        private Sprite _sprite;

        public UIItemStack? ItemStack { get; set; }

        public bool IsSelected { get; set; } = false;

        public UIInventoryCell()
        {
            _sprite = new Sprite(AssetManager.GetTexture("ui/inventory_cell"));
            _sprite.Scale = new Vector2f(0.1f, 0.1f);
            _sprite.Origin = new Vector2f(80, 80);
        }

        public override void Update(float deltaTime)
        {
            if (ItemStack != null)
            {
                ItemStack.Update(deltaTime);
            }

            if(IsSelected)
            {
                _sprite.Scale = new Vector2f(0.11f, 0.11f);
                _sprite.Origin = new Vector2f(88, 88);
            }
            else
            {
                _sprite.Scale = new Vector2f(0.1f, 0.1f);
            }
        }

        public override void Draw(RenderTarget target, RenderStates states)
        {
            states.Transform *= Transform;

            target.Draw(_sprite, states);

            if (ItemStack != null)
            {
                ItemStack.Draw(target, states);
            }
        }

        public void SetItemStack(UIItemStack itemStack)
        {
            itemStack.Perent = this;
            ItemStack = itemStack;
        }

        public UIItemStack GetItemStack()
        {
            return ItemStack!;
        }

        public Item.Item? GetItem()
        {
            return ItemStack?.Item ?? null;
        }
    }
}
