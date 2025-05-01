namespace VoxelGame.Worlds.Tile
{
    public class ChestEntity : TileEntity
    {
        public bool IsOpen { get; set; } = false;
        public ChestEntity(TileType type) : base(type)
        {
        }

        public override void Use()
        {
            IsOpen = !IsOpen;

            if(IsOpen)
            {

            }
        }
    }
}
