﻿using SFML.Graphics;
using SFML.System;
using SFML.Window;
using VoxelGame.Item;
using VoxelGame.Resources;

namespace VoxelGame.UI.Inventory
{
    public class UIItemStack : UIBase
    {
        public const int ItemStakSize = 16;

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

            var texture = TextureManager.GetTexture(Item.SpriteName);

            float scale = 1.0f;

            var size = new Vector2f(texture.Size.X, texture.Size.Y) / ItemStakSize;
            if(size.X > size.Y)
                scale = size.X;
            else
                scale = size.Y;

            rect = new RectangleShape((Vector2f)texture.Size / scale);
            rect.Texture = texture;
            rect.Origin = Size;
            rect.Position = Size / 2;

            _text = new Text(ItemCount.ToString(), TextureManager.GetFont("Arial"));
            _text.Position = rect.Position - new Vector2f(30, 20);
            _text.FillColor = Color.White;
            _text.CharacterSize = 16;

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
            if (IsHovered)
            {
                DebugRender.AddText(UIManager.MousePosition - new Vector2f(0, 150), $"{Item.Name}\n" +
                                                             $"Описание: {Item.Description}\n" +
                                                             $"Скорость: {Item.Speed}\n" +
                                                             $"Урон: {Item.Damage}");

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
