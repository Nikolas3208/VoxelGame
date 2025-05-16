using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System.Security.Cryptography.X509Certificates;
using VoxelGame.Item;
using VoxelGame.Physics;
using VoxelGame.Resources;

namespace VoxelGame.UI.Inventory
{
    public class UIInventory : UIWindow
    {
        private UIInventoryCell? _selectedCell;

        protected int CellCountX { get; set; } = 10;
        protected int CellCountY { get; set; } = 6;

        public Player Player { get; set; }
        public UICraft Craft {  get; set; }

        public bool IsFullInventoryVisible { get; private set; } = false;

        public UIInventory(Vector2f size) : base(size, "Player Inventory")
        {
            Craft = new UICraft(this);

            TitleBarIsVisible = false;
            IsRectVisible = false;

            for (int i = 0; i < CellCountX; i++)
            {
                Childs.Add(new UIInventoryCell() { Position = new Vector2f(i * UIInventoryCell.CellSize, 0), Perent = this });
            }

            SetSelectedCell((UIInventoryCell)Childs[0]);

            for (int x = 0; x < CellCountX; x++) 
            {
                for (int y = 1; y < CellCountY; y++)
                {
                    Childs.Add(new UIInventoryCell() { Position = new Vector2f(x * UIInventoryCell.CellSize, -y * UIInventoryCell.CellSize), Perent = this, IsVisible = false, IsUpdate = false });
                }
            }

            rect.Size = new Vector2f(Size.X, UIInventoryCell.CellSize);
            rect.Origin = new Vector2f(UIItemStack.ItemStakSize, UIItemStack.ItemStakSize);
        }

        public void SetSelectedCell(int cellIndex)
        {
            if (cellIndex < 0 || cellIndex > 10)
                return;

            SetSelectedCell((UIInventoryCell)Childs[cellIndex]);
        }

        public void SetSelectedCell(UIInventoryCell inventoryCell)
        {
            if (_selectedCell != null)
                _selectedCell.IsSelected = false;

            if (inventoryCell != null)
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

        public List<Craft> GetAvailableCrafts()
        {
            var crafts = new List<Craft>();
            bool breakCraft = false;

            for (int i = 1; i < Enum.GetNames<ItemList>().Length; i++)
            {
                var item = Items.GetItem((ItemList)i);

                if (item == null)
                    continue;

                var iCrafts = item.GetCrafts();

                if (iCrafts == null)
                    continue;

                breakCraft = false;

                foreach (var craft in iCrafts)
                {
                    for(int i2 = 0; i2 < craft.Items.Length; i2++)
                    {
                        var itemC = craft.Items[i2];

                        if (GetItemCount(itemC.Item) < itemC.Count || !CanTool(Player.CraftTools, craft.Tool))
                        {
                            break;
                        }

                        if(i2 == craft.Items.Length - 1)
                        {
                            crafts.Add(craft);
                        }
                    }
                }
            }

            return crafts;
        }

        bool CanTool(List<CraftTool> a, CraftTool b)
        {
            foreach (var tool in a)
            {
                if((tool & b) != 0 && (b & tool) != 0) return true;
            }

            return false;
        }

        public int GetItemCount(ItemList item)
        {
            var itemStack = GetItemStackByItem(item);

            if (itemStack == null) return 0;

            return itemStack.ItemCount;
        }

        public UIItemStack? GetItemStackByItem(ItemList item)
        {
            var cell = GetCellByItem(item);

            if(cell == null) return null;

            return cell.ItemStack;
        }

        public UIInventoryCell? GetCellByItem(ItemList item)
        {
            return Childs.Select(c => c as UIInventoryCell).ToList()
                .Find(c => c!.GetItem() != null && c!.GetItem()!.ItemList == item);
        }

        private UIInventoryCell? GetSuitableCell(UIItemStack itemStack, int count = 1)
        {
            var type = itemStack.ItemType;

            var cells = Childs.Select(c => c as UIInventoryCell).ToList();

            var cellResult = cells.Find(cell => cell != null && cell.ItemStack != null
                   && cell.ItemStack.ItemType == type && cell.ItemStack.Item.ItemList == itemStack.Item.ItemList && !cell.ItemStack.IsFull);

            return cellResult;
        }

        private UIInventoryCell? GetEmptyCell()
        {
            var cells = Childs.Select(c => c as UIInventoryCell).ToList();

            return cells.Find(cell => cell != null && cell.ItemStack == null);
        }

        public UIInventoryCell? GetSelectedCell()
        {
            return Childs.Select(c => c as UIInventoryCell).ToList().Find(cell => cell!.IsSelected);
        }

        public void ShowInventory()
        {
            rect.Size = new Vector2f(Size.X, UIInventoryCell.CellSize * CellCountY);
            rect.Origin = new Vector2f(UIItemStack.ItemStakSize, UIInventoryCell.CellSize * CellCountY - UIInventoryCell.CellSize / 2);

            IsFullInventoryVisible = true;

            for (int x = 0; x < CellCountX; x++)
            {
                for (int y = 0; y < CellCountY; y++)
                {
                    int index = x + y * CellCountX;

                    var child = Childs[index];
                    child.IsVisible = true;
                    child.IsUpdate = true;
                    child.CanDrop = true;
                }
            }
        }

        public void HideInventory()
        {
            IsFullInventoryVisible = false;

            rect.Size = new Vector2f(Size.X, UIInventoryCell.CellSize);
            rect.Origin = new Vector2f(UIItemStack.ItemStakSize, UIItemStack.ItemStakSize);

            for (int x = 0; x < CellCountX; x++)
            {
                for (int y = 1; y < CellCountY; y++)
                {
                    int index = x + y * CellCountX;

                    var child = Childs[index];

                    child.IsVisible = false;
                    child.IsUpdate = false;
                    child.CanDrag = false;
                    child.CanDrop = false;
                }
            }
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            if (Craft != null)
                Craft.UpdateOver(deltaTime);

            if (IsFullInventoryVisible && Craft != null)
            {
                Craft.Position = new Vector2f(-Game.GetWindowSize().X / 2 * Game.GetZoom() + 140, -Game.GetWindowSize().Y / 2 * Game.GetZoom() - 80)  + Position;
                Craft.Update(deltaTime);
            }
        }

        public override void UpdateOver(float deltaTime)
        {
            base.UpdateOver(deltaTime);

            if (!IsFullInventoryVisible && IsHovered && Mouse.IsButtonPressed(Mouse.Button.Left))
            {
                var select = (UIInventoryCell)Childs.Find(c => c.IsHovered);
                if (select != null)
                {
                    SetSelectedCell(select);
                }
            }
        }

        public override void Draw(RenderTarget target, RenderStates states)
        {
            base.Draw(target, states);

            if (IsFullInventoryVisible && Craft != null)
                Craft.Draw(target, states);
        }
    }
}
