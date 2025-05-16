using SFML.Graphics;
using SFML.System;

namespace VoxelGame.Graphics.Animation;
public class AnimSprite : Transformable, Drawable
{
    /// <summary>
    /// Время добавляющееся к таймеру
    /// </summary>
    private float _speed = 0.05f;

    /// <summary>
    /// 
    /// </summary>
    private RectangleShape _rect;

    /// <summary>
    /// Спрайт лист
    /// </summary>
    public SpriteSheet? SpriteSheet { get; protected set; }

    /// <summary>
    /// Родитель анимаия
    /// </summary>
    public Animation? PerentAnim { get; set; }

    /// <summary>
    /// Цвет
    /// </summary>
    public Color Color
    {
        get => _rect.FillColor;
        set => _rect.FillColor = value;
    }

    /// <summary>
    /// Размер
    /// </summary>
    public Vector2f Size
    {
        get => _rect.Size;
    }

    /// <summary>
    /// Конструтктор аниамии
    /// </summary>
    /// <param name="spriteSheet"> Спрайт лист </param>
    public AnimSprite(SpriteSheet? spriteSheet)
    {
        SpriteSheet = spriteSheet;

        _rect = new RectangleShape(new Vector2f(SpriteSheet!.SubWidth, SpriteSheet!.SubHeight));
        _rect.Texture = SpriteSheet?.Texture;

        _rect.Origin = _rect.Size / 2;
    }

    /// <summary>
    /// Обновление спрайт листа
    /// </summary>
    /// <param name="spriteSheet"> Спрайт лист </param>
    /// <returns> False если спрайт лист null </returns>
    public bool UpdateSpriteSheet(SpriteSheet spriteSheet)
    {
        if (SpriteSheet == null)
            return false;

        SpriteSheet = spriteSheet;
        _rect = new RectangleShape(new Vector2f(SpriteSheet!.SubWidth, SpriteSheet!.SubHeight));
        _rect.Texture = SpriteSheet?.Texture;
        _rect.Origin = new Vector2f(SpriteSheet!.SubWidth / 2, SpriteSheet!.SubHeight / 2);

        return true;
    }

    /// <summary>
    /// Получить позицию и размер спрайта на листе
    /// </summary>
    /// <returns> позиция и размер спрайта на листе </returns>
    public IntRect GetTextureRect()
    {
        var currFrame = PerentAnim!.GetFrame(_speed);

        return SpriteSheet!.GetTextureRect(currFrame.SpriteId);
    }


    /// <summary>
    /// Рисовать кадр
    /// </summary>
    /// <param name="target"></param>
    /// <param name="states"></param>
    public void Draw(RenderTarget target, RenderStates states)
    {
        _rect.TextureRect = GetTextureRect();

        states.Transform *= Transform;
        target.Draw(_rect, states);
    }
}
