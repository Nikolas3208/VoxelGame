using SFML.Graphics;
using SFML.System;
using VoxelGame.Item;

namespace VoxelGame.Worlds.Tile
{
    public enum WallType
    {
        None,
        GroundWall,
        StoneWall,
        OakBoardWall,
    }

    public class WallTile : ITile
    {
        public const int WallSize = 36; // Размер стенки

        // Соседние плитки
        protected WallTile? _upWall;
        protected WallTile? _downWall;
        protected WallTile? _leftWall;
        protected WallTile? _rightWall;

        protected Vertex[] _vertices = new Vertex[6]; // Вершины стенки

        /// <summary>
        /// Id плитки
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Тип плитки
        /// </summary>
        public WallType Type { get; set; }

        /// <summary>
        /// Требуемый инструмент
        /// </summary>
        public ItemType RequiredTool { get; set; }

        /// <summary>
        /// Требуемаяя мощность инструмента
        /// </summary>
        public int RequiredToolPower { get; set; }

        /// <summary>
        /// Выпадаемый предмет, после разрушения
        /// </summary>
        public ItemList DropItem { get; set; }

        /// <summary>
        /// Прочность
        /// </summary>
        public float Strength { get; set; }

        /// <summary>
        /// Твердый?
        /// </summary>
        public bool IsSolid { get; set; }

        /// <summary>
        /// Родительский чанк
        /// </summary>
        public Chunk PerentChunk { get; set; }

        /// <summary>
        /// Если плитка состоит из нескольких, добавляем им всем родителя
        /// </summary>
        public WallTile? PerentTile { get; set; }

        /// <summary>
        /// Верхний сосед
        /// </summary>
        public WallTile? UpWall
        {
            get => _upWall;
            set
            {
                _upWall = value;
                UpdateView();
            }
        }

        /// <summary>
        /// Нижний сосед
        /// </summary>
        public WallTile? DownWall
        {
            get => _downWall;
            set
            {
                _downWall = value;
                UpdateView();
            }
        }

        /// <summary>
        /// Левый сосед
        /// </summary>
        public WallTile? LeftWall
        {
            get => _leftWall;
            set
            {
                _leftWall = value;
                UpdateView();
            }
        }

        /// <summary>
        /// Правый сосед
        /// </summary>
        public WallTile? RightWall
        {
            get => _rightWall;
            set
            {
                _rightWall = value;
                UpdateView();
            }
        }

        /// <summary>
        /// Мировая позиция стенки
        /// </summary>
        public Vector2f GlobalPosition { get; set; }

        /// <summary>
        /// Локальная позиция стенки
        /// </summary>
        public Vector2f LocalPosition { get; set; }

        /// <summary>
        /// Размер плитки
        /// </summary>
        public Vector2f Size { get; set; }

        /// <summary>
        /// Стенка
        /// </summary>
        /// <param name="type"> Тип стекни </param>
        /// <param name="requiredTool"> Требуемый инструмент </param>
        /// <param name="reqiuredToolPower"> Мощность требуемого инструмента </param>
        /// <param name="strength"> Прочность стенки </param>
        /// <param name="isSolid"> Стенка твердая? </param>
        public WallTile(WallType type, ItemList dropItem, ItemType requiredTool, int reqiuredToolPower, float strength, bool isSolid,
            Chunk perentChunk, WallTile? upWall, WallTile? downWall, WallTile? leftWall, WallTile? rightWall, Vector2f localPosition)
        {
            Type = type;
            DropItem = dropItem;
            RequiredTool = requiredTool;
            RequiredToolPower = reqiuredToolPower;
            Strength = strength;
            IsSolid = isSolid;
            LocalPosition = localPosition;

            PerentChunk = perentChunk;

            // Присваиваем соседей, а соседям эту плитку
            if (upWall != null)
            {
                _upWall = upWall;
                _upWall.DownWall = this;    // Для верхнего соседа эта плитка будет нижним соседом
            }
            if (downWall != null)
            {
                _downWall = downWall;
                _downWall.UpWall = this;    // Для нижнего соседа эта плитка будет верхним соседом
            }
            if (leftWall != null)
            {
                _leftWall = leftWall;
                _leftWall.RightWall = this;    // Для левого соседа эта плитка будет правым соседом
            }
            if (rightWall != null)
            {
                _rightWall = rightWall;
                _rightWall.LeftWall = this;    // Для правого соседа эта плитка будет левым соседом
            }

            UpdateView();
        }

        /// <summary>
        /// Обновляем текстурные координаты
        /// </summary>
        public void UpdateView()
        {
            Vector2u texturePosFraq = new Vector2u(0, 0); // Позиция спрайта на текстурном атласе

            switch (Type)
            {
                default:
                    // Если у плитки есть все соседи
                    if (_upWall != null && _downWall != null && _leftWall != null && _rightWall != null)
                    {
                        int i = World.Random.Next(0, 3); // Случайное число от 0 до 2
                        texturePosFraq = new Vector2u(1 + (uint)i, 1);
                    }
                    // Если у плитки отсутствуют все соседи
                    else if (_upWall == null && _downWall == null && _leftWall == null && _rightWall == null)
                    {
                        int i = World.Random.Next(0, 3);
                        texturePosFraq = new Vector2u(9 + (uint)i, 3);
                    }

                    //---------------

                    // Если у плитки отсутствует только верхний сосед
                    else if (_upWall == null && _downWall != null && _leftWall != null && _rightWall != null)
                    {
                        int i = World.Random.Next(0, 3);
                        texturePosFraq = new Vector2u(1 + (uint)i, 0);
                    }
                    // Если у плитки отсутствует только нижний сосед
                    else if (_upWall != null && _downWall == null && _leftWall != null && _rightWall != null)
                    {
                        int i = World.Random.Next(0, 3);
                        texturePosFraq = new Vector2u(1 + (uint)i, 2);
                    }
                    // Если у плитки отсутствует только левый сосед
                    else if (_upWall != null && _downWall != null && _leftWall == null && _rightWall != null)
                    {
                        int i = World.Random.Next(0, 3);
                        texturePosFraq = new Vector2u(0, (uint)i);
                    }
                    // Если у плитки отсутствует только правый сосед
                    else if (_upWall != null && _downWall != null && _leftWall != null && _rightWall == null)
                    {
                        int i = World.Random.Next(0, 3);
                        texturePosFraq = new Vector2u(4, (uint)i);
                    }

                    //---------------

                    // Если у плитки отсутствует только верхний и левый сосед
                    else if (_upWall == null && _downWall != null && _leftWall == null && _rightWall != null)
                    {
                        int i = World.Random.Next(0, 3);
                        texturePosFraq = new Vector2u((uint)i * 2, 3);
                    }
                    // Если у плитки отсутствует только верхний и правый сосед
                    else if (_upWall == null && _downWall != null && _leftWall != null && _rightWall == null)
                    {
                        int i = World.Random.Next(0, 3);
                        texturePosFraq = new Vector2u(1 + (uint)i * 2, 3);
                    }
                    // Если у плитки отсутствует только нижний и левый сосед
                    else if (_upWall != null && _downWall == null && _leftWall == null && _rightWall != null)
                    {
                        int i = World.Random.Next(0, 3);
                        texturePosFraq = new Vector2u(0 + (uint)i * 2, 4);
                    }
                    // Если у плитки отсутствует только нижний и правый сосед
                    else if (_upWall != null && _downWall == null && _leftWall != null && _rightWall == null)
                    {
                        int i = World.Random.Next(0, 3);
                        texturePosFraq = new Vector2u(1 + (uint)i * 2, 4);
                    }
                    //Если есть только правая и леввая плитка
                    else if (_upWall == null && _downWall == null && _leftWall != null && _rightWall != null)
                    {
                        int i = World.Random.Next(0, 3);
                        texturePosFraq = new Vector2u(6 + (uint)i, 4);
                    }
                    //Если есть только верхняя и нижняя плитка
                    else if (_upWall != null && _downWall != null && _leftWall == null && _rightWall == null)
                    {
                        int i = World.Random.Next(0, 3);
                        texturePosFraq = new Vector2u(5, 0 + (uint)i);
                    }

                    //----------------------------------------------

                    //Если есть только верхний сосед
                    else if (_upWall != null && _downWall == null && _leftWall == null && _rightWall == null)
                    {
                        int i = World.Random.Next(0, 3);
                        texturePosFraq = new Vector2u(6, 3);
                    }
                    // Если есть только нижний сосед
                    else if (_upWall == null && _downWall != null && _leftWall == null && _rightWall == null)
                    {
                        int i = World.Random.Next(0, 3);
                        texturePosFraq = new Vector2u(6, 0);
                    }
                    // Если есть только левый сосед
                    else if (_upWall == null && _downWall == null && _leftWall != null && _rightWall == null)
                    {
                        int i = World.Random.Next(0, 3);
                        texturePosFraq = new Vector2u(12, 0 + (uint)i);
                    }
                    // Если есть только правый сосед
                    else if (_upWall == null && _downWall == null && _leftWall == null && _rightWall != null)
                    {
                        int i = World.Random.Next(0, 3); // Случайное число от 0 до 2
                        texturePosFraq = new Vector2u(9, 0 + (uint)i);
                    }
                    // Если есть только правый сосед
                    else if (_upWall == null && _downWall != null && _leftWall != null && _rightWall != null)
                    {
                        int i = World.Random.Next(0, 2); // Случайное число от 0 до 2
                        texturePosFraq = new Vector2u(1 + (uint)i, 0);
                    }
                    break;
            }

            _vertices[0] = new Vertex(new Vector2f(LocalPosition.X * Tile.TileSize, LocalPosition.Y * Tile.TileSize) - new Vector2f(Tile.TileSize, Tile.TileSize) / 2);

            _vertices[1] = new Vertex(new Vector2f(LocalPosition.X * Tile.TileSize, LocalPosition.Y * Tile.TileSize + WallSize) - new Vector2f(Tile.TileSize, Tile.TileSize) / 2);
            _vertices[2] = new Vertex(new Vector2f(LocalPosition.X * Tile.TileSize + WallSize, LocalPosition.Y * Tile.TileSize) - new Vector2f(Tile.TileSize, Tile.TileSize) / 2);
            _vertices[3] = new Vertex(new Vector2f(LocalPosition.X * Tile.TileSize + WallSize, LocalPosition.Y * Tile.TileSize) - new Vector2f(Tile.TileSize, Tile.TileSize) / 2);
            _vertices[4] = new Vertex(new Vector2f(LocalPosition.X * Tile.TileSize, LocalPosition.Y * Tile.TileSize + WallSize) - new Vector2f(Tile.TileSize, Tile.TileSize) / 2);

            _vertices[5] = new Vertex(new Vector2f(LocalPosition.X * Tile.TileSize + WallSize, LocalPosition.Y * Tile.TileSize + WallSize) - new Vector2f(Tile.TileSize, Tile.TileSize) / 2);

            // Получаем текстурные координаты
            int x = (int)(texturePosFraq.X * WallTile.WallSize);
            int y = (int)(texturePosFraq.Y * WallTile.WallSize);

            _vertices[0].TexCoords = new Vector2f(x, y);

            _vertices[1].TexCoords = new Vector2f(x, WallSize + y);
            _vertices[2].TexCoords = new Vector2f(WallSize + x, y);
            _vertices[3].TexCoords = new Vector2f(WallSize + x, y);
            _vertices[4].TexCoords = new Vector2f(x, WallSize + y);

            _vertices[5].TexCoords = new Vector2f(WallSize + x, WallSize + y);

            PerentChunk?.UpdateWallViewByWorldPosition(GlobalPosition, _vertices);
        }

        /// <summary>
        /// Лобаем плитку
        /// </summary>
        /// <param name="type"> Тип предмета </param>
        /// <param name="itemPower"> Мошность предмета </param>
        /// <returns> Если здоровье плитки 0 или меньше true </returns>
        public bool Breaking(ItemType type, float itemPower, float damage)
        {
            if (UpWall == null || DownWall == null || LeftWall == null || RightWall == null)
            {
                if (type == RequiredTool && itemPower >= RequiredToolPower)
                {
                    Strength -= damage;
                    if (Strength <= 0)
                        return true;
                }
            }

            return false;
        }

        public Vertex[] GetVertices()
        {
            return _vertices;
        }
    }
}
