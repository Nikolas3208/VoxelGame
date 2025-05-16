using SFML.System;
using SFML.Window;
using VoxelGame.Meths;
using VoxelGame.UI;
using VoxelGame.UI.Inventory;

namespace VoxelGame.Worlds.Tile
{
    public class ChestEntity : TileEntity
    {
        private UIChestInventory _chestInventory;

        public bool IsOpen { get; set; } = false;
        public ChestEntity() : base(TileEntityType.Chest)
        {
            _chestInventory = new UIChestInventory(new Vector2f(UIInventoryCell.CellSize * 10, UIInventoryCell.CellSize * 6));
            _chestInventory.Position = Position;
            _chestInventory.SetSelectedCell(null);
            _chestInventory.Craft = null;
        }

        public void OpenChest()
        {
            IsOpen = true;

            _chestInventory.Position = Game.GetCameraPosition() - new Vector2f(_chestInventory.Size.X / 2 - UIInventoryCell.CellSize - 8, 0);
            _chestInventory.ShowInventory();
            UIManager.AddWindow(_chestInventory);
        }
        public void CloseChest()
        { 
            IsOpen = false;

            UIManager.RemoveWindow(_chestInventory);
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            if(IsOpen && Keyboard.IsKeyPressed(Keyboard.Key.E))
            {
                CloseChest();
            }

            if(MathHelper.DistanceSquared(Game.GetCameraPosition(), Position + Chunk!.Position) > 25000)
            {
                CloseChest();
            }
        }

        public override void Use()
        {
            OpenChest();
        }
    }
}
