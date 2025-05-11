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
