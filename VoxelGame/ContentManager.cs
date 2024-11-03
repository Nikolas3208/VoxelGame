using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoxelGame.Worlds.Chunks.Tile;

namespace VoxelGame
{
    public class ContentManager
    {
        public string? contentPath;


        private static List<Texture> textures;

        public static void LoadContent(string path)
        {
            textures = new List<Texture>();

            int count = Directory.GetFiles(path + "Textures\\Tiles\\", "*", SearchOption.AllDirectories).Length;

            for (int i = 0; i < count; i++)
            {
                textures.Add(new Texture(path + $"Textures\\Tiles\\Tiles_{i}.png"));
            }
        }

        public static Texture GetTextureByTileType(TileType type)
        {
            if ((int)type < textures.Count)
                return textures[(int)type];

            return null;
        }
    }
}
