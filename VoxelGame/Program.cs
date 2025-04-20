using SFML.Window;

namespace VoxelGame
{
    public class Program
    {
        public static void Main()
        {
            Game game = new Game(VideoMode.DesktopMode, "Voxel Game");
            game.Run();
        }
    }
}