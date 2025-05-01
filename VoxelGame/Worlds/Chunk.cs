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
        public const int ChunkSize = 48;

        /// <summary>
        /// Мир (родитель)
        /// </summary>
        private World _world;

        /// <summary>
        /// Массив плиток
        /// </summary>
        private InfoTile[] _tiles;

        /// <summary>
        /// Список тел
        /// </summary>
        private List<AABB> _colliders;

        /// <summary>
        /// Сетка чанка
        /// </summary>
        private ChunkMesh _chunkMesh { get; }

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
        public bool SetTile(int x, int y, InfoTile? tile)
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
            }
            else if (_tiles[index] != null && IsComplate)
            {
                _world.DropItem(_tiles[index],
                    new Vector2f(x, y) * InfoTile.TileSize + Position + new Vector2f(InfoTile.TileSize, InfoTile.TileSize) / 2);
            }

            if (!IsComplate)
            {
                // Устанавливаем плитку в массив
                _tiles[index] = tile!;

                // Устанавливаем плитку в сетку чанка
                _chunkMesh.SetTileToMesh(x, y, tile!);
            }
            else
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
                    _chunkMesh.SetTileToMesh(x, y, tile!);
                }
                else
                    return false;
            }

            if(IsComplate)
            {
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
        public bool SetTile(int x, int y, TileType type)
        {
            return SetTile(x, y, Tiles.GetTile(type));
        }

        /// <summary>
        /// Установить плитку в локальных координатах, по мировым координатам
        /// </summary>
        /// <param name="position"> Позиция </param>
        /// <param name="tile"> Плитка </param>
        /// <returns> True если плитка установленна </returns>
        public bool SetTileByWorldPosition(Vector2f position, InfoTile? tile)
        {
            int x = (int)((position.X - Position.X) / InfoTile.TileSize);
            int y = (int)((position.Y - Position.Y) / InfoTile.TileSize);

            if (x < 0 || x >= ChunkSize || y < 0 || y >= ChunkSize)
                return false;

            return SetTile(x, y, tile);
        }

        /// <summary>
        /// Установить плитку в локальных координатах, по мировым координатам, по типу плитки
        /// </summary>
        /// <param name="position"> Позиция </param>
        /// <param name="tile"> Плитка </param>
        /// <returns> True если плитка установленна </returns>
        public bool SetTileByWorldPosition(Vector2f position, TileType type)
        {
            int x = (int)((position.X - Position.X) / InfoTile.TileSize);
            int y = (int)((position.Y - Position.Y) / InfoTile.TileSize);

            return SetTile(x, y, type);
        }

        /// <summary>
        /// Получить плитку по локальным координатам
        /// </summary>
        /// <param name="x"> Позиция по Х </param>
        /// <param name="y"> Позиция по У </param>
        /// <returns> Плитку, null если х,у вышли за рамки чанка или если плитка null </returns>
        public InfoTile? GetTile(int x, int y)
        {
            if(x < 0 || x >= ChunkSize || y < 0 || y >= ChunkSize)
                return null;

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

                    if (tile == null || tile.Type == TileType.Wood || tile.Type == TileType.Leaves)
                        continue;

                    if (upTile == null || downTile == null || leftTile == null || rightTile == null)
                    {
                        _colliders.Add(new AABB(new Vector2f(x, y) * InfoTile.TileSize + Position, new Vector2f(x, y) * InfoTile.TileSize + new Vector2f(InfoTile.TileSize, InfoTile.TileSize) + Position));
                    }
                }
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

            target.Draw(_chunkMesh.GetChunkMesh(), PrimitiveType.Triangles, states);
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
