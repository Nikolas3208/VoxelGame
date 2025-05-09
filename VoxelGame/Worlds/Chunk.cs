using SFML.Graphics;
using SFML.System;
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
        /// Массив плиток
        /// </summary>
        private InfoTile[] _tiles;

        /// <summary>
        /// Список плиток сушностей
        /// </summary>
        private List<TileEntity> _tilesEntiy;

        /// <summary>
        /// Список тел
        /// </summary>
        private List<AABB> _colliders;

        /// <summary>
        /// Сетка чанка
        /// </summary>
        private ChunkMesh _chunkMesh { get; }

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
        /// Чанк
        /// </summary>
        /// <param name="world"> Мир (родитель) </param>
        public Chunk(World world)
        {
            _world = world;

            _tiles = new InfoTile[ChunkSize * ChunkSize];
            _tilesEntiy = new List<TileEntity>();

            _colliders = new List<AABB>(ChunkSize * ChunkSize);
            _chunkMesh = new ChunkMesh(this);
        }

        /// <summary>
        /// Установить плитку в локальных координатах
        /// </summary>
        /// <param name="x"> Позиция по Х </param>
        /// <param name="y"> Позиия по У </param>
        /// <param name="tile"> Поитка </param>
        /// <returns> True если плитка установлена </returns>
        public bool SetTile(int x, int y, InfoTile? tile, bool isPlayer = false, bool isWall = false)
        {
            // Проверяем на валидность координаты
            if (x < 0 || x >= ChunkSize || y < 0 || y >= ChunkSize)
                return false;

            // Находим индекс плитки в массиве
            int index = x * ChunkSize + y;

            // Если плитка не null, то устанавливаем ей индекс
            if (tile != null)
            {
                tile!.Id = index;
                tile!.IsWall = isWall;
                tile!.Chunk = this;
            }
            else if (_tiles[index] != null && IsComplate)
            {
                _world.DropItem(_tiles[index],
                    new Vector2f(x, y) * InfoTile.TileSize + Position + new Vector2f(InfoTile.TileSize, InfoTile.TileSize) / 2);
            }

            if (!IsComplate || !isPlayer)
            {
                // Устанавливаем плитку в массив
                _tiles[index] = tile!;

                // Устанавливаем плитку в сетку чанка
                if (isWall)
                {
                    _chunkMesh.SetTileToMesh(x, y, tile!, Color.Black);
                }
                else
                {
                    _chunkMesh.SetTileToMesh(x, y, tile!, Color.Black);
                }
            }
            else if(isPlayer && IsComplate)
            {
                var upTile = GetTile(x, y + 1);
                var downTile = GetTile(x, y - 1);
                var leftTile = GetTile(x - 1, y);
                var rightTile = GetTile(x + 1, y);

                if (upTile != null || downTile != null || leftTile != null || rightTile != null)
                {
                    // Устанавливаем плитку в массив
                    _tiles[index] = tile!;

                    // Устанавливаем плитку в сетку чанка
                    if (isWall)
                    {
                        _chunkMesh.SetTileToMesh(x, y, tile!, Color.Black);
                    }
                    else
                    {
                        _chunkMesh.SetTileToMesh(x, y, tile!, Color.Black);
                    }
                }
                else
                    return false;
            }

            if(IsComplate)
            {
                if (!isWall)
                    GenerateColliders();
            }

            return true;
        }

        /// <summary>
        /// Установить плитку в локальных координатах, по типу плитки
        /// </summary>
        /// <param name="x"> Позиция по Х </param>
        /// <param name="y"> Позиция по У </param>
        /// <param name="type"> Тип плитки </param>
        /// <returns> True если плитка установлена </returns>
        public bool SetTile(int x, int y, TileType type, bool isPlayer = false, bool isWall = false)
        {
            return SetTile(x, y, Tiles.GetTile(type, isWall), isPlayer, isWall);
        }

        /// <summary>
        /// Установить плитку в локальных координатах, по мировым координатам
        /// </summary>
        /// <param name="position"> Позиция </param>
        /// <param name="tile"> Плитка </param>
        /// <returns> True если плитка установленна </returns>
        public bool SetTileByWorldPosition(Vector2f position, InfoTile? tile, bool isWall = false)
        {
            int x = (int)((position.X - Position.X) / InfoTile.TileSize);
            int y = (int)((position.Y - Position.Y) / InfoTile.TileSize);

            if (x < 0 || x >= ChunkSize || y < 0 || y >= ChunkSize)
                return false;

            return SetTile(x, y, tile, isWall);
        }

        /// <summary>
        /// Установить плитку в локальных координатах, по мировым координатам, по типу плитки
        /// </summary>
        /// <param name="position"> Позиция </param>
        /// <param name="tile"> Плитка </param>
        /// <returns> True если плитка установленна </returns>
        public bool SetTileByWorldPosition(Vector2f position, TileType type, bool isWall = false)
        {
            int x = (int)((position.X - Position.X) / InfoTile.TileSize);
            int y = (int)((position.Y - Position.Y) / InfoTile.TileSize);

            if(type == TileType.Chest)
            {
                return AddTileEntity(new ChestEntity() { Chunk = this, Position = new Vector2f(x, y) * InfoTile.TileSize, Id = x * ChunkSize + y });
            }

            return SetTile(x, y, type, isWall: isWall);
        }

        /// <summary>
        /// Получить плитку по локальным координатам
        /// </summary>
        /// <param name="x"> Позиция по Х </param>
        /// <param name="y"> Позиция по У </param>
        /// <returns> Плитку, null если х,у вышли за рамки чанка или если плитка null </returns>
        public InfoTile? GetTile(int x, int y)
        {
            if(y < 0)
            {
                var upChunk = _world.GetChunk(X, Y - 1);
                return upChunk != null ? upChunk.GetTile(x, ChunkSize + y) : null;
            }
            else if(y >= ChunkSize)
            {
                var downChunk = _world.GetChunk(X, Y + 1);
                return downChunk != null ? downChunk.GetTile(x, y - ChunkSize) : null;
            }
            else if(x < 0)
            {
                var leftChunk = _world.GetChunk(X - 1, Y);
                return leftChunk != null ? leftChunk.GetTile(ChunkSize + x, y) : null;
            }
            else if(x >= ChunkSize)
            {
                var rightChunk = _world.GetChunk(X + 1, Y);
                return rightChunk != null ? rightChunk.GetTile(x - ChunkSize, y) : null;
            }

            return _tiles[x * ChunkSize + y];
        }

        /// <summary>
        /// Получить плитку по мировым координатам
        /// </summary>
        /// <param name="position"> Позиция </param>
        /// <returns> Плитку, null если х,у вышли за рамки чанка или если плитка null </returns>
        public InfoTile? GetTileByWorldPosition(Vector2f position)
        {
            int x = (int)((position.X - Position.X) / InfoTile.TileSize);
            int y = (int)((position.Y - Position.Y) / InfoTile.TileSize);
            return GetTile(x, y);
        }

        public bool BreakingTail(int x, int y, float damage)
        {
            var tile = GetTile(x, y);

            if (tile != null)
            {
                if(tile.BreakingTail(damage) == null)
                {
                    SetTile(x, y, null);

                    return true;
                }
            }

            return false;
        }

        public bool BreakingTailByWorldPosition(Vector2f position, float damage)
        {
            var tile = GetTileByWorldPosition(position);

            if (tile != null)
            {
                if (tile.BreakingTail(damage) == null)
                {
                    SetTileByWorldPosition(position, null);

                    return true;
                }
            }

            return false;
        }

        public bool AddTileEntity(TileEntity tileEntity)
        {
            if (_tilesEntiy.Contains(tileEntity) || _tilesEntiy.Find(t => tileEntity.Id == t.Id) != null)
                return false;

            _tilesEntiy.Add(tileEntity);

            return true;
        }

        public TileEntity? GetTileEntity(int index)
        {
            return _tilesEntiy.Find(t => t.Id == index);
        }

        public TileEntity? GetTileEntityByWorldPosition(Vector2f position)
        {
            int x = (int)((position.X - Position.X) / InfoTile.TileSize);
            int y = (int)((position.Y - Position.Y) / InfoTile.TileSize);


            return GetTileEntity(x * ChunkSize + y);
        }

        public bool RemoveTileEntity(TileEntity tileEntity)
        {
            return _tilesEntiy.Remove(tileEntity);
        }

        public void UpdateTileColor(Color color, int x, int y)
        {
            if (GetTile(x, y) != null)
            {
                if(GetTile(x, y)!.IsWall)
                {
                    if (color.R >= 80)
                        color = new Color((byte)(color.R - 80), (byte)(color.R - 80), (byte)(color.R - 80));
                }
                _chunkMesh.UpdateTileColor(color, x, y);
            }
        }

        public Color GetTileColor(int x, int y)
        {
            if(y < 0)
            {
                if(GetTile(x, y) != null)
                {
                    return GetTile(x, y)!.Chunk.GetTileColor(x, ChunkSize + y);
                }
            }
            else if(y >= ChunkSize)
            {
                if (GetTile(x, y) != null)
                {
                    return GetTile(x, y)!.Chunk.GetTileColor(x, y - ChunkSize);
                }
            }
            else if(x < 0)
            {
                if (GetTile(x, y) != null)
                {
                    return GetTile(x, y)!.Chunk.GetTileColor(ChunkSize + x, y);
                }
            }
            else if(x >= ChunkSize)
            {
                if (GetTile(x, y) != null)
                {
                    return GetTile(x, y)!.Chunk.GetTileColor(x - ChunkSize, y);
                }
            }

            return _chunkMesh.GetTileColor(x, y);
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

                    upTile = upTile?.Type == TileType.Wood ? null : upTile;
                    downTile = downTile?.Type == TileType.Wood ? null : downTile;
                    leftTile = leftTile?.Type == TileType.Wood ? null : leftTile;
                    rightTile = rightTile?.Type == TileType.Wood ? null : rightTile;

                    if (tile == null || tile.Type == TileType.Wood || tile.Type == TileType.Leaves || tile.IsWall)
                        continue;

                    if ((upTile == null || upTile.IsWall) || (downTile == null || downTile.IsWall) || (leftTile == null || leftTile.IsWall) || (rightTile == null || rightTile.IsWall))
                    {
                        _colliders.Add(new AABB(new Vector2f(x, y) * InfoTile.TileSize + Position, new Vector2f(x, y) * InfoTile.TileSize + new Vector2f(InfoTile.TileSize, InfoTile.TileSize) + Position));
                    }
                }
            }
        }

        public void Update(float deltaTime)
        {
            foreach(var e in _tilesEntiy)
            {
                e.Update(deltaTime);
            }
        }

        /// <summary>
        /// Рисуем чанк
        /// </summary>
        /// <param name="target"></param>
        /// <param name="states"></param>
        public void Draw(RenderTarget target, RenderStates states)
        {
            states.Transform *= Transform;
            states.Texture = AssetManager.GetTexture("terrain");
            states.BlendMode = BlendMode.Alpha;

            target.Draw(_chunkMesh.GetChunkMesh(), PrimitiveType.Triangles, states);

            foreach(var e in _tilesEntiy)
                e.Draw(target, states);
        }

        /// <summary>
        /// Получить AABB чанка
        /// </summary>
        /// <returns> AABB </returns>
        public AABB GetAABB()
        {
            return new AABB(Position.X, Position.Y, Position.X + ChunkSize * InfoTile.TileSize, Position.Y + ChunkSize * InfoTile.TileSize);
        }

        /// <summary>
        /// Получить список колайдеров
        /// <summary>
        public List<AABB> GetColliders()
        {
            return _colliders;
        }
    }
}
