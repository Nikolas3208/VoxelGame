using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoxelGame.Worlds.Chunks
{
    public class ChunkInfo
    {
        public static Vector2i ChunckSize = new Vector2i(16, 16);
        public static Vector2f ChunkSizeByPixel = new Vector2f(256, 256);
    }
}
