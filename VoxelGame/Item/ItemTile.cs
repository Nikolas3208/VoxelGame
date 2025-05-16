using SFML.Graphics;
using VoxelGame.Worlds.Tile;

namespace VoxelGame.Item
{
    public class ItemTile : Item
    {
        /// <summary>
        /// Это стена?
        /// </summary>
        public WallType WallType { get; set; } = WallType.None;

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
        public ItemTile(TileType type, int maxCountInStack = 64) : base(ItemType.Tile, type.ToString(), maxCountInStack)
        {
            TileType = type;
        }

        /// <summary>
        /// Предмет стенка
        /// </summary>
        /// <param name="type"> Тип стенки </param>
        /// <param name="spriteIndex">  </param>
        /// <param name="maxCountInStack"> Максимальное количество в стаке </param>
        public ItemTile(WallType type, int maxCountInStack = 64) : base(ItemType.Wall, type.ToString(), maxCountInStack)
        {
            WallType = type;
        }

        public ItemTile(ItemTile tile) : base(tile)
        {
            TileType = tile.TileType;
            WallType = tile.WallType;
        }
    }
}
