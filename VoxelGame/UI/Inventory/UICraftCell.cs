using SFML.Graphics;
using SFML.System;
using SFML.Window;
using VoxelGame.Graphics;
using VoxelGame.Item;
using VoxelGame.Resources;

namespace VoxelGame.UI.Inventory
{
    public class UICraftCell : UIBase
    {
        private Sprite _itemSprite { get; set; }

        public Item.Item? Item { get; set; }
        public UICraftCell()
        {
            rect = new RectangleShape(new Vector2f(UIInventoryCell.CellSize, UIInventoryCell.CellSize));
            rect.Texture = AssetManager.GetTexture("Inventory_Back");
        }

        public void SetItem(Item.Item item)
        {
            Item = item;

            if (Item == null)
            {
                _itemSprite = null;
                return;
            }

            _itemSprite = new Sprite(AssetManager.GetTexture(Item.SpriteName));
            _itemSprite.Origin = -new Vector2f(8, 8);
        }

        public void SetCraft(Craft craft)
        {
            var item = Items.GetItem(craft.OutCraft);
            item.SetCraft(craft);

            SetItem(item);
        }

        public bool isMouseClick = false;

        public override void UpdateOver(float deltaTime)
        {
            base.UpdateOver(deltaTime);

            if(IsHovered)
            {
                if(Item != null)
                {
                    if (Item.Craft != null)
                    {
                        var craft = Item.Craft;

                        for (int i = 0; i < craft.Items.Length; i++)
                        {
                            var item = craft.Items[i];

                            DebugRender.AddText(UIManager.MousePosition + new Vector2f(0, i - 1) * 30, "Предмет для крафта: " + item.Item.ToString());
                            DebugRender.AddText(UIManager.MousePosition + new Vector2f(0, i + 1) * 30, "Количество предмета: " + item.Count.ToString());
                        }
                        DebugRender.AddText(UIManager.MousePosition + new Vector2f(0, craft.Items.Length + 1) * 30, "Предмет на выходе: " + Item.Name);
                        DebugRender.AddText(UIManager.MousePosition + new Vector2f(0, craft.Items.Length + 2) * 30, "Количество предмета на выходе: " + craft.OutCount.ToString());
                    }
                }
                if (Mouse.IsButtonPressed(Mouse.Button.Left) && !isMouseClick)
                {
                    isMouseClick = true;
                    if (Item != null)
                        (Perent as UICraft).CraftItem(Item);
                }
            }

            if (!Mouse.IsButtonPressed(Mouse.Button.Left))
            {
                isMouseClick = false;
            }
        }

        public override void Draw(RenderTarget target, RenderStates states)
        {
            base.Draw(target, states);

            states.Transform *= Transform;

            if (_itemSprite != null)
                target.Draw(_itemSprite, states);
        }
    }
}
