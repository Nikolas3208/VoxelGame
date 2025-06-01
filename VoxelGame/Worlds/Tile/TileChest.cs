using SFML.Graphics;
using SFML.System;
using VoxelGame.Item;
using VoxelGame.UI;
using VoxelGame.UI.Inventory;

namespace VoxelGame.Worlds.Tile
{
    /// <summary>
    /// Класс TileChest представляет плитку сундука в игровом мире.
    /// </summary>
    public class TileChest : Tile, IUsedTile
    {
        /// <summary>
        /// Размер X спрайта сундука на текстурном листе.
        /// </summary>
        private int _chestSpriteX = 16;

        /// <summary>
        /// Размер Y спрайта сундука на текстурном листе.
        /// </summary>
        private int _chestSpriteY = 18;

        /// <summary>
        /// Инвентарь, связанный с сундуком.
        /// </summary>
        private UIInventory _chestInventory;

        /// <summary>
        /// Конструктор класса TileChest.
        /// </summary>
        /// <param name="type">Тип плитки.</param>
        /// <param name="dropItem">Предмет, выпадающий при разрушении плитки.</param>
        /// <param name="requiredTool">Требуемый инструмент для взаимодействия с плиткой.</param>
        /// <param name="reqiuredToolPower">Мощность требуемого инструмента.</param>
        /// <param name="strength">Прочность плитки.</param>
        /// <param name="isSolid">Является ли плитка твердой.</param>
        /// <param name="perentChunk">Родительский чанк.</param>
        /// <param name="upTile">Соседняя плитка сверху.</param>
        /// <param name="downTile">Соседняя плитка снизу.</param>
        /// <param name="leftTile">Соседняя плитка слева.</param>
        /// <param name="rightTile">Соседняя плитка справа.</param>
        /// <param name="wall">Стенка, связанная с плиткой.</param>
        /// <param name="localPosition">Локальная позиция плитки.</param>
        public TileChest(TileType type, ItemList dropItem, ItemType requiredTool, int reqiuredToolPower, float strength, bool isSolid,
            Chunk perentChunk, Tile? upTile, Tile? downTile, Tile? leftTile, Tile? rightTile, WallTile? wall, Vector2f localPosition)
            : base(type, dropItem, requiredTool, reqiuredToolPower, strength, isSolid,
                  perentChunk, upTile, downTile, leftTile, rightTile, wall, localPosition)
        {
            // Установка имени текстуры.
            TextureName = "Chest";

            // Установка размера плитки.
            Size = new Vector2f(2, 2);

            // Инициализация инвентаря сундука.
            _chestInventory = new UIInventory(new Vector2f(UIInventoryCell.CellSize * 10, UIInventoryCell.CellSize));

            // Установка позиции инвентаря на экране.
            _chestInventory.Position = new Vector2f(0, Game.GetWindowSizeWithZoom().Y / 2 - _chestInventory.Size.Y);
            _chestInventory.ShowInventory(); // Показываем инвентарь сундука
            _chestInventory.Craft = null!; // Отключаем крафт для сундука
        }

        /// <summary>
        /// Обновляет визуальное представление плитки в зависимости от соседей.
        /// </summary>
        public override void UpdateView()
        {
            if (DownTile is not TileChest)
            {
                if (RightTile is TileChest)
                    GenerateTileMesh(0, 1);
                else if (LeftTile is TileChest)
                    GenerateTileMesh(1, 1);
            }
            else
            {
                if (RightTile is TileChest)
                    GenerateTileMesh(0, 0);
                else if (LeftTile is TileChest)
                    GenerateTileMesh(1, 0);
            }
        }

        /// <summary>
        /// Генерирует сетку плитки с учетом текстурных координат.
        /// </summary>
        /// <param name="x">Координата X текстуры.</param>
        /// <param name="y">Координата Y текстуры.</param>
        public void GenerateTileMesh(int x, int y)
        {
            // Установка вершин сетки плитки.
            _vertices[0] = new Vertex(new Vector2f(LocalPosition.X * Tile.TileSize, LocalPosition.Y * Tile.TileSize));
            _vertices[1] = new Vertex(new Vector2f(LocalPosition.X * Tile.TileSize, LocalPosition.Y * Tile.TileSize + Tile.TileSize));
            _vertices[2] = new Vertex(new Vector2f(LocalPosition.X * Tile.TileSize + Tile.TileSize, LocalPosition.Y * Tile.TileSize));
            _vertices[3] = new Vertex(new Vector2f(LocalPosition.X * Tile.TileSize + Tile.TileSize, LocalPosition.Y * Tile.TileSize));
            _vertices[4] = new Vertex(new Vector2f(LocalPosition.X * Tile.TileSize, LocalPosition.Y * Tile.TileSize + Tile.TileSize));
            _vertices[5] = new Vertex(new Vector2f(LocalPosition.X * Tile.TileSize + Tile.TileSize, LocalPosition.Y * Tile.TileSize + Tile.TileSize));

            // Вычисление текстурных координат.
            x = (int)(x * _chestSpriteX + x * 2);
            y = (int)(y * _chestSpriteY + y * 2);

            _vertices[0].TexCoords = new Vector2f(x, y);
            _vertices[1].TexCoords = new Vector2f(x, _chestSpriteY + y);
            _vertices[2].TexCoords = new Vector2f(_chestSpriteX + x, y);
            _vertices[3].TexCoords = new Vector2f(_chestSpriteX + x, y);
            _vertices[4].TexCoords = new Vector2f(x, _chestSpriteY + y);
            _vertices[5].TexCoords = new Vector2f(_chestSpriteX + x, _chestSpriteY + y);

            // Обновление визуального представления плитки в чанке.
            PerentChunk?.UpdateTileViewByWorldPosition(GlobalPosition, _vertices);
        }

        /// <summary>
        /// Открывает инвентарь сундука.
        /// </summary>
        public void Use()
        {
            if (PerentTile == null)
            {

                UIManager.AddWindow(_chestInventory);
                PerentChunk.GetWorld()?.GetPlayer()!.ShowInventory(this); // Показываем инвентарь игрока при открытии сундука
            }
            else
            {
                (PerentTile as IUsedTile)?.Use();
            }
        }

        /// <summary>
        /// Возвращает инвентарь, связанный с плиткой сундука.
        /// </summary>
        /// <returns>Объект UIInventory, представляющий инвентарь сундука, или null, если инвентарь отсутствует.</returns>
        public UIInventory GetInventory()
        {
            return _chestInventory;
        }
    }
}
