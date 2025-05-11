using VoxelGame.Worlds.Tile;

namespace VoxelGame.Item
{
    public class ItemTile : Item
    {
        /// <summary>
        /// Это стена?
        /// </summary>
        public bool IsWall = false;

        /// <summary>
        /// Тип плитки
        /// </summary>
        public TileType TileType { get; set; } = TileType.None;

        /// <summary>
        /// Предмет плитка
        /// </summary>
        /// <param name="type"> Тип плитки </param>
        /// <param name="spriteIndex"> Идентефикатор спрайта на листе </param>
        /// <param name="maxCountInStack"> Максимальное количество в стаке </param>
        /// <param name="isWall"> Это стена? </param>
        public ItemTile(TileType type, int spriteIndex, int maxCountInStack = 64, bool isWall = false) : base(ItemType.Tile, type.ToString(), spriteIndex, maxCountInStack)
        {
            TileType = type;
            IsWall = isWall;
        }

        public ItemTile(ItemTile tile) : base(tile)
        {
            IsWall = tile.IsWall;
            TileType = tile.TileType;
        }
    }
}
