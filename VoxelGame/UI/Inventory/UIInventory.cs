using VoxelGame.Item;

namespace VoxelGame.UI.Inventory
{
    public class UIInventory : UIWindow
    {
        public bool AddItem(InfoItem infoItem)
        {
            return AddItem(new UIItemStack(infoItem));
        }

        public bool AddItem(UIItemStack itemStack)
        {
            var cell = GetEmptyCellOrSuitable(itemStack.ItemType);

            if (cell == null)
                return false;

            cell.SetItemStack(itemStack);

            return true;
        }

        public void GetItem()
        {
            
        }

        private UIInventoryCell? GetEmptyCellOrSuitable(ItemType type)
        {
            var cells = childs.Select(c => c as UIInventoryCell).ToList();

             var cellResult = cells.Find(cell => cell != null && cell.ItemStack != null
                    && cell.ItemStack.ItemType == type && !cell.ItemStack.IsFull);

            if(cellResult == null)
                return cells.Find(cell => cell != null && cell.ItemStack == null);

            return cellResult;
        }
    }
}
