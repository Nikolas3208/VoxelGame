using SFML.System;

namespace VoxelGame.Worlds.Tile
{
    public class DoorTile : InfoTile, IUsableTile
    {
        public DoorTile() : base(TileType.Door)
        {
            // Initialize door-specific properties if needed
            Size = new Vector2f(32, 64);
        }
        public void Use()
        {
            // Logic to open or close the door
            Console.WriteLine("Door used");
        }
    }
}
