using SFML.Graphics;
using SFML.System;
using VoxelGame.Item;

namespace VoxelGame.Worlds.Tile
{
    /// <summary>
    /// Перечисление типов плиток, доступных в игре.
    /// </summary>
    public enum TileType
    {
        /// <summary>
        /// Отсутствие плитки, используется для пустых областей.
        /// </summary>
        None,

        /// <summary>
        /// Земля — базовый строительный материал.
        /// </summary>
        Ground,

        /// <summary>
        /// Трава — декоративный элемент или поверхность.
        /// </summary>
        Grass,

        /// <summary>
        /// Камень — строительный материал или ресурс.
        /// </summary>
        Stone,

        /// <summary>
        /// Дерево (дуб) — ресурс для создания досок и других предметов.
        /// </summary>
        Oak,

        /// <summary>
        /// Дерево (берёза) — альтернативный ресурс для создания досок.
        /// </summary>
        Birch,

        /// <summary>
        /// Листва — декоративный элемент.
        /// </summary>
        Leaves,

        /// <summary>
        /// Железная руда — ресурс для создания железных предметов.
        /// </summary>
        IronOre,

        /// <summary>
        /// Медная руда — ресурс для создания медных предметов.
        /// </summary>
        CopperOre,

        /// <summary>
        /// Железный слиток — обработанный ресурс для создания предметов.
        /// </summary>
        IronIngot,

        /// <summary>
        /// Медный слиток — обработанный ресурс для создания предметов.
        /// </summary>
        CopperIngot,

        /// <summary>
        /// Доска (дуб) — строительный материал.
        /// </summary>
        OakBoard,

        /// <summary>
        /// Доска (берёза) — строительный материал.
        /// </summary>
        BirchBoard,

        /// <summary>
        /// Дверь — элемент интерьера или защиты.
        /// </summary>
        Door,

        /// <summary>
        /// Верстак — инструмент для создания сложных предметов.
        /// </summary>
        Workbench,

        /// <summary>
        /// Сундук — хранилище для предметов.
        /// </summary>
        Chest,

        /// <summary>
        /// Факел — источник света.
        /// </summary>
        Torch,

        /// <summary>
        /// Печка — инструмент для обработки ресурсов.
        /// </summary>
        Stove,

        /// <summary>
        /// Наковальня — инструмент для создания металлических предметов.
        /// </summary>
        Anvil,

        /// <summary>
        /// Растительность — декоративный элемент.
        /// </summary>
        Vegetation,

        /// <summary>
        /// Невидимая стена — используется для ограничения движения.
        /// </summary>
        InvisibleWall
    }
    /// <summary>
    /// Класс, представляющий плитку в игровом мире.
    /// </summary>
    public class Tile : ITile
    {
        /// <summary>
        /// Размер одной плитки в пикселях.
        /// </summary>
        public const int TileSize = 16;

        // Ссылки на соседние плитки
        protected Tile? _upTile;
        protected Tile? _downTile;
        protected Tile? _leftTile;
        protected Tile? _rightTile;

        // Ссылка на стену, связанную с плиткой
        protected WallTile? _wallTile;

        // Вершины для отрисовки плитки
        protected Vertex[] _vertices;

        /// <summary>
        /// Уникальный идентификатор плитки.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Тип плитки.
        /// </summary>
        public TileType Type { get; set; }

        /// <summary>
        /// Требуемый инструмент для разрушения плитки.
        /// </summary>
        public ItemType RequiredTool { get; set; }

        /// <summary>
        /// Требуемая мощность инструмента для разрушения плитки.
        /// </summary>
        public int RequiredToolPower { get; set; }

        /// <summary>
        /// Предмет, который выпадает при разрушении плитки.
        /// </summary>
        public ItemList DropItem { get; set; }

        /// <summary>
        /// Прочность плитки.
        /// </summary>
        public float Strength { get; set; }

        /// <summary>
        /// Является ли плитка твёрдой (непроходимой).
        /// </summary>
        public bool IsSolid { get; set; }

        /// <summary>
        /// Является ли плитка деревом.
        /// </summary>
        public bool IsTree { get; set; } = false;

        /// <summary>
        /// Является ли плитка верхушкой дерева.
        /// </summary>
        public bool IsTreeTop { get; set; } = false;

        /// <summary>
        /// Родительский чанк, в котором находится плитка.
        /// </summary>
        public Chunk PerentChunk { get; set; }

        /// <summary>
        /// Родительская плитка (если плитка состоит из нескольких частей).
        /// </summary>
        public Tile? PerentTile { get; set; }

        /// <summary>
        /// Список дочерних плиток (если плитка состоит из нескольких частей).
        /// </summary>
        public List<Tile> ChildTiles { get; set; } = new List<Tile>();

        /// <summary>
        /// Верхний сосед плитки.
        /// </summary>
        public Tile? UpTile
        {
            get => _upTile;
            set
            {
                _upTile = value;
                UpdateView(); // Обновление текстурных координат при изменении соседа
            }
        }

        /// <summary>
        /// Нижний сосед плитки.
        /// </summary>
        public Tile? DownTile
        {
            get => _downTile;
            set
            {
                _downTile = value;
                UpdateView();
            }
        }

        /// <summary>
        /// Левый сосед плитки.
        /// </summary>
        public Tile? LeftTile
        {
            get => _leftTile;
            set
            {
                _leftTile = value;
                UpdateView();
            }
        }

        /// <summary>
        /// Правый сосед плитки.
        /// </summary>
        public Tile? RightTile
        {
            get => _rightTile;
            set
            {
                _rightTile = value;
                UpdateView();
            }
        }

        /// <summary>
        /// Связанная стена.
        /// </summary>
        public WallTile? WallTile
        {
            get => _wallTile;
            set
            {
                _wallTile = value;
                UpdateView();
            }
        }

        /// <summary>
        /// Мировая позиция плитки.
        /// </summary>
        public Vector2f GlobalPosition { get; set; }

        /// <summary>
        /// Локальная позиция плитки в чанке.
        /// </summary>
        public Vector2f LocalPosition { get; set; }

        /// <summary>
        /// Размер плитки.
        /// </summary>
        public Vector2f Size { get; set; }

        /// <summary>
        /// Имя текстуры плитки.
        /// </summary>
        public string TextureName { get; set; }

        /// <summary>
        /// Конструктор плитки.
        /// </summary>
        /// <param name="type">Тип плитки.</param>
        /// <param name="dropItem">Предмет, выпадающий при разрушении.</param>
        /// <param name="requiredTool">Требуемый инструмент.</param>
        /// <param name="reqiuredToolPower">Мощность требуемого инструмента.</param>
        /// <param name="strength">Прочность плитки.</param>
        /// <param name="isSolid">Является ли плитка твёрдой.</param>
        /// <param name="perentChunk">Родительский чанк.</param>
        /// <param name="upTile">Верхний сосед.</param>
        /// <param name="downTile">Нижний сосед.</param>
        /// <param name="leftTile">Левый сосед.</param>
        /// <param name="rightTile">Правый сосед.</param>
        /// <param name="wall">Связанная стена.</param>
        /// <param name="localPosition">Локальная позиция плитки.</param>
        public Tile(TileType type, ItemList dropItem, ItemType requiredTool, int reqiuredToolPower, float strength, bool isSolid, Chunk perentChunk,
            Tile? upTile, Tile? downTile, Tile? leftTile, Tile? rightTile, WallTile? wall, Vector2f localPosition)
        {
            Type = type;
            DropItem = dropItem;
            RequiredTool = requiredTool;
            RequiredToolPower = reqiuredToolPower;
            Strength = strength;
            IsSolid = isSolid;
            LocalPosition = localPosition;

            PerentChunk = perentChunk;

            _vertices = new Vertex[6];

            TextureName = type.ToString();

            // Присваиваем соседей, а соседям эту плитку
            if (upTile != null)
            {
                _upTile = upTile;
                _upTile.DownTile = this;    // Для верхнего соседа эта плитка будет нижним соседом
            }
            if (downTile != null)
            {
                _downTile = downTile;
                _downTile.UpTile = this;    // Для нижнего соседа эта плитка будет верхним соседом
            }
            if (leftTile != null)
            {
                _leftTile = leftTile;
                _leftTile.RightTile = this;    // Для левого соседа эта плитка будет правым соседом
            }
            if (rightTile != null)
            {
                _rightTile = rightTile;
                _rightTile.LeftTile = this;    // Для правого соседа эта плитка будет левым соседом
            }

            WallTile = wall;

            UpdateView();
        }

        /// <summary>
        /// Обновляет текстурные координаты плитки в зависимости от её соседей.
        /// </summary>
        public virtual void UpdateView()
        {
            Vector2u texturePosFraq = new Vector2u(0, 0); // Позиция спрайта на текстурном атласе

            switch (Type)
            {
                default:
                    // Если у плитки есть все соседи
                    if (_upTile != null && _downTile != null && _leftTile != null && _rightTile != null)
                    {
                        int i = World.Random.Next(0, 3); // Случайное число от 0 до 2
                        texturePosFraq = new Vector2u(1 + (uint)i, 1);
                    }
                    // Если у плитки отсутствуют все соседи
                    else if (_upTile == null && _downTile == null && _leftTile == null && _rightTile == null)
                    {
                        int i = World.Random.Next(0, 3);
                        texturePosFraq = new Vector2u(9 + (uint)i, 3);
                    }

                    //---------------

                    // Если у плитки отсутствует только верхний сосед
                    else if (_upTile == null && _downTile != null && _leftTile != null && _rightTile != null)
                    {
                        int i = World.Random.Next(0, 3);
                        texturePosFraq = new Vector2u(1 + (uint)i, 0);
                    }
                    // Если у плитки отсутствует только нижний сосед
                    else if (_upTile != null && _downTile == null && _leftTile != null && _rightTile != null)
                    {
                        int i = World.Random.Next(0, 3);
                        texturePosFraq = new Vector2u(1 + (uint)i, 2);
                    }
                    // Если у плитки отсутствует только левый сосед
                    else if (_upTile != null && _downTile != null && _leftTile == null && _rightTile != null)
                    {
                        int i = World.Random.Next(0, 3);
                        texturePosFraq = new Vector2u(0, (uint)i);
                    }
                    // Если у плитки отсутствует только правый сосед
                    else if (_upTile != null && _downTile != null && _leftTile != null && _rightTile == null)
                    {
                        int i = World.Random.Next(0, 3);
                        texturePosFraq = new Vector2u(4, (uint)i);
                    }

                    //---------------

                    // Если у плитки отсутствует только верхний и левый сосед
                    else if (_upTile == null && _downTile != null && _leftTile == null && _rightTile != null)
                    {
                        int i = World.Random.Next(0, 3);
                        texturePosFraq = new Vector2u((uint)i * 2, 3);
                    }
                    // Если у плитки отсутствует только верхний и правый сосед
                    else if (_upTile == null && _downTile != null && _leftTile != null && _rightTile == null)
                    {
                        int i = World.Random.Next(0, 3);
                        texturePosFraq = new Vector2u(1 + (uint)i * 2, 3);
                    }
                    // Если у плитки отсутствует только нижний и левый сосед
                    else if (_upTile != null && _downTile == null && _leftTile == null && _rightTile != null)
                    {
                        int i = World.Random.Next(0, 3);
                        texturePosFraq = new Vector2u(0 + (uint)i * 2, 4);
                    }
                    // Если у плитки отсутствует только нижний и правый сосед
                    else if (_upTile != null && _downTile == null && _leftTile != null && _rightTile == null)
                    {
                        int i = World.Random.Next(0, 3);
                        texturePosFraq = new Vector2u(1 + (uint)i * 2, 4);
                    }
                    //Если есть только правая и леввая плитка
                    else if (_upTile == null && _downTile == null && _leftTile != null && _rightTile != null)
                    {
                        int i = World.Random.Next(0, 3);
                        texturePosFraq = new Vector2u(6 + (uint)i, 4);
                    }
                    //Если есть только верхняя и нижняя плитка
                    else if (_upTile != null && _downTile != null && _leftTile == null && _rightTile == null)
                    {
                        int i = World.Random.Next(0, 3);
                        texturePosFraq = new Vector2u(5, 0 + (uint)i);
                    }

                    //----------------------------------------------

                    //Если есть только верхний сосед
                    else if (_upTile != null && _downTile == null && _leftTile == null && _rightTile == null)
                    {
                        int i = World.Random.Next(0, 3);
                        texturePosFraq = new Vector2u(6, 3);
                    }
                    // Если есть только нижний сосед
                    else if (_upTile == null && _downTile != null && _leftTile == null && _rightTile == null)
                    {
                        int i = World.Random.Next(0, 3);
                        texturePosFraq = new Vector2u(6, 0);
                    }
                    // Если есть только левый сосед
                    else if (_upTile == null && _downTile == null && _leftTile != null && _rightTile == null)
                    {
                        int i = World.Random.Next(0, 3);
                        texturePosFraq = new Vector2u(12, 0 + (uint)i);
                    }
                    // Если есть только правый сосед
                    else if (_upTile == null && _downTile == null && _leftTile == null && _rightTile != null)
                    {
                        int i = World.Random.Next(0, 3); // Случайное число от 0 до 2
                        texturePosFraq = new Vector2u(9, 0 + (uint)i);
                    }
                    // Если есть только правый сосед
                    else if (_upTile == null && _downTile != null && _leftTile != null && _rightTile != null)
                    {
                        int i = World.Random.Next(0, 2); // Случайное число от 0 до 2
                        texturePosFraq = new Vector2u(1 + (uint)i, 0);
                    }
                    break;
            }

            // Получаем текстурные координаты
            int x = (int)(texturePosFraq.X * Tile.TileSize + texturePosFraq.X * 2);
            int y = (int)(texturePosFraq.Y * Tile.TileSize + texturePosFraq.Y * 2);

            _vertices[0] = new Vertex(new Vector2f(LocalPosition.X * Tile.TileSize, LocalPosition.Y * Tile.TileSize));
            _vertices[1] = new Vertex(new Vector2f(LocalPosition.X * Tile.TileSize, LocalPosition.Y * Tile.TileSize + Tile.TileSize));
            _vertices[2] = new Vertex(new Vector2f(LocalPosition.X * Tile.TileSize + Tile.TileSize, LocalPosition.Y * Tile.TileSize));
            _vertices[3] = new Vertex(new Vector2f(LocalPosition.X * Tile.TileSize + Tile.TileSize, LocalPosition.Y * Tile.TileSize));
            _vertices[4] = new Vertex(new Vector2f(LocalPosition.X * Tile.TileSize, LocalPosition.Y * Tile.TileSize + Tile.TileSize));
            _vertices[5] = new Vertex(new Vector2f(LocalPosition.X * Tile.TileSize + Tile.TileSize, LocalPosition.Y * Tile.TileSize + Tile.TileSize));

            _vertices[0].TexCoords = new Vector2f(x, y);

            _vertices[1].TexCoords = new Vector2f(x, Tile.TileSize + y);
            _vertices[2].TexCoords = new Vector2f(Tile.TileSize + x, y);
            _vertices[3].TexCoords = new Vector2f(Tile.TileSize + x, y);
            _vertices[4].TexCoords = new Vector2f(x, Tile.TileSize + y);

            _vertices[5].TexCoords = new Vector2f(Tile.TileSize + x, Tile.TileSize + y);

            PerentChunk?.UpdateTileViewByWorldPosition(GlobalPosition, _vertices);
        }

        /// <summary>
        /// Разрушение плитки.
        /// </summary>
        /// <param name="type">Тип инструмента.</param>
        /// <param name="itemPower">Мощность инструмента.</param>
        /// <param name="damage">Наносимый урон.</param>
        /// <returns>True, если плитка разрушена.</returns>
        public virtual bool Breaking(ItemType type, float itemPower, float damage)
        {
            if (type == RequiredTool && itemPower >= RequiredToolPower)
            {
                if (UpTile != null && this is not TileTree && UpTile is TileTree)
                    return false;

                Strength -= damage;
                if (Strength <= 0)
                    return true;
            }

            if(RequiredTool == ItemType.All || RequiredTool == ItemType.None)
            {
                Strength -= damage;
                if (Strength <= 0)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Проверяет, подходит ли инструмент для разрушения плитки.
        /// </summary>
        /// <param name="type">Тип инструмента.</param>
        /// <returns>True, если инструмент подходит.</returns>
        public bool IsRequiredTool(ItemType type)
        {
            if (RequiredTool == ItemType.All || RequiredTool == ItemType.None)
                return true;

            return RequiredTool == type;
        }

        /// <summary>
        /// Проверяет, подходит ли инструмент и его мощность для разрушения плитки.
        /// </summary>
        /// <param name="type">Тип инструмента.</param>
        /// <param name="power">Мощность инструмента.</param>
        /// <returns>True, если инструмент и мощность подходят.</returns>
        public bool IsRequiredToolAndPower(ItemType type, float power)
        {
            if ((RequiredTool == ItemType.All || RequiredTool == ItemType.None) && power >= RequiredToolPower)
                return true;

            return power >= RequiredToolPower && type == RequiredTool;
        }

        /// <summary>
        /// Возвращает вершины для отрисовки плитки.
        /// </summary>
        /// <returns>Массив вершин.</returns>
        public Vertex[] GetVertices() => _vertices;
    }
}
