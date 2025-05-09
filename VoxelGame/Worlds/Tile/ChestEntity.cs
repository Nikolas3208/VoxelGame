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
            _chestInventory = new UIChestInventory(new Vector2f(800, 600));
            _chestInventory.Position = Position;
        }

        public void OpenChest()
        {
            IsOpen = true;

            _chestInventory.Position = Position + Chunk!.Position;
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
