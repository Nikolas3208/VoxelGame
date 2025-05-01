using SFML.System;

namespace VoxelGame.Meths;

public class MathHelper
{

    public static float Clamp(float value, float min, float max)
    {
        if (min == max)
            return min;

        if (min > max)
            throw new ArgumentOutOfRangeException("Min is greater then the max");

        if (value < min)
            return min;

        if (value > max)
            return max;

        return value;
    }

    public static float Length(Vector2f v)
    {
        return MathF.Sqrt(v.X * v.X + v.Y * v.Y);
    }

    public static float LengthSquared(Vector2f v)
    {
        return v.X * v.X + v.Y * v.Y;
    }

    public static float Distance(float x1, float y1, float x2, float y2)
    {
        float x = x2 - x1;
        float y = y2 - y1;
        return MathF.Sqrt(x * x + y * y);
    }

    public static float Distance(Vector2f a, Vector2f b)
    {
        float x = b.X - a.X;
        float y = b.Y - a.Y;

        return MathF.Sqrt(x * x + y * y);
    }

    public static float DistanceSquared(Vector2f a, Vector2f b)
    {
        float x = b.X - a.X;
        float y = b.Y - a.Y;

        return x * x + y * y;
    }

    public static Vector2f Normalize(Vector2f v)
    {
        if (v.X == 0 && v.Y == 0)
            return v;

        float len = Length(v);
        v.X /= len;
        v.Y /= len;
        return v;
    }

    public static float Dot(Vector2f a, Vector2f b)
    {
        return a.X * b.X + a.Y * b.Y;
    }

    public static float Cross(Vector2f a, Vector2f b)
    {
        return a.X * b.Y - a.Y * b.X;
    }

    public static bool NearlyEqual(float a, float b)
    {
        return MathF.Abs(a - b) <= 0.0005f;
    }

    public static bool NearlyEqual(Vector2f a, Vector2f b)
    {
        return NearlyEqual(a.X, b.X) && NearlyEqual(a.Y, b.Y);
    }
}


