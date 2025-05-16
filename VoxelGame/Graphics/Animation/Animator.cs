using SFML.Graphics;

namespace VoxelGame.Graphics.Animation;

public class Animator : Transformable, Drawable
{
    /// <summary>
    /// Именной список анимаций
    /// </summary>
    private SortedDictionary<string, Animation> _animations;
    private Animation? _currentAnimation;
    private string _currentAnimationName = string.Empty;
    private string _nextAnimName = string.Empty;

    public Animator()
    {
        _animations = new SortedDictionary<string, Animation>();
    }

    /// <summary>
    /// Добавить анимаию
    /// </summary>
    /// <param name="name"> Имя анимаии </param>
    /// <param name="animation"> Анимаия </param>
    /// <returns> Если анимации с таким именем нет возвращает true </returns>
    public bool AddAnimation(string name, Animation animation)
    {
        if (_animations.ContainsKey(name))
            return false;

        _animations.Add(name, animation);

        _currentAnimation = animation;
        _currentAnimationName = name;
        return true;
    }

    /// <summary>
    /// Удалить анимацию
    /// </summary>
    /// <param name="name"> Имя анимаии </param>
    /// <returns> Если анимаие не найдена возвращает false </returns>
    public bool RemoveAnimation(string name)
    {
        if (!_animations.ContainsKey(name))
            return false;

        return _animations.Remove(name);
    }

    /// <summary>
    /// Получить анимацию
    /// </summary>
    /// <param name="name"> Имя анимации </param>
    /// <returns> Если анимаия не найдена возвращает null </returns>
    public Animation? GetAnimation(string name)
    {
        if (!_animations.ContainsKey(name))
            return null;

        return _animations[name];
    }

    /// <summary>
    /// Получить текущую анимаию
    /// </summary>
    /// <returns> Animation </returns>
    public Animation? GetAnimation()
    {
        return _currentAnimation;
    }


    /// <summary>
    /// Играть анимаию
    /// </summary>
    /// <param name="name"> Имя анимации </param>
    public void Play(string name)
    {
        if (!_animations.ContainsKey(name))
            return;
        if (name == _currentAnimationName)
            return;

        _currentAnimationName = name;
        _currentAnimation = _animations[_currentAnimationName];
    }

    /// <summary>
    /// Рисовать анимаию
    /// </summary>
    /// <param name="target"></param>
    /// <param name="states"></param>
    public void Draw(RenderTarget target, RenderStates states)
    {
        states.Transform *= Transform;

        _currentAnimation!.Draw(target, states);
    }
}
