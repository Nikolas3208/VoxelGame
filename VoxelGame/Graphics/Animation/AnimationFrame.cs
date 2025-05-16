namespace VoxelGame.Graphics.Animation;
public class AnimationFrame
{
    /// <summary>
    /// Номер кадра в анимации
    /// </summary>
    public int FrameId { get; }

    /// <summary>
    /// Номер спрайта в SpriteSheet
    /// </summary>
    public int SpriteId { get; }

    public float Time { get; }

    /// <summary>
    /// Кадр анимации
    /// </summary>
    /// <param name="frameId"> номер кадра </param>
    /// <param name="spriteId"> номер спрайта </param>
    /// <param name="time"> Время кадра </param>
    public AnimationFrame(int spriteId, float time)
    {
        FrameId = spriteId;
        SpriteId = spriteId;
        Time = time;
    }
}
