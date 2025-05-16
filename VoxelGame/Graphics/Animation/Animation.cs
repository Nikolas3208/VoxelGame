using SFML.Graphics;

namespace VoxelGame.Graphics.Animation;

public class Animation : Transformable, Drawable
{
    /// <summary>
    /// Масив кадров
    /// </summary>
    private AnimationFrame[]? _frames;

    /// <summary>
    /// Текущий кадр
    /// </summary>
    private AnimationFrame? _curentFrame;

    /// <summary>
    /// Индекс текущей анимации
    /// </summary>
    private int _currentFrameIndex;

    /// <summary>
    /// Таймер ( Считает время анимации )
    /// </summary>
    private float _timer;

    /// <summary>
    /// Спрайт используемый для анимации
    /// </summary>
    private AnimSprite? _animSprite;

    /// <summary>
    /// Имя анимаии
    /// </summary>
    public string Name { get; set; } = string.Empty;

    public bool IsComplayt { get; set; } = false;

    /// <summary>
    /// Создание пустой анимаии
    /// </summary>
    /// <param name="name"> Имя анимации </param>
    public Animation(string name)
    {
        Name = name;
        Reset();
    }

    /// <summary>
    /// Создание анимации с кадрами
    /// </summary>
    /// <param name="name"> Имя анимаии </param>
    /// <param name="frames"> Кадры анимаии </param>
    public Animation(string name, AnimationFrame[] frames) : this(name)
    {
        _frames = frames;
        _curentFrame = _frames[_currentFrameIndex];
    }

    /// <summary>
    /// Создание анимаии с кадрами и спрайтом анимации
    /// </summary>
    /// <param name="name"> Има анимации </param>
    /// <param name="frames"> Кадры анимаии </param>
    /// <param name="animSprite"> Спрайт анимации </param>
    public Animation(string name, AnimationFrame[] frames, AnimSprite animSprite) : this(name, frames)
    {
        animSprite.PerentAnim = this;
        _animSprite = animSprite;
    }

    /// <summary>
    /// Установить кадры анимации с заменой предыдуших
    /// </summary>
    /// <param name="frames"> Кадры анимаии </param>
    public void SetAnimationFrames(params AnimationFrame[] frames)
    {
        _curentFrame = frames[0];
        _frames = frames;
    }

    /// <summary>
    /// Получить кадры анимаии
    /// </summary>
    /// <returns> Возвращает кадры анимаии </returns>
    public AnimationFrame[]? GetAnimationFrames() => _frames;

    /// <summary>
    /// Установить спрайт анимации
    /// </summary>
    /// <param name="animSprite"> Спрайт анимаии </param>
    public void SetAnimSprite(AnimSprite animSprite)
    {
        animSprite.PerentAnim = this;
        _animSprite = animSprite;
    } 

    /// <summary>
    /// Получить спрайт анимации
    /// </summary>
    /// <returns> Спрайт анимаии </returns>
    public AnimSprite? GetAnimationSprite() => _animSprite;

    /// <summary>
    /// Получить кадр и обновить кадр
    /// </summary>
    /// <param name="speed"> Время </param>
    /// <returns></returns>
    public AnimationFrame GetFrame(float speed)
    {
        IsComplayt = false;

        _timer += speed;

        if (_timer >= _curentFrame!.Time)
            NextFrame();

        return _curentFrame;
    }

    /// <summary>
    /// Рисовать анимаию
    /// </summary>
    /// <param name="target"></param>
    /// <param name="states"></param>
    public void Draw(RenderTarget target, RenderStates states)
    {
        states.Transform *= Transform;

        target.Draw(_animSprite, states);
    }

    /// <summary>
    /// Сбросить анимаию
    /// </summary>
    private void Reset()
    {
        _timer = 0;
        _currentFrameIndex = 0;
        if (_frames != null)
            _curentFrame = _frames![_currentFrameIndex];
    }

    /// <summary>
    /// Следующий кадр
    /// </summary>
    private void NextFrame()
    {
        _timer = 0;
        _currentFrameIndex++;

        if (_currentFrameIndex == _frames!.Length)
        {
            _currentFrameIndex = 0;
            IsComplayt = true;
        }

        _curentFrame = _frames![_currentFrameIndex];
    }
}
