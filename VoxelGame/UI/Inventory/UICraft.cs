using SFML.Graphics;
using SFML.System;
using SFML.Window;
using VoxelGame.Item;
using VoxelGame.Worlds;

namespace VoxelGame.UI.Inventory
{
    public class UICraft : UIBase
    {
        private UIInventory _inventory;

        private List<Craft> crafts;

        public UICraft(UIInventory inventory)
        {
            _inventory = inventory;

            rect = new RectangleShape(new Vector2f(5, 8) * UIInventoryCell.CellSize);

            for (int x = 0; x < 5; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    Childs.Add(new UICraftCell() { Position = new Vector2f(x, y) * UIInventoryCell.CellSize, Perent = this } );
                }
            }
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            if (_inventory != null)
            {
                crafts = _inventory.GetAvailableCrafts(CraftTool.None);
            }
            if (!Mouse.IsButtonPressed(Mouse.Button.Left))
            {
                foreach (var c in Childs)
                {
                    (c as UICraftCell).SetItem(null);
                }

                for (int i = 0; i < crafts.Count; i++)
                {
                    (Childs[i] as UICraftCell).SetCraft(crafts[i]);
                }
            }
        }

        public Item.Item? CraftItem(ItemList itemList)
        {
            var item = Items.GetItem(itemList);
            if (item == null)
                return null;

            var craft = item.Craft;
            if (craft == null)
                return null;

            if (_inventory.AddItem(item, craft.OutCount))
            {
                for (int i = 0; i < craft.Items.Length; i++)
                {
                    var itemStack = _inventory.GetItemStackByItem(craft.Items[i].Item);
                    if (itemStack != null)
                        itemStack.ItemCount -= craft.Items[i].Count;
                }

                return item;
            }

            return null;
        }

        public Item.Item? CraftItem(Item.Item item)
        {
            if (item == null)
                return null;

            var craft = item.Craft;
            if (craft == null)
                return null;

            if (_inventory.AddItem(item, craft.OutCount))
            {
                for (int i = 0; i < craft.Items.Length; i++)
                {
                    var itemStack = _inventory.GetItemStackByItem(craft.Items[i].Item);
                    if (itemStack != null)
                        itemStack.ItemCount -= craft.Items[i].Count;
                }

                return item;
            }

            return null;
        }
    }
}
