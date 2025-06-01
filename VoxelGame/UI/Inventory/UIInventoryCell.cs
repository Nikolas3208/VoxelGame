using SFML.Graphics;
using SFML.System;
using VoxelGame.Resources;

namespace VoxelGame.UI.Inventory
{
    public class UIInventoryCell : UIBase
    {
        public const int CellSize = 32;

        private bool _isSelected = false;

        private UIItemStack? _itemStack;
        public UIItemStack? ItemStack
        {
            get => _itemStack;
            set
            {
                if(_itemStack != null && value != null && _itemStack.Item.ItemList == value.Item.ItemList)
                {
                    _itemStack.ItemCount += value.ItemCount;

                    if (_itemStack.ItemCount < 0)
                    {
                        _itemStack = null;
                    }
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

        public bool IsSelected 
        { 
            get => _isSelected;
            set
            {
                _isSelected = value;

                rect.FillColor = value ? new Color(255, 255, 0, 255) : new Color(100, 100, 255, 127);
            }
        }

        public UIInventoryCell()
        {
            rect = new RectangleShape(new Vector2f(CellSize, CellSize));
            rect.Texture = TextureManager.GetTexture("Inventory_Back");
            rect.FillColor = new Color(100, 100, 255, 127);

            rect.Origin = Size / 2;

            CanDrop = true;
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
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
