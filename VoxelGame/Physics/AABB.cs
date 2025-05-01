using SFML.System;
using System.Numerics;

namespace VoxelGame.Physics
{
    public class AABB
    {
        public Vector2f Min { get; set; }
        public Vector2f Max { get; set; }

        public Entity? Entity { get; set; }

        public AABB(Vector2f max)
        {
            Max = max;
            Min = new Vector2f();
        }

        public AABB(Vector2f min, Vector2f max)
        {
            Min = min;
            Max = max;
        }

        public AABB(float w, float h)
        {
            Max = new Vector2f(w, h);
            Min = new Vector2f();
        }

        public AABB(float x, float y, float w, float h)
        {
            Min = new Vector2f(x, y);
            Max = new Vector2f(x + w, y + h);
        }

        public bool Intersect(AABB other)
        {
            return Min.X < other.Max.X && Max.X > other.Min.X &&
                   Min.Y < other.Max.Y && Max.Y > other.Min.Y;
        }

        public bool Intersect(AABB other, out Vector2f normal, out float depth)
        {
            normal = new Vector2f();
            depth = 0;

            float dx = MathF.Min(Max.X, other.Max.X) - MathF.Max(Min.X, other.Min.X);
            float dy = MathF.Min(Max.Y, other.Max.Y) - MathF.Max(Min.Y, other.Min.Y);

            if (dx > 0 && dy > 0)
            {
                if (dx < dy)
                {
                    float direction = Max.X > other.Min.X && Min.X < other.Min.X ? -1f : 1f;
                    normal = new Vector2f(direction, 0);
                    depth = dx;

                    return true;
                }
                else
                {
                    float direction = Max.Y > other.Min.Y && Min.Y < other.Min.Y ? -1f : 1f;
                    normal = new Vector2f(0, direction);
                    depth = dy;

                    return true;
                }
            }

            return false;
        }

        public AABB Transform(Vector2f position)
        {
            return new AABB(Min + position, Max + position);
        }
    }
}
