using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoxelGame.Worlds.Chunks.Tiles
{
    public enum TileType
    {
        None,
        Ground,
        GroundWall
    }

    public class TileInfo
    {
        private TileType type = TileType.Ground;

        public const int MinTileSize = 16;

        /// <summary>
        /// Тип плитки
        /// Стандартное значение Ground
        /// </summary>
        public TileType Type { get => type; set { type = value; if (value != TileType.None) SetTileSettings(value); } }

        /// <summary>
        /// Размер плитки
        /// </summary>
        public Vector2f Size;

        /// <summary>
        /// Id притки в сетке
        /// </summary>
        public int Id = 0;

        public TileInfo() { }

        /// <summary>
        /// Информация о плитке
        /// </summary>
        /// <param name="type"> Тип плитки </param>
        public TileInfo(TileType type)
        {
            Type = type;
        }

        private void SetTileSettings(TileType value)
        {
            if(value != TileType.None)
            {
                if (value == TileType.Ground)
                {
                    Size = new Vector2f(16, 16);
                }
                else if (value == TileType.GroundWall)
                {
                    Size = new Vector2f(32, 32);
                }
            }
        }

        public static Texture? GetTextureByTileType(TileType key)
        {
            switch (key)
            {
                case TileType.Ground:
                    return ContentManager.Texture1;
                case TileType.GroundWall:
                    return ContentManager.Texture2;
            }

            return null;
        }
    }
}
