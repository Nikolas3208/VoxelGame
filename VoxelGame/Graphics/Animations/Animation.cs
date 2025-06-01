using SFML.Graphics;
using SFML.System;

namespace VoxelGame.Graphics.Animations
{
    /// <summary>
    /// Enum representing the type of animation
    /// </summary>
    public enum AnimationType
    {
        Default,
        Tranformation
    }

    public class Animation : Drawable
    {
        /// <summary>
        /// The sprite sheet that contains textures for the animation
        /// </summary>
        private SpriteSheet _spriteSheet;

        /// <summary>
        /// The current sprite of the animation to be drawn on the screen
        /// </summary>
        private RectangleShape _rect;

        /// <summary>
        /// The array of animation frames
        /// </summary>
        private AnimationFrame[] _frames;

        /// <summary>
        /// The current animation frame
        /// </summary>
        private AnimationFrame _currentFrame;

        /// <summary>
        /// The index of the current frame in the array
        /// </summary>
        private int _currentFrameIndex = 0;

        /// <summary>
        /// Timer to track time between frames
        /// </summary>
        private float _timer;

        /// <summary>
        /// The type of animation
        /// </summary>
        public AnimationType Type { get; set; }

        public float AnimationSpeed { get; set; } = 1f; // Animation speed

        /// <summary>
        /// Has the animation ended?
        /// </summary>
        public bool IsEnd { get; private set; }

        /// <summary>
        /// Animation constructor
        /// </summary>
        /// <param name="spriteSheet"> Sprite sheet </param>
        /// <param name="type"> Animation type </param>
        /// <param name="frames"> Animation frames </param>
        public Animation(SpriteSheet spriteSheet, AnimationType type, params AnimationFrame[] frames)
        {
            _spriteSheet = spriteSheet;
            _frames = frames;
            Type = type;

            _rect = new RectangleShape(new Vector2f(spriteSheet.SubWidth, spriteSheet.SubHeight));
            _rect.Origin = _rect.Size / 2;
            _rect.Texture = spriteSheet.Texture;

            ResetAnimation();
        }

        /// <summary>
        /// Reset the animation to the first frame
        /// </summary>
        private void ResetAnimation()
        {
            _timer = 0f;
            _currentFrameIndex = 0;
            _currentFrame = _frames[_currentFrameIndex];
            IsEnd = false;

            UpdateSprite();
        }

        /// <summary>
        /// Move to the next animation frame
        /// </summary>
        private void NextFrame()
        {
            _timer = 0;
            _currentFrameIndex++;

            if (_currentFrameIndex >= _frames.Length)
            {
                IsEnd = true;
                _currentFrameIndex = 0;
                ResetAnimation();
            }

            _currentFrame = _frames[_currentFrameIndex];

            UpdateSprite();
        }

        /// <summary>
        /// Update the sprite's appearance based on the current frame
        /// </summary>
        private void UpdateSprite()
        {
            _rect.TextureRect = _spriteSheet.GetTextureRect(_currentFrame.X, _currentFrame.Y);
            _rect.Position = _currentFrame.Position;
            _rect.FillColor = _currentFrame.Color;

            if (Type == AnimationType.Tranformation)
            {
                _rect.Scale = _currentFrame.Scale;
                _rect.Rotation = _currentFrame.Rotation;
            }
        }

        /// <summary>
        /// Update the animation
        /// </summary>
        /// <param name="deltaTime"> Delta time </param>
        public void Update(float deltaTime)
        {
            _timer += deltaTime * AnimationSpeed;

            if (_timer >= _currentFrame.Time)
                NextFrame();
        }

        /// <summary>
        /// Draw the current animation frame on the screen
        /// </summary>
        /// <param name="target"></param>
        /// <param name="states"></param>
        public void Draw(RenderTarget target, RenderStates states)
        {
            if (_spriteSheet == null || _rect == null)
                return;

            target.Draw(_rect, states);
        }
    }
}
