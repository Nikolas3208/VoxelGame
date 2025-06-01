namespace VoxelGame.Worlds.Tile
{
    /// <summary>
    /// Интерфейс для плиток, которые могут быть использованы игроком.
    /// </summary>
    public interface IUsedTile
    {
        /// <summary>
        /// Метод, который вызывается при использовании плитки.
        /// </summary>
        void Use();
    }
}