using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoxelGame.Item;
using VoxelGame.Item.Crafts;

namespace VoxelGame.Worlds.Chunks.Tile
{
    public abstract class InfoTile : Transformable
    {
        /// <summary>
        /// Константа минимальный размер одной плитки
        /// </summary>
        public const int MinTileSize = 16;

        /// <summary>
        /// Тип плитки
        /// </summary>
        public TileType Type;

        /// <summary>
        /// Размер плитки, стандартный размер 16х16
        /// </summary>
        public Vector2i TileSize = new Vector2i(MinTileSize, MinTileSize);

        /// <summary>
        /// Идентификатор плитки в сетке
        /// </summary>
        public int TileId;

        /// <summary>
        /// Являеться ли плитка стеной
        /// </summary>
        public bool IsWall = false;

        /// <summary>
        /// Кординаты плитки на текстуре
        /// </summary>
        public Vector2f TextureCoord = new Vector2f(33, 0);

        /// <summary>
        /// Крафт плитки
        /// </summary>
        private ItemCraft craft;

        /// <summary>
        /// Родительская плитка ( модет не быть )
        /// </summary>
        protected InfoTile? perentTile;

        /// <summary>
        /// Робительский чанк ( обязан быть )
        /// </summary>
        protected Chunk perentChunk;

        /// <summary>
        /// Создание плитки стандартного размера
        /// </summary>
        /// <param name="chunk"> Родительский чанк </param>
        /// <param name="type"> Тип плитки </param>
        public InfoTile(Chunk chunk, TileType type)
        {
            perentChunk = chunk;
            Type = type;
        }

        /// <summary>
        /// Создание плитки храннящей ссылку на родителя
        /// </summary>
        /// <param name="chunk"> Родительский чанк </param>
        /// <param name="tile"> Родительская плитка </param>
        public InfoTile(Chunk chunk, InfoTile tile)
        {
            perentTile = tile;
            Type = tile.Type;
            TileSize = tile.TileSize;
        }

        /// <summary>
        /// Создание плитки своего размера
        /// </summary>
        /// <param name="chunk"> Родительский чанк </param>
        /// <param name="type"> Тип плитки </param>
        /// <param name="tileSize"> Размер плитки </param>
        public InfoTile(Chunk chunk, TileType type, Vector2i tileSize)
        {
            perentChunk = chunk;
            Type = type;
            TileSize = tileSize;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual bool OnTileUse()
        {
            if (perentTile != null)
            {
                return perentTile.OnTileUse();
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="craft"></param>
        public void SetItemCraft(ItemCraft craft) => this.craft = craft;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="chunk"></param>
        public void SetPerentChunk(Chunk chunk) => perentChunk = chunk;

        public Vector2f GetPositionByChunk()
        {
            return Position - perentChunk.Position;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ItemCraft GetItemCraft() => this.craft;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public InfoTile GetPerentTile() => perentTile;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Chunk GetPerentChunk() => perentChunk;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public FloatRect GetFloatRect() => new FloatRect(Position, (Vector2f)TileSize);
    }
}
