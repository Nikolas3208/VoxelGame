using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoxelGame
{
    public class ContentManager
    {
        public string? contentPath;

        public static Texture Texture1 = new Texture("Contents\\Textures\\Items\\Item_0.png");
        public static Texture Texture2 = new Texture("Contents\\Textures\\Items\\Item_4.png");
    }
}
