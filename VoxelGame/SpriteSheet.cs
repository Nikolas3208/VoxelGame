using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoxelGame
{
    public class SpriteSheet
    {
        private Texture texture;

        private float borderSize;

        private int subWidth;
        private int subHeight;
        private int countX;
        private int countY;

        /// <summary>
        /// Спрайт-лист
        /// </summary>
        /// <param name="texture"> текстура </param>
        /// <param name="x"> Количество фрагментов по ширине или размер одного фрагмента в пикселях </param>
        /// <param name="y"> Количество фрагментов по высоте или размер одного фрагмента в пикселях </param>
        /// <param name="xyIsCount"> X и Y это количество фрагментов? </param>
        /// <param name="borderSize"> Расстояние между фрагментами </param>
        public SpriteSheet(Texture texture, int x, int y, bool xyIsCount, float borderSize = 0)
        {
            this.texture = texture;

            if (borderSize > 0)
                this.borderSize = borderSize + 1;
            else
                this.borderSize = 0;

            if(xyIsCount)
            {
                subWidth = (int)Math.Ceiling((float)texture.Size.X / x);
                subHeight = (int)Math.Ceiling((float)texture.Size.Y / y);
                countX = x;
                countY = y;
            }
            else
            {
                subWidth = x;
                subHeight = y;
                countX = (int)Math.Ceiling((float)texture.Size.X / x);
                countY = (int)Math.Ceiling((float)texture.Size.Y / y);
            }             
        }

        public IntRect GetTextureRect(int x, int y)
        {
            int x2 = x * subWidth + x * (int)borderSize;
            int y2 = y * subHeight + y * (int)borderSize;
            return new IntRect(x2, y2, subWidth, subHeight);
        }
    }
}
