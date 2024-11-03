using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoxelGame
{
    public class Perlin2d
    {
        byte[] permutationTable;

        public Perlin2d(int seed = -1)
        {
            Random rand = seed == 1 ? new Random(seed) : new Random((int)DateTime.Now.Ticks);
            permutationTable = new byte[1024];
            rand.NextBytes(permutationTable);
        }

        private float[] GetPseudoRandomGradientVector(int x, int y)
        {
            int v = (int)(((x * 1836311903) ^ (y * 2971215073) + 4807526976) & 1023);
            v = permutationTable[v] & 3;

            switch (v)
            {
                case 0: return new float[] { 1, 0 };
                case 1: return new float[] { -1, 0 };
                case 2: return new float[] { 0, 1 };
                default: return new float[] { 0, -1 };
            }
        }

        static float QunticCurve(float t)
        {
            return t * t * t * (t * (t * 6 - 15) + 10);
        }

        static float Lerp(float a, float b, float t)
        {
            return a + (b - a) * t;
        }

        static float Dot(float[] a, float[] b)
        {
            return a[0] * b[0] + a[1] * b[1];
        }

        public float Noise(float fx, float fy)
        {
            int left = (int)System.Math.Floor(fx);
            int top = (int)System.Math.Floor(fy);
            float pointInQuadX = fx - left;
            float pointInQuadY = fy - top;

            float[] topLeftGradient = GetPseudoRandomGradientVector(left, top);
            float[] topRightGradient = GetPseudoRandomGradientVector(left + 1, top);
            float[] bottomLeftGradient = GetPseudoRandomGradientVector(left, top + 1);
            float[] bottomRightGradient = GetPseudoRandomGradientVector(left + 1, top + 1);

            float[] distanceToTopLeft = new float[] { pointInQuadX, pointInQuadY };
            float[] distanceToTopRight = new float[] { pointInQuadX - 1, pointInQuadY };
            float[] distanceToBottomLeft = new float[] { pointInQuadX, pointInQuadY - 1 };
            float[] distanceToBottomRight = new float[] { pointInQuadX - 1, pointInQuadY - 1 };

            float tx1 = Dot(distanceToTopLeft, topLeftGradient);
            float tx2 = Dot(distanceToTopRight, topRightGradient);
            float bx1 = Dot(distanceToBottomLeft, bottomLeftGradient);
            float bx2 = Dot(distanceToBottomRight, bottomRightGradient);

            pointInQuadX = QunticCurve(pointInQuadX);
            pointInQuadY = QunticCurve(pointInQuadY);

            float tx = Lerp(tx1, tx2, pointInQuadX);
            float bx = Lerp(bx1, bx2, pointInQuadX);
            float tb = Lerp(tx, bx, pointInQuadY);

            return tb;
        }

        public float Noise(float fx, float fy, int octaves, float persistence = 2.5f, float amplitude = 1)
        {
            float max = 0;
            float result = 0;

            while (octaves-- > 0)
            {
                max += amplitude;
                result += Noise(fx, fy) * amplitude;
                amplitude *= persistence;
                fx *= 2;
                fy *= 2;
            }

            return result / max;
        }

        public static float[] Interpolate(float[] map, int length, int octaves = 1)
        {
            float[] interpolateMap = new float[length];

            for (int i = 0; i < octaves; i++)
            {
                for (int x = 1; x < length - 1; x++)
                {
                    interpolateMap[x] = (map[x - 1] + map[x + 1] + map[x]) / 3;
                }
            }
            interpolateMap[0] = interpolateMap[1];
            interpolateMap[length - 1] = interpolateMap[length - 2];

            return interpolateMap;
        }
        public static float[,] Interpolate(float[,] map, int length, int octaves = 1)
        {
            float[,] interpolateMap = new float[length / 2, length / 2];

            for (int i = 0; i < octaves; i++)
            {
                for (int x = 1; x < length / 2 - 1; x++)
                {
                    for (int y = 1; y < length / 2 - 1; y++)
                    {
                        interpolateMap[x, y] = (map[x - 1, y] + map[x + 1, y] + map[x, y] + map[x, y - 1] + map[x, y + 1] + map[x - 1, y - 1] + map[x + 1, y + 1] + map[x - 1, y + 1] + map[x + 1, y - 1]) / 9;
                    }
                }
            }

            return interpolateMap;
        }
    }
}
