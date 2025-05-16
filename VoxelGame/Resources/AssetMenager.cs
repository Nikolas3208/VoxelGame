using SFML.Graphics;
using VoxelGame.Graphics;

namespace VoxelGame.Resources
{
    public static class AssetManager
    {
        /// <summary>
        /// Сортированный список текстур
        /// </summary>
        private static Dictionary<string, Texture> _textures = new Dictionary<string, Texture>();

        /// <summary>
        /// Сортированый список шрифтов
        /// </summary>
        private static Dictionary<string, Font> _fonts = new Dictionary<string, Font>();

        /// <summary>
        /// Базовый путь
        /// </summary>
        public static string BasePath { get; set; } = "Assets/";

        public static void Load()
        {
            var files = Directory.GetFiles(BasePath, "*.*", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                string name = Path.GetFileNameWithoutExtension(file);

                if (!_textures.ContainsKey(name))
                    _textures.Add(name, new Texture(file));
            }
        }

        /// <summary>
        /// Получить текстуру по имени
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException"></exception>
        public static Texture GetTexture(string name)
        {
            if (_textures.ContainsKey(name))
            {
                return _textures[name];
            }

            throw new Exception("File not found. " + name);
        }

        /// <summary>
        /// Получить спрайт лист по имени
        /// </summary>
        /// <param name="name"></param>
        /// <param name="tileSize"> Размер одного спрайта </param>
        /// <param name="abIsCount"> размер плитки это поличество плиток по ширине и высоте </param>
        /// <param name="borderSize"> Расстояние между спрайтами </param>
        /// <returns></returns>
        public static SpriteSheet GetSpriteSheet(string name, int tileSize = 16, bool abIsCount = false, int borderSize = 0)
        {
            Texture texture = GetTexture(name);

            return new SpriteSheet(tileSize, tileSize, abIsCount, borderSize, texture);
        }

        private static Font font = new Font("arial.ttf");

        /// <summary>
        /// Получить фон по имени
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Font GetFont(string name)
        {
            return font;
        }
    }
}
