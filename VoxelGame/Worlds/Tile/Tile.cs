using SFML.Graphics;
using SFML.System;
using VoxelGame.Item;

namespace VoxelGame.Worlds.Tile
{
    public enum TileType
    {
        None,
        Ground,
        Grass,
        Stone,
        Oak,
        Birch,
        Leaves,
        IronOre,
        CopperOre,
        IronIngot,
        CopperIngot,
        OakBoard,
        BirchBoard,
        Door,
        Workbench,
        Chest,
        Torch,
        Stove,
        Anvil,
        Vegetation
    }
    public class Tile : ITile
    {
        public const int TileSize = 16; // Размер плитки

        protected Tile? _upTile;
        protected Tile? _downTile;
        protected Tile? _leftTile;
        protected Tile? _rightTile;

        protected WallTile? _wallTile;

        protected Vertex[] _vertices;

        /// <summary>
        /// Id плитки
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Тип плитки
        /// </summary>
        public TileType Type { get; set; }

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
        /// Является ли плитка деревом
        /// </summary>
        public bool IsTree { get; set; } = false;

        /// <summary>
        /// Является ли плитка верхушкой дерева
        /// </summary>
        public bool IsTreeTop { get; set; } = false;

        /// <summary>
        /// Родительский чанк
        /// </summary>
        public Chunk PerentChunk { get; set; }

        /// <summary>
        /// Если плитка состоит из нескольких, добавляем им всем родителя
        /// </summary>
        public Tile? PerentTile { get; set; }

        /// <summary>
        /// Верхний сосед
        /// </summary>
        public Tile? UpTile 
        { 
            get => _upTile;
            set
            {
                _upTile = value;
                UpdateView();
            }
        }

        /// <summary>
        /// Нижний сосед
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
        /// Левый сосед
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
        /// Правый сосед
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
        /// Мировая позиция плитки
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

        public string TextureName { get; set; } 

        /// <summary>
        /// Плитка
        /// </summary>
        /// <param name="type"> Тип плитки </param>
        /// <param name="requiredTool"> Требуемый инструмент </param>
        /// <param name="reqiuredToolPower"> Мощность требуемого инструмента </param>
        /// <param name="strength"> Прочность плитки </param>
        /// <param name="isSolid"> Плитка твердая? </param>
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
        /// Обновляем текстурные координаты
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
        /// Лобаем плитку
        /// </summary>
        /// <param name="type"> Тип предмета </param>
        /// <param name="itemPower"> Мошность предмета </param>
        /// <returns> Если здоровье плитки 0 или меньше true </returns>
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

        public Vertex[] GetVertices() => _vertices;
    }
}
