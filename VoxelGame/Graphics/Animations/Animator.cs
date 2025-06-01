using SFML.Graphics;

namespace VoxelGame.Graphics.Animations
{
    /// <summary>
    /// Class that manages object animations
    /// </summary>
    public class Animator : Drawable
    {
        /// <summary>
        /// Dictionary of all animations by name
        /// </summary>
        private Dictionary<string, List<Animation>> _animations;

        /// <summary>
        /// Currently playing animation
        /// </summary>
        private List<Animation>? _currentAnimation;

        /// <summary>
        /// Animator constructor
        /// </summary>
        public Animator()
        {
            _animations = new Dictionary<string, List<Animation>>();
            _currentAnimation = null;
        }

        /// <summary>
        /// Add an animation by name
        /// </summary>
        /// <param name="name">Animation name</param>
        /// <param name="animation">Animation</param>
        /// <returns>True if the animation was added</returns>
        public bool AddAnimation(string name, params Animation[] animation)
        {
            if (!_animations.ContainsKey(name))
            {
                var animationList = new List<Animation>(animation);

                _animations.Add(name, animationList);

                return true;
            }
            else
            {
                _animations[name].AddRange(animation);
                return true;
            }
        }

        /// <summary>
        /// Play animation by name
        /// </summary>
        /// <param name="name">Animation name</param>
        public void Play(string name)
        {
            if (_animations.ContainsKey(name))
            {
                _currentAnimation = _animations[name];
            }
            else
            {
                _currentAnimation = null;
            }
        }

        /// <summary>
        /// Update the current animation
        /// </summary>
        /// <param name="deltaTime">Time between frames</param>
        public void Update(float deltaTime)
        {
            if (_currentAnimation == null)
                return;

            foreach (var animation in _currentAnimation)
            {
                // Update each animation frame based on deltaTime
                animation.Update(deltaTime);
            }
        }

        /// <summary>
        /// Draw the current animation
        /// </summary>
        /// <param name="target">Render target</param>
        /// <param name="states">Render states</param>
        public void Draw(RenderTarget target, RenderStates states)
        {
            if (_currentAnimation == null)
                return;

            foreach (var animation in _currentAnimation)
            {
                animation.Draw(target, states);
            }
        }
    }
}

