using SFML.Graphics;
using VoxelGame.Graphics;

namespace VoxelGame.Resources
{
    public static class AssetManager
    {
        private static Dictionary<string, Texture> _textures = new Dictionary<string, Texture>();
        private static Dictionary<string, Font> _fonts = new Dictionary<string, Font>();

        public static string BasePath { get; set; } = "Assets/";

        public static Texture GetTexture(string name)
        {
            if (_textures.ContainsKey(name))
            {
                return _textures[name];
            }
            else
            {
                if (!File.Exists(BasePath + "Textures/" + name + ".png"))
                {
                    throw new FileNotFoundException("Texture not found: " + BasePath + "Textures/" + name + ".png");
                }

                Texture texture = new Texture(BasePath + "Textures/" + name + ".png");
                _textures.Add(name, texture);
                return texture;
            }
        }

        public static SpriteSheet GetSpriteSheet(string name, int tileSize = 16, bool abIsCount = false, int borderSize = 0)
        {
            Texture texture = GetTexture(name);

            return new SpriteSheet(tileSize, tileSize, abIsCount, borderSize, texture);
        }

        private static Font font = new Font("arial.ttf");

        public static Font GetFont(string name)
        {
            return font;
        }
    }
}
