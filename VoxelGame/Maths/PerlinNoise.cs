namespace VoxelGame.Meths;

public class PerlinNoise
{
    private readonly int[] permutation;

    public readonly Random Random;

    public readonly int Seed;

    public PerlinNoise(int seed)
    {
        Seed = seed;

        Random = new Random(seed);
        permutation = new int[512];
        int[] p = new int[256];

        for (int i = 0; i < 256; i++)
            p[i] = i;

        for (int i = 0; i < 256; i++)
        {
            int swapIndex = Random.Next(256);
            (p[i], p[swapIndex]) = (p[swapIndex], p[i]);
        }

        for (int i = 0; i < 512; i++)
            permutation[i] = p[i % 256];
    }

    private static float Fade(float t) =>
        t * t * t * (t * (t * 6 - 15) + 10);

    private static float Lerp(float a, float b, float t) =>
        a + t * (b - a);

    private static float Grad(int hash, float x, float y)
    {
        int h = hash & 15;
        float u = h < 8 ? x : y;
        float v = h < 4 ? y : h == 12 || h == 14 ? x : 0;
        return ((h & 1) == 0 ? u : -u) +
               ((h & 2) == 0 ? v : -v);
    }

    public float Noise(float x, float y)
    {
        int xi = (int)Math.Floor(x) & 255;
        int yi = (int)Math.Floor(y) & 255;

        float xf = x - (int)Math.Floor(x);
        float yf = y - (int)Math.Floor(y);

        float u = Fade(xf);
        float v = Fade(yf);

        int aa = permutation[permutation[xi] + yi];
        int ab = permutation[permutation[xi] + yi + 1];
        int ba = permutation[permutation[xi + 1] + yi];
        int bb = permutation[permutation[xi + 1] + yi + 1];

        float x1 = Lerp(Grad(aa, xf, yf), Grad(ba, xf - 1, yf), u);
        float x2 = Lerp(Grad(ab, xf, yf - 1), Grad(bb, xf - 1, yf - 1), u);
        return Lerp(x1, x2, v);
    }

    // Новый метод: Fractal Noise (с октавами)
    public float Noise(float x, float y, int octaves = 1, float frequency = 0.1f, float amplitude = 1, float persistence = 0.5f)
    {
        float total = 0;
        float max = 0;

        for (int i = 0; i < octaves; i++)
        {
            float freq = frequency * (float)Math.Pow(2, i);
            float amp = amplitude * (float)Math.Pow(persistence, i);

            total += Noise(x * freq, y * freq) * amp;
            max += amp;
        }

        return total / max; // нормализуем от -1 до 1
    }
}
