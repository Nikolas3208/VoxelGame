using SFML.Graphics;
using SFML.System;
using VoxelGame.Item;
using VoxelGame.Physics;
using VoxelGame.Physics.Collision.Colliders;
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
        private List<RigidBody> _bodies;

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

            _bodies = new List<RigidBody>(ChunkSize * ChunkSize);
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
            tile!.Id = index;

            // Устанавливаем плитку в массив
            _tiles[index] = tile;

            // Устанавливаем плитку в сетку чанка
            _chunkMesh.SetTileToMesh(x, y, tile);

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
            return SetTile(x, y, TileByTypes.GetTileByType(type));
        }

        /// <summary>
        /// Установить плитку в локальных координатах, по мировым координатам
        /// </summary>
        /// <param name="position"> Позиция </param>
        /// <param name="tile"> Плитка </param>
        /// <returns> True если плитка установленна </returns>
        public bool SetTileByWorldPosition(Vector2f position, InfoTile tile)
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
            if (x < 0 || x >= ChunkSize || y < 0 || y >= ChunkSize)
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

        /// <summary>
        /// Создать коллайдеры для чанка
        /// </summary>
        public void GenerateColliders()
        {
            Vector2f start = new Vector2f(-1, -1);

            for (int x = 0; x < ChunkSize; x++)
            {
                for (int y = 0; y < ChunkSize; y++)
                {
                    var tile = GetTile(x, y);
                    var below = GetTile(x, y + 1);

                    // Игнорируем не твёрдые плитки
                    if (tile == null || tile.Type == TileType.Wood || tile.Type == TileType.Leaves)
                        continue;

                    // Если снизу пусто — значит верхняя грань открыта, создаём коллайдер
                    if (below == null)
                    {
                        Vector2f p0 = new Vector2f(x, y) * InfoTile.TileSize;
                        Vector2f p1 = new Vector2f(x, y + 1) * InfoTile.TileSize;
                        Vector2f p2 = new Vector2f(x + 1, y + 1) * InfoTile.TileSize;
                        Vector2f p3 = new Vector2f(x + 1, y) * InfoTile.TileSize;

                        var polygon = new Polygon(new List<Vector2f> { p0, p1, p2, p3 });

                        var body = new RigidBody(polygon, 1, 0, BodyType.Static)
                        {
                            Position = Position,
                            Layer = CollisionLayer.Ground,
                            CollidesWith = CollisionLayer.All
                        };

                        _bodies.Add(body);
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
        /// Получить список тел
        /// <summary>
        public List<RigidBody> GetBodies()
        {
            return _bodies;
        }
    }
}
