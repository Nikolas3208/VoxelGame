﻿using SFML.System;
using VoxelGame.Item;

namespace VoxelGame.UI.Inventory
{
    public class UIInventory : UIWindow
    {
        private UIInventoryCell? _selectedCell;

        public bool IsFullInventoryVisible { get; private set; } = false;

        public UIInventory(Vector2f size) : base(size, "Player Inventory")
        {
            TitleBarIsVisible = false;

            for (int i = 0; i < 10; i++)
            {
                childs.Add(new UIInventoryCell() { Position = new Vector2f(i * 80, 0), Perent = this });
            }

            SetSelectedCell((UIInventoryCell)childs[0]);

            for (int x = 0; x < 10; x++) 
            {
                for (int y = 1; y < 6; y++)
                {
                    childs.Add(new UIInventoryCell() { Position = new Vector2f(x * 80, -y * 80), Perent = this, IsVisible = false, IsUpdate = false });
                }
            }
        }

        public void SetSelectedCell(int cellIndex)
        {
            if (cellIndex < 0 || cellIndex > 10)
                return;

            SetSelectedCell((UIInventoryCell)childs[cellIndex]);
        }

        public void SetSelectedCell(UIInventoryCell inventoryCell)
        {
            if (_selectedCell != null)
                _selectedCell.IsSelected = false;

            inventoryCell.IsSelected = true;
            _selectedCell = inventoryCell;
        }

        public bool AddItem(DropItem item)
        {
            return AddItem(item.Item, item.ItemCount);
        }

        public bool AddItem(Item.Item infoItem, int count = 1)
        {
            return AddItem(new UIItemStack(infoItem), count);
        }

        public bool AddItem(UIItemStack itemStack, int count = 1)
        {
            var cell = GetSuitableCell(itemStack, count);

            if (cell != null && cell.ItemStack != null)
            {
                cell.ItemStack.ItemCount += count;
            }
            else
            {
                cell = GetEmptyCell();

                if (cell == null)
                    return false;

                itemStack.ItemCount = count;

                cell.ItemStack = itemStack;
            }

            return true;
        }

        public Item.Item? GetItemWithSelectedCell()
        {
            return _selectedCell.GetItem();
        }

        public Item.Item? CraftItem(ItemList itemList)
        {
            var item = Items.GetItem(itemList);
            if (item == null)
                return null;

            var craft = item.GetCraft();
            if (craft == null)
                return null;

            for(int i = 0; i < craft.Items.Length; i++)
            {
                var itemCraft = craft.Items[i];
                bool contains = false;
                foreach (var cell in childs.Select(c => c as UIInventoryCell))
                {
                    var cellItem = cell!.GetItem();

                    if (cellItem == null)
                        continue;

                    if (cellItem!.ItemList == itemCraft.Item && cell.ItemStack!.ItemCount >= itemCraft.Count)
                    {
                        contains = true;
                        break;
                    }
                }

                if (!contains)
                    return null;
            }



            if (AddItem(item, craft.OutCount))
            {
                for (int i = 0; i < craft.Items.Length; i++)
                {
                    var itemCraft = craft.Items[i];
                    foreach (var cell in childs.Select(c => c as UIInventoryCell))
                    {
                        var cellItem = cell!.GetItem();

                        if (cellItem == null)
                            continue;

                        if (cellItem!.ItemList == itemCraft.Item && cell.ItemStack!.ItemCount >= itemCraft.Count)
                        {
                            cell.ItemStack.ItemCount -= itemCraft.Count;
                            break;
                        }
                    }
                }

                return item;
            }

            return null;
        }

        private UIInventoryCell? GetSuitableCell(UIItemStack itemStack, int count = 1)
        {
            var type = itemStack.ItemType;

            var cells = childs.Select(c => c as UIInventoryCell).ToList();

            var cellResult = cells.Find(cell => cell != null && cell.ItemStack != null
                   && cell.ItemStack.ItemType == type && cell.ItemStack.Item.Name == itemStack.Item.Name && !cell.ItemStack.IsFull);

            return cellResult;
        }

        private UIInventoryCell? GetEmptyCell()
        {
            var cells = childs.Select(c => c as UIInventoryCell).ToList();

            return cells.Find(cell => cell != null && cell.ItemStack == null);
        }

        public UIInventoryCell? GetSelectedCell()
        {
            return childs.Select(c => c as UIInventoryCell).ToList().Find(cell => cell!.IsSelected);
        }

        public void ShowInventory()
        {
            rect.Size = new Vector2f(Size.X, 550);
            rect.Origin = new Vector2f(40, 520);
            IsFullInventoryVisible = true;

            for (int x = 0; x < 10; x++)
            {
                for (int y = 0; y < 6; y++)
                {
                    int index = x + y * 10;

                    var child = childs[index];
                    child.IsVisible = true;
                    child.IsUpdate = true;
                }
            }
        }

        public void HideInventory()
        {
            IsFullInventoryVisible = false;
            rect.Size = new Vector2f(800, 100);
            rect.Origin = new Vector2f(40, 50);

            for (int x = 0; x < 10; x++)
            {
                for (int y = 1; y < 6; y++)
                {
                    int index = x + y * 10;

                    var child = childs[index];

                    child.IsVisible = false;
                    child.IsUpdate = false;
                }
            }
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
        }
    }
}
