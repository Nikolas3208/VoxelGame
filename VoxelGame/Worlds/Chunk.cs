using SFML.Graphics;
using SFML.System;
using System;
using VoxelGame.Item;
using VoxelGame.Physics;
using VoxelGame.Resources;
using VoxelGame.Worlds.Tile;

namespace VoxelGame.Worlds
{
    public class Chunk : Transformable, Drawable
    {
        /// <summary>
        /// Размер чанка
        /// </summary>
        public const int ChunkSize = 16;

        /// <summary>
        /// Мир (родитель)
        /// </summary>
        private World _world;

        /// <summary>
        /// Список плиток
        /// </summary>
        private Tile.Tile[] _tiles;

        /// <summary>
        /// Список плиток стен
        /// </summary>
        private WallTile[] _tilesWall;

        /// <summary>
        /// Список плиток сушностей
        /// </summary>
        private List<TileEntity> _tilesEntiy;

        /// <summary>
        /// Список тел
        /// </summary>
        private List<AABB> _colliders;

        /// <summary>
        /// Сетка плиток чанка
        /// </summary>
        private Dictionary<TileType, ChunkMesh> _chunkMeshs { get; }

        /// <summary>
        /// Сетка стен чанка
        /// </summary>
        private Dictionary<WallType, ChunkMesh> _chunkMeshsWall { get; }

        /// <summary>
        /// Локальная позиция чанка
        /// </summary>
        public int X, Y;

        /// <summary>
        /// Идентификатор чанка
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Чанк готов( создан )
        /// </summary>
        public bool IsComplate { get; set; }

        /// <summary>
        /// Цвет освещения плитки
        /// </summary>
        public byte[,] LightColorTile { get; set; } = new byte[ChunkSize, ChunkSize];

        /// <summary>
        /// Цвет освещения стенки
        /// </summary>
        public byte[,] LightColorWall { get; set; } = new byte[ChunkSize, ChunkSize];

        /// <summary>
        /// Чанк
        /// </summary>
        /// <param name="world"> Мир (родитель) </param>
        public Chunk(World world)
        {
            _world = world;

            _tiles = new Tile.Tile[ChunkSize * ChunkSize];
            _tilesWall = new WallTile[ChunkSize * ChunkSize];

            LightColorTile = new byte[ChunkSize, ChunkSize];
            LightColorWall = new byte[ChunkSize, ChunkSize];

            _tilesEntiy = new List<TileEntity>();

            _colliders = new List<AABB>(ChunkSize * ChunkSize);
            _chunkMeshs = new Dictionary<TileType, ChunkMesh>();
            _chunkMeshsWall = new Dictionary<WallType, ChunkMesh>();
        }

        /// <summary>
        /// В чанке?
        /// </summary>
        /// <param name="x"> Х координата </param>
        /// <param name="y"> У координата </param>
        /// <returns> True если не выходит за размеры чанка или больше -1 </returns>
        public bool InBounds(int x, int y)
        {
            return x >= 0 && x < ChunkSize && y >= 0 && y < ChunkSize;
        }

        ///----------------------------Плитки---------------------------///
        
        /// <summary>
        /// Установить плитку в локальных координатах, по типу плитки
        /// </summary>
        /// <param name="x"> Позиция по Х </param>
        /// <param name="y"> Позиция по У </param>
        /// <param name="type"> Тип плитки </param>
        /// <returns> True если плитка установлена </returns>
        public bool SetTile(int x, int y, TileType type, bool isPlayer = false, bool setWall = false)
        {
            if(!InBounds(x, y))
                return false;

            var upTile = GetTile(x, y - 1, true);
            var downTile = GetTile(x, y + 1, true);
            var leftTile = GetTile(x - 1, y, true);
            var rightTile = GetTile(x + 1, y, true);

            if (type != TileType.None)
            {
                switch(type)
                {
                    case TileType.Workbench:
                        return SetWorkbench(x, y, type);
                    case TileType.Stove:
                        return SetStove(x, y, type);
                    case TileType.Anvil:
                        return SetAnvil(x, y, type);
                }

                var tile = Tiles.GetTile(type, this, upTile, downTile, leftTile, rightTile, new Vector2f(x, y));
                tile.GlobalPosition = new Vector2f(x, y) * Tile.Tile.TileSize + Position;

                int index = x * ChunkSize + y;

                if (_tiles[index] != null)
                {
                    if (_chunkMeshs.ContainsKey(_tiles[index].Type))
                    {
                        SetTileToMesh(x, y, _tiles[index].Type, new Vertex[6]);
                    }
                }

                SetTileToMesh(x, y, type, tile.GetVertices());

                _tiles[index] = tile;

                if (setWall)
                    return SetWall(x, y, Tiles.TileTypeToWallType(type));
            }
            else
            {
                int index = x * ChunkSize + y;

                if (_tiles[index] != null)
                {
                    if (_chunkMeshs.ContainsKey(_tiles[index].Type))
                    {
                        SetTileToMesh(x, y, _tiles[index].Type, new Vertex[6]);
                    }
                    _world.DropItem(_tiles[index].DropItem, _tiles[index].GlobalPosition);

                    _tiles[index] = null;

                    if (upTile != null)
                        upTile!.DownTile = null;
                    if (downTile != null)
                        downTile!.UpTile = null;
                    if (leftTile != null)
                        leftTile!.RightTile = null;
                    if (rightTile != null)
                        rightTile!.LeftTile = null;
                }
            }

            if (IsComplate)
                GenerateColliders();

            return true;
        }

        /// <summary>
        /// Установить плитку в локальных координатах, по мировым координатам, по типу плитки
        /// </summary>
        /// <param name="position"> Позиция </param>
        /// <param name="tile"> Плитка </param>
        /// <returns> True если плитка установленна </returns>
        public bool SetTileByWorldPosition(Vector2f position, TileType type, bool isPlayer = false)
        {
            int x = (int)((position.X - Position.X) / Tile.Tile.TileSize);
            int y = (int)((position.Y - Position.Y) / Tile.Tile.TileSize);

            return SetTile(x, y, type, isPlayer: isPlayer);
        }

        public bool SetWorkbench(int x, int y, TileType type)
        {
            var upTile = GetTile(x, y - 1, true);
            var downTile = GetTile(x, y + 1, true);
            var leftTile = GetTile(x - 1, y, true);
            var rightTile = GetTile(x + 1, y, true);

            if(leftTile == null)
            {
                var tile = Tiles.GetTile(type, this, upTile, downTile, leftTile, rightTile, new Vector2f(x, y));
                tile.GlobalPosition = new Vector2f(x, y) * Tile.Tile.TileSize + Position;

                SetTileToMesh(x, y, type, tile.GetVertices());

                int index = x * ChunkSize + y;

                _tiles[index] = tile;

                upTile = GetTile(x - 1, y - 1, true);
                downTile = GetTile(x - 1, y + 1, true);
                leftTile = GetTile(x - 2, y, true);
                rightTile = GetTile(x, y, true);

                var tileLeft = Tiles.GetTile(type, this, upTile, downTile, leftTile, rightTile, new Vector2f(x - 1, y));
                tileLeft.GlobalPosition = new Vector2f(x - 1, y) * Tile.Tile.TileSize + Position;
                tileLeft.PerentTile = tile;

                SetTileToMesh(x - 1, y, type, tileLeft.GetVertices());

                index = (x - 1) * ChunkSize + y;

                if (index < 0)
                    return _world.GetChunk(X - 1, Y) != null ?_world.GetChunk(X - 1, Y)!.SetWorkbench(ChunkSize + (x - 1), y, type) : false;

                _tiles[index] = tileLeft;

                return true;
            }
            else if (rightTile == null)
            {
                var tile = Tiles.GetTile(type, this, upTile, downTile, leftTile, rightTile, new Vector2f(x, y));
                tile.GlobalPosition = new Vector2f(x, y) * Tile.Tile.TileSize + Position;

                SetTileToMesh(x, y, type, tile.GetVertices());

                int index = x * ChunkSize + y;

                _tiles[index] = tile;

                upTile = GetTile(x + 1, y - 1, true);
                downTile = GetTile(x + 1, y + 1, true);
                leftTile = GetTile(x, y, true);
                rightTile = GetTile(x + 2, y, true);

                var tileRight = Tiles.GetTile(type, this, upTile, downTile, leftTile, rightTile, new Vector2f(x + 1, y));
                tileRight.GlobalPosition = new Vector2f(x + 1, y) * Tile.Tile.TileSize + Position;
                tileRight.PerentTile = tile;

                SetTileToMesh(x + 1, y, type, tileRight.GetVertices());

                index = (x + 1) * ChunkSize + y;

                _tiles[index] = tileRight;

                return true;
            }

            return false;
        }

        public bool BreakingWorkbench(int x, int y)
        {
            var tile = GetTile(x, y);
            var leftTile = GetTile(x - 1, y, true);
            var rightTile = GetTile(x + 1, y, true);

            if (tile != null && tile is TileWorkbench)
            {
                SetTile(x, y, TileType.None);
                if (leftTile != null && leftTile is TileWorkbench)
                {
                    var upTile = GetTile(x - 1, y - 1, true);
                    var downTile = GetTile(x - 1, y + 1, true);
                    leftTile = GetTile(x - 2, y, true);
                    rightTile = GetTile(x, y, true);

                    int index = (x - 1) * ChunkSize + y;

                    if (_tiles[index] != null)
                    {
                        if (_chunkMeshs.ContainsKey(_tiles[index].Type))
                        {
                            SetTileToMesh(x - 1, y, _tiles[index].Type, new Vertex[6]);
                        }

                        _tiles[index] = null;

                        if (upTile != null)
                            upTile!.DownTile = null;
                        if (downTile != null)
                            downTile!.UpTile = null;
                        if (leftTile != null)
                            leftTile!.RightTile = null;
                        if (rightTile != null)
                            rightTile!.LeftTile = null;

                        return true;
                    }
                }
                else if(rightTile != null && rightTile is TileWorkbench)
                {
                    var upTile = GetTile(x + 1, y - 1, true);
                    var downTile = GetTile(x + 1, y + 1, true);
                    leftTile = GetTile(x, y, true);
                    rightTile = GetTile(x + 2, y, true);

                    int index = (x + 1) * ChunkSize + y;

                    if (_tiles[index] != null)
                    {
                        if (_chunkMeshs.ContainsKey(_tiles[index].Type))
                        {
                            SetTileToMesh(x + 1, y, _tiles[index].Type, new Vertex[6]);
                        }

                        _tiles[index] = null;

                        if (upTile != null)
                            upTile!.DownTile = null;
                        if (downTile != null)
                            downTile!.UpTile = null;
                        if (leftTile != null)
                            leftTile!.RightTile = null;
                        if (rightTile != null)
                            rightTile!.LeftTile = null;

                        return true;
                    }
                }
            }

            return false;
        }

        public bool SetStove(int x, int y, TileType type)
        {
            return true;
        }

        public bool SetAnvil(int x, int y, TileType type)
        {
            return true;
        }

        /// <summary>
        /// Установить плитку в локальных координатах сетки, по типу плитки
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="type"></param>
        /// <param name="vertices"></param>
        public void SetTileToMesh(int x, int y, TileType type, Vertex[] vertices)
        {
            if(_chunkMeshs.ContainsKey(type))
            {
                _chunkMeshs[type].SetToMesh(x, y, vertices);
            }
            else
            {
                var chunkMesh = new ChunkMesh(this);
                chunkMesh.SetToMesh(x, y, vertices);

                _chunkMeshs.Add(type, chunkMesh);
            }
        }

        /// <summary>
        /// Получить плитку по локальным координатам
        /// </summary>
        /// <param name="x"> Позиция по Х </param>
        /// <param name="y"> Позиция по У </param>
        /// <returns> Плитку, null если х,у вышли за рамки чанка или если плитка null </returns>
        public Tile.Tile? GetTile(int x, int y, bool getOtherChunk = false)
        {
            if (!InBounds(x, y) && !getOtherChunk)
            {
                return null;
            }
            else if(getOtherChunk)
            {
                if (x < 0)
                {
                   return _world.GetChunk(X - 1, Y)?.GetTile(ChunkSize + x, y);
                }
                else if (x >= ChunkSize)
                {
                    return _world.GetChunk(X + 1, Y)?.GetTile(x - ChunkSize, y);
                }
                else if (y < 0)
                {
                    return _world.GetChunk(X, Y - 1)?.GetTile(x, ChunkSize + y);
                }
                else if (y >= ChunkSize)
                {
                    return _world.GetChunk(X, Y + 1)?.GetTile(x, y - ChunkSize);
                }
            }


            return _tiles[x * ChunkSize + y];
        }

        /// <summary>
        /// Получить плитку по мировым координатам
        /// </summary>
        /// <param name="position"> Позиция </param>
        /// <returns> Плитку, null если х,у вышли за рамки чанка или если плитка null </returns>
        public Tile.Tile? GetTileByWorldPosition(Vector2f position, bool getOtherChunk = false)
        {
            int x = (int)((position.X - Position.X) / Tile.Tile.TileSize);
            int y = (int)((position.Y - Position.Y) / Tile.Tile.TileSize);

            return GetTile(x, y, getOtherChunk);
        }

        /// <summary>
        /// Получить плитку по типу плитки
        /// </summary>
        /// <param name="type"> Тип плитки </param>
        /// <returns></returns>
        public Tile.Tile? GetTileByType(TileType type)
        {
            return _tiles.FirstOrDefault(t => t != null && t.Type == type);
        }

        /// <summary>
        /// Получить список плиток по типу плитки
        /// </summary>
        /// <param name="type"> Тип плитки </param>
        /// <returns></returns>
        public List<Tile.Tile> GetTilesByType(TileType type)
        {
            return _tiles.Where(t => t != null && t.Type == type).ToList();
        }

        /// <summary>
        /// Ломаем плитку, в локальных координатах
        /// </summary>
        /// <param name="x"> Х координата </param>
        /// <param name="y"> У координата </param>
        /// <param name="type"> Тип предмета которым ломаем </param>
        /// <param name="itemPower"> Мощность предмета </param>
        /// <param name="damage"> Урон по плитки </param>
        /// <returns></returns>
        public bool BreakingTile(int x, int y, ItemType type, float itemPower, float damage)
        {
            var tile = GetTile(x, y);

            if (tile != null)
            {
                if(tile.BreakingTile(type, itemPower, damage))
                {
                    if(tile.IsTree)
                    {
                        ChoppingTree(tile, x, y);
                    }

                    switch(tile.Type)
                    {
                        case TileType.Workbench:
                            return BreakingWorkbench(x, y);
                    }

                    return SetTile(x, y, TileType.None);
                }
            }

            return false;
        }

        /// <summary>
        /// Ломаем плитку, в мировых координатах
        /// </summary>
        /// <param name="position"> Позиия плитки </param>
        /// <param name="type"> Тип предмета которым ломаем </param>
        /// <param name="itemPower"> Мощность предмета </param>
        /// <param name="damage"> Урон по плитки </param>
        /// <returns></returns>
        public bool BreakingTileByWorldPosition(Vector2f position, ItemType type, float itemPower, float damage)
        {
            int x = (int)((position.X - Position.X) / Tile.Tile.TileSize);
            int y = (int)((position.Y - Position.Y) / Tile.Tile.TileSize);

            return BreakingTile(x, y, type, itemPower, damage);
        }

        /// <summary>
        /// Обновляем вид плитки, в локальных координатах
        /// </summary>
        /// <param name="x"> Х координата </param>
        /// <param name="y"> У координата </param>
        /// <param name="vertices"> Массив вершин </param>
        public void UpdateTileView(int x, int y, Vertex[] vertices)
        {
            var tile = GetTile(x, y);

            if (tile == null)
                return;

            _chunkMeshs[tile.Type].UpdateViewMesh(x, y, vertices);
        }

        /// <summary>
        /// Обновляем вид плитки, в мировых координатах
        /// </summary>
        /// <param name="position"> Позиция плитки </param>
        /// <param name="vertices"> Массив вершин </param>
        public void UpdateTileViewByWorldPosition(Vector2f position, Vertex[] vertices)
        {
            int x = (int)((position.X - Position.X) / Tile.Tile.TileSize);
            int y = (int)((position.Y - Position.Y) / Tile.Tile.TileSize);

            UpdateTileView(x, y, vertices);
        }

        /// <summary>
        /// Обновляем цвет плитки, в локальных координатах
        /// </summary>
        /// <param name="color"> Цвет </param>
        /// <param name="x"> Х координата </param>
        /// <param name="y"> У координата </param>
        public void UpdateTileColor(Color color, int x, int y)
        {
            var tile = GetTile(x, y);

            if (tile != null && _chunkMeshs.ContainsKey(tile.Type))
            {
                _chunkMeshs[tile.Type].UpdateTileColor(color, x, y);
            }
        }

        /// <summary>
        /// Получить цвет плитки, по локальным координатам
        /// </summary>
        /// <param name="x"> Х координата </param>
        /// <param name="y"> У координата </param>
        /// <returns></returns>
        public Color GetTileColor(int x, int y)
        {
            var tile = GetTile(x, y);

            if (tile == null || !_chunkMeshs.ContainsKey(tile.Type))
                return Color.Black;

            return _chunkMeshs[tile.Type].GetTileColor(x, y);
        }


        ///------------------------------Стенки-------------------------///

        /// <summary>
        /// Установить стенку в локальных координатах, по типу стенки
        /// </summary>
        /// <param name="type"> Тип стенки </param>
        /// <param name="x"> Позиция по Х </param>
        /// <param name="y"> Позиция по У </param>
        public bool SetWall(int x, int y, WallType type)
        {
            if (!InBounds(x, y))
                return false;

            var upWall = GetWall(x, y - 1, true);
            var downWall = GetWall(x, y + 1, true);
            var leftWall = GetWall(x - 1, y, true);
            var rightWall = GetWall(x + 1, y, true);

            if (type != WallType.None)
            {
                var wall = Tiles.GetWall(type, this, upWall, downWall, leftWall, rightWall, new Vector2f(x, y));
                wall.GlobalPosition = new Vector2f(x, y) * Tile.Tile.TileSize + Position;

                int index = x * ChunkSize + y;

                _tilesWall[index] = wall;

                SetWallToMesh(x, y, type, wall.GetVertices());
            }
            else
            {
                int index = x * ChunkSize + y;
                
                if (_tilesWall[index] != null)
                {
                    if (_chunkMeshsWall.ContainsKey(_tilesWall[index].Type))
                    {
                         SetWallToMesh(x, y, _tilesWall[index].Type, new Vertex[6]);
                    }

                    _tilesWall[index] = null;

                    if (upWall != null)
                        upWall!.DownWall = null;
                    if (downWall != null)
                        downWall!.UpWall = null;
                    if (leftWall != null)
                        leftWall!.RightWall = null;
                    if (rightWall != null)
                        rightWall!.LeftWall = null;
                }
            }

            return true;
        }

        /// <summary>
        /// Установить стенку в локальных координатах, по мировым координатам, по типу стенки
        /// </summary>
        /// <param name="position"> Позиия в мировых координатах </param>
        /// <param name="type"> Тип стенки </param>
        /// <returns></returns>
        public bool SetWallByWorldPosition(Vector2f position, WallType type)
        {
            int x = (int)((position.X - Position.X) / Tile.Tile.TileSize);
            int y = (int)((position.Y - Position.Y) / Tile.Tile.TileSize);

            return SetWall(x, y, type);
        }

        /// <summary>
        /// Установить стенку в локальных координатах, в сетке, по типу стенки
        /// </summary>
        /// <param name="x"> Позиция по Х </param>
        /// <param name="y"> Позиция по У </param>
        /// <param name="type"> Тип стенки </param>
        /// <param name="vertices"> Вершины </param>
        public void SetWallToMesh(int x, int y, WallType type, Vertex[] vertices)
        {
            if (_chunkMeshsWall.ContainsKey(type))
            {
                _chunkMeshsWall[type].SetToMesh(x, y, vertices);
            }
            else
            {
                var chunkMesh = new ChunkMesh(this);
                chunkMesh.SetToMesh(x, y, vertices);

                _chunkMeshsWall.Add(type, chunkMesh);
            }
        }

        /// <summary>
        /// Получить стенку по локальным координатам
        /// </summary>
        /// <param name="x"> Позиция по X </param>
        /// <param name="y"> Позиция по Y </param>
        /// <param name="getOtherChunk"> Проверять соседнии чанки? </param>
        /// <returns></returns>
        public WallTile? GetWall(int x, int y, bool getOtherChunk = false)
        {
            if (!InBounds(x, y) && !getOtherChunk)
            {
                return null;
            }
            else if (getOtherChunk)
            {
                if (x < 0)
                {
                    return _world.GetChunk(X - 1, Y)?.GetWall(ChunkSize + x, y);
                }
                else if (x >= ChunkSize)
                {
                    return _world.GetChunk(X + 1, Y)?.GetWall(x - ChunkSize, y);
                }
                else if (y < 0)
                {
                    return _world.GetChunk(X, Y - 1)?.GetWall(x, ChunkSize + y);
                }
                else if (y >= ChunkSize)
                {
                    return _world.GetChunk(X, Y + 1)?.GetWall(x, y - ChunkSize);
                }
            }


            return _tilesWall[x * ChunkSize + y];
        }

        /// <summary>
        /// Получить стенку по мировым координатам
        /// </summary>
        /// <param name="position"> Позиция в мировых координатах </param>
        /// <param name="getOtherChunk"> Проверять соседнии чанки? </param>
        /// <returns></returns>
        public WallTile? GetWallByWorldPosition(Vector2f position, bool getOtherChunk = false)
        {
            int x = (int)((position.X - Position.X) / Tile.Tile.TileSize);
            int y = (int)((position.Y - Position.Y) / Tile.Tile.TileSize);

            return GetWall(x, y, getOtherChunk);
        }

        /// <summary>
        /// Обновляем вид стенки, в локальных координатах
        /// </summary>
        /// <param name="x"> Позиция по X </param>
        /// <param name="y"> Позиция по Y </param>
        /// <param name="vertices"> Массив вершин </param>
        public void UpdateWallView(int x, int y, Vertex[] vertices)
        {
            var tile = GetWall(x, y);

            if (tile == null)
                return;

            _chunkMeshsWall[tile.Type].UpdateViewMesh(x, y, vertices);
        }

        /// <summary>
        /// Обновляем вид стенки, в мировых координатах
        /// </summary>
        /// <param name="position"> Позиция стенки в мировых координатах </param>
        /// <param name="vertices"> Массив вершин </param>
        public void UpdateWallViewByWorldPosition(Vector2f position, Vertex[] vertices)
        {
            int x = (int)((position.X - Position.X) / Tile.Tile.TileSize);
            int y = (int)((position.Y - Position.Y) / Tile.Tile.TileSize);

            UpdateWallView(x, y, vertices);
        }

        /// <summary>
        /// Обновляем цвет стенки, в локальных координатах
        /// </summary>
        /// <param name="color"> Цвет </param>
        /// <param name="x"> Позиция по X </param>
        /// <param name="y"> Позиция по Y </param>
        public void UpdateWallColor(Color color, int x, int y)
        {
            var wall = GetWall(x, y);

            if (wall != null && _chunkMeshsWall.ContainsKey(wall.Type))
            {
                _chunkMeshsWall[wall.Type].UpdateTileColor(color, x, y);
            }
        }

        /// <summary>
        /// Получить цвет стенки, по локальным координатам
        /// </summary>
        /// <param name="x"> Позиция по X </param>
        /// <param name="y"> Позиция по Y </param>
        /// <returns></returns>
        public Color GetWallColor(int x, int y)
        {
            var wall = GetWall(x, y);

            if (wall == null || !_chunkMeshsWall.ContainsKey(wall.Type))
                return Color.Black;

            return _chunkMeshsWall[wall.Type].GetTileColor(x, y);
        }

        /// <summary>
        /// Рубим дерево
        /// </summary>
        /// <param name="treeTile"> Плитка дерева </param>
        /// <param name="x"> Позиция по X </param>
        /// <param name="startY"> Позиция по Y </param>
        /// <returns></returns>
        public bool ChoppingTree(Tile.Tile treeTile, int x, int startY)
        {
            for (int y = 0; treeTile != null && (treeTile.IsTree || treeTile.IsTreeTop); y--)
            {
                if (y != 0)
                    treeTile = GetTile(x, startY + y, true)!;

                if (treeTile != null)
                {
                    if(!SetTile(x, startY + y, TileType.None))
                    {
                        _world.GetChunk(X, Y - 1)?.SetTile(x, ChunkSize + (startY + y), TileType.None);
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Создать коллайдеры для чанка
        /// </summary>
        public void GenerateColliders()
        {
            _colliders.Clear();
            Vector2f[] vertices = new Vector2f[4];

            for (int i = 0; i < 4; i++)
                vertices[i] = new Vector2f(-1, -1);

            for (int x = 0; x < ChunkSize; x++)
            {
                for (int y = 0; y < ChunkSize; y++)
                {
                    var tile = GetTile(x, y);
                    var upTile = GetTile(x, y + 1);
                    var downTile = GetTile(x, y - 1);
                    var leftTile = GetTile(x - 1, y);
                    var rightTile = GetTile(x + 1, y);

                    if (tile == null || !tile.IsSolid)
                        continue;

                    if ((upTile == null || !upTile.IsSolid) || (downTile == null || !downTile.IsSolid) || (leftTile == null || !leftTile.IsSolid) || (rightTile == null || !rightTile.IsSolid))
                    {
                        _colliders.Add(new AABB(tile.GlobalPosition, tile.GlobalPosition + new Vector2f(Tile.Tile.TileSize, Tile.Tile.TileSize)));
                    }
                }
            }
        }

        /// <summary>
        /// Обновляем чанк
        /// </summary>
        /// <param name="deltaTime"> Время кадра </param>
        public void Update(float deltaTime)
        {
            foreach(var e in _tilesEntiy)
            {
                e.Update(deltaTime);
            }
        }

        /// <summary>
        /// Рисуем стены чанка
        /// </summary>
        /// <param name="target"></param>
        /// <param name="states"></param>
        public void DrawWall(RenderTarget target, RenderStates states)
        {
            states.Transform *= Transform;

            foreach (var chunkMesh in _chunkMeshsWall)
            {
                states.Texture = AssetManager.GetTexture(chunkMesh.Key.ToString());

                target.Draw(chunkMesh.Value.GetChunkMesh(), PrimitiveType.Triangles, states);
            }

            foreach (var e in _tilesEntiy)
                e.Draw(target, states);
        }

        /// <summary>
        /// Рисуем стены чанка
        /// </summary>
        /// <param name="target"></param>
        /// <param name="states"></param>
        public void DrawTile(RenderTarget target, RenderStates states)
        {
            states.Transform *= Transform;

            foreach (var chunkMesh in _chunkMeshs)
            {
                states.Texture = AssetManager.GetTexture(chunkMesh.Key.ToString());

                target.Draw(chunkMesh.Value.GetChunkMesh(), PrimitiveType.Triangles, states);
            }
        }

        /// <summary>
        /// Получить AABB чанка
        /// </summary>
        /// <returns> AABB </returns>
        public AABB GetAABB()
        {
            return new AABB(Position.X, Position.Y, Position.X + ChunkSize * Tile.Tile.TileSize, Position.Y + ChunkSize * Tile.Tile.TileSize);
        }

        /// <summary>
        /// Получить список колайдеров
        /// <summary>
        public List<AABB> GetColliders()
        {
            return _colliders;
        }

        /// <summary>
        /// Руализуем интерфейс Drawable
        /// </summary>
        /// <param name="target"></param>
        /// <param name="states"></param>
        public void Draw(RenderTarget target, RenderStates states)
        {
        }
    }
}
