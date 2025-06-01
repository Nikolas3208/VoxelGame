using SFML.Graphics;
using SFML.System;

namespace VoxelGame.Graphics.Animations
{
    public class AnimationFrame
    {
        /// <summary>
        /// Frame X coordinate in the sprite sheet
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// Frame Y coordinate in the sprite sheet
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        /// Frame duration
        /// </summary>
        public float Time { get; set; }

        /// <summary>
        /// Frame rotation
        /// </summary>
        public float Rotation { get; set; } = 0f; // Rotation of the frame

        /// <summary>
        /// Frame position
        /// </summary>
        public Vector2f Position { get; set; }

        /// <summary>
        /// Frame scale, default is (1, 1)
        /// </summary>
        public Vector2f Scale { get; set; } = new Vector2f(1f, 1f); // Scale of the frame

        /// <summary>
        /// Frame color, default is white
        /// </summary>
        public Color Color { get; set; } = Color.White; // Color of the frame, default is white

        /// <summary>
        /// Animation frame constructor
        /// </summary>
        /// <param name="x">Frame X position in the sprite sheet</param>
        /// <param name="y">Frame Y position in the sprite sheet</param>
        /// <param name="time">Frame duration</param>
        /// <param name="rotation">Frame rotation</param>
        /// <param name="position">Frame position</param>
        /// <param name="scale">Frame scale, if (0, 0) then (1, 1)</param>
        /// <param name="color">Frame color, if (0, 0, 0) then (255, 255, 255)</param>
        public AnimationFrame(int x, int y, float time, float rotation = 0, Vector2f position = new Vector2f(), Vector2f scale = new Vector2f(), Color color = new Color())
        {
            X = x;
            Y = y;
            Time = time;
            Rotation = rotation;
            Position = position;

            if (scale.X != 0 || scale.Y != 0)
                Scale = scale;

            if (color.R != 0 || color.G != 0 || color.B != 0)
                Color = color;
        }
    }
}
