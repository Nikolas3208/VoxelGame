using SFML.Graphics;
using SFML.System;
using VoxelGame.Resources;

namespace VoxelGame.UI.Inventory
{
    public class UIInventoryCell : UIBase
    {
        private UIItemStack _itemStack;
        public UIItemStack? ItemStack
        {
            get => _itemStack;
            set
            {
                if(_itemStack != null && value != null && _itemStack.Item.ItemList == value.Item.ItemList)
                {
                    _itemStack.ItemCount += value.ItemCount;
                    return;
                }

                _itemStack = value;

                if(_itemStack != null)
                {
                    _itemStack.Perent = this;
                    _itemStack.Position = new Vector2f();
                    AddChild(_itemStack);
                }
            }
        }

        public bool IsSelected { get; set; } = false;

        public UIInventoryCell()
        {
            rect = new RectangleShape(new Vector2f(80, 80));
            rect.Texture = AssetManager.GetTexture("ui/inventory_cell");

            rect.Origin = Size / 2;

            CanDrop = true;
        }

        public override void OnDrop(UIBase drop)
        {
            base.OnDrop(drop);

            if (drop == null)
                return;

            if (drop is UIItemStack)
            {
                ItemStack = (UIItemStack)drop;
            }
        }

        public Item.Item? GetItem()
        {
            return ItemStack?.Item ?? null;
        }
    }
}
