using SFML.Window;

namespace VoxelGame
{
    public class Program
    {
        public static void Main()
        {
            Game game = new Game(new WindowSettings { Title = "VoxelGame", VideoMode = VideoMode.DesktopMode, Styles = Styles.Default });
            game.Run();
        }
    }
}