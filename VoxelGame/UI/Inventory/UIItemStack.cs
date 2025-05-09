using SFML.Graphics;
using SFML.System;
using SFML.Window;
using VoxelGame.Graphics;
using VoxelGame.Item;
using VoxelGame.Resources;

namespace VoxelGame.UI.Inventory
{
    public class UIItemStack : UIBase
    {
        private Text _text;

        private int _itemCount;

        public Item.Item Item;

        public ItemType ItemType => Item.Type;

        public int ItemCount
        {
            get => _itemCount;
            set
            {
                if (value < Item.MaxCoutnInStack + 1)
                {
                    _itemCount = value;
                }

                if (value == 0)
                {
                    var perent = Perent as UIInventoryCell;
                    if (perent != null)
                    {
                        perent.RemoveChild(this);
                        perent.ItemStack = null;
                    }
                }
            }
        }

        public bool IsFull => _itemCount >= Item.MaxCoutnInStack;

        public UIItemStack(Item.Item infoItem)
        {
            Item = infoItem;

            SpriteSheet spriteSheet;

            switch(infoItem.Type)
            {
                case ItemType.Tile:
                    spriteSheet = AssetManager.GetSpriteSheet("terrain");
                    break;
                default:
                    spriteSheet = AssetManager.GetSpriteSheet("items");
                    break;
            }

            rect = new RectangleShape(new Vector2f(48, 48));
            rect.Texture = spriteSheet.Texture;
            rect.TextureRect = spriteSheet.GetTextureRect(infoItem.SpriteIndex);
            rect.Origin = Size;
            rect.Position = Size / 2;

            _text = new Text(ItemCount.ToString(), AssetManager.GetFont("Arial"));
            _text.Position = rect.Position - new Vector2f(30, 20);
            _text.FillColor = Color.White;

            ItemCount = 1;

            CanDrag = true;
            CanDrop = true;
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            _text.DisplayedString = ItemCount.ToString();
        }

        public override void UpdateOver(float deltaTime)
        {
            if(IsHovered)
            {
                DebugRender.AddText(UIManager.MousePosition - new Vector2f(0, 150), $"{Item.Name}\n" +
                                                             $"{Item.Description}\n" +
                                                             $"{Item.Speed}" +
                                                             $"{Item.Damage}");

                if (UIManager.Drag == null)
                {
                    if (Mouse.IsButtonPressed(Mouse.Button.Right))
                    {
                        if (ItemCount > 1)
                        {
                            UIManager.Drag = new UIItemStack(Item) { ItemCount = this.ItemCount / 2 };
                            ItemCount = ItemCount / 2;

                        }
                        else
                        {
                            UIManager.Drag = this;
                            OnDrag();
                        }
                    }
                }
            }

            base.UpdateOver(deltaTime);
        }

        public override void Draw(RenderTarget target, RenderStates states)
        {
            base.Draw(target, states);

            states.Transform *= Transform;

            if (Item.MaxCoutnInStack != 1)
                target.Draw(_text, states);
        }

        public override void OnDrag()
        {
            if (Perent != null && Perent is UIInventoryCell)
                (Perent as UIInventoryCell).ItemStack = null;

            base.OnDrag();
        }

        public override void OnDrop(UIBase ui)
        {
            if (ui is UIItemStack)
            {
                var itemSrc = ui as UIItemStack;
                var itemDest = this;

                if (itemSrc.Item == itemDest.Item && !itemDest.IsFull && itemDest.ItemCount + itemSrc.ItemCount < itemDest.Item.MaxCoutnInStack)
                    itemDest.ItemCount += itemSrc.ItemCount;
                else
                    ui.OnCancelDrag();
            }
        }
    }
}
