using SFML.Graphics;
using SFML.System;
using VoxelGame.Item;
using VoxelGame.Physics;
using VoxelGame.Resources;
using VoxelGame.Worlds.Tile;

namespace VoxelGame.Worlds
{
    public class World : Transformable, Drawable
    {
        /// <summary>
        /// Количество чанков по ширине
        /// </summary>
        public int ChunkCountX;

        /// <summary>
        /// Количество чанков по высоте
        /// </summary>
        public int ChumkCountY;

        /// <summary>
        /// Сид мира
        /// </summary>
        private int _seed;

        /// <summary>
        /// Физический мир
        /// </summary>
        private PhysicsWorld _physicsWorld;

        /// <summary>
        /// Масив чанков
        /// </summary>
        private Chunk[] _chanks;

        /// <summary>
        /// Список чанков которые будут рисоваться
        /// </summary>
        private List<Chunk> drawbleChunk;

        /// <summary>
        /// Список сущностей
        /// </summary>
        private List<Entity> _entities;

        /// <summary>
        /// Список предметов лежащих в мире
        /// </summary>
        private List<DropItem> _dropItems;

        /// <summary>
        /// Средння высота мира
        /// </summary>
        public float BaseHeight { get; set; } = 50;

        /// <summary>
        /// Мир
        /// </summary>
        /// <param name="width"> Ширина в плитках </param>
        /// <param name="height"> Высота в плитках </param>
        /// <param name="seed"> Сид мира </param>
        public World(int width, int height, int seed)
        {
            ChunkCountX = width / Chunk.ChunkSize;
            ChumkCountY = height / Chunk.ChunkSize;
            _seed = seed;

            _physicsWorld = new PhysicsWorld(new Vector2f(0, 200));

            _chanks = new Chunk[ChunkCountX * ChumkCountY];
            drawbleChunk = new List<Chunk>();
            _entities = new List<Entity>();
            _dropItems = new List<DropItem>();

            _entities.Add(new Player(this, new AABB(32, 64)));
        }

        /// <summary>
        /// Ставим чанк
        /// </summary>
        /// <param name="x"> Позиция чанка по Х в локальных координатах </param>
        /// <param name="y"> Позиция чанка по У в локальных координатах </param>
        /// <param name="chunk"> Чанк </param>
        public void SetChunk(int x, int y, Chunk chunk) 
        {
            if (_chanks == null)
                return;

            if (x < 0 || x >= ChunkCountX || y < 0 || y >= ChumkCountY)
                return;

            int index = (x * ChunkCountX) + y;

            if (index >= _chanks.Length)
                return;

            chunk.GenerateColliders();
            chunk.IsComplate = true;
            _chanks[index] = chunk;
        }

        /// <summary>
        /// Получить чанк по локальным координатам
        /// </summary>
        /// <param name="x"> Позиция по Х </param>
        /// <param name="y"> Позиция по У </param>
        /// <returns> Чанк, null если координаты вышли за границы локальных координат или чанк был null </returns>
        public Chunk? GetChunk(int x, int y)
        {
            if (_chanks == null)
                return null;

            if (x < 0 || x >= ChunkCountX || y < 0 || y >= ChumkCountY)
                return null;

            int index = (x * ChunkCountX) + y;

            if (index >= _chanks.Length)
                return null;

            return _chanks[index];
        }

        /// <summary>
        /// Получить чанк по глобальным координатам
        /// </summary>
        /// <param name="x"> Позиция по Х </param>
        /// <param name="y"> Позиция по У </param>
        /// <returns> Чанк, null если координаты вышли за границы локальных координат или чанк был null </returns>
        public Chunk? GetChunkByWorldPosition(Vector2f position)
        {
            int x = (int)(position.X / Chunk.ChunkSize / InfoTile.TileSize);
            int y = (int)(position.Y / Chunk.ChunkSize / InfoTile.TileSize);
            return GetChunk(x, y);
        }

        object lockes = new object();

        /// <summary>
        /// Обновление мира
        /// </summary>
        /// <param name="deltaTime"> Время кадра </param>
        public void Update(float deltaTime)
        {
            Task<List<Chunk>> chunks = Task.Run(() =>
            {
                var drawbleChunk = new List<Chunk>();
                var tilePos = (Game.GetCameraPosition().X / Chunk.ChunkSize / InfoTile.TileSize, Game.GetCameraPosition().Y / Chunk.ChunkSize / InfoTile.TileSize);
                var tilesPerScreen = (MathF.Ceiling(Game.GetWindowSize().X / (Chunk.ChunkSize * InfoTile.TileSize)), MathF.Ceiling(Game.GetWindowSize().Y / (Chunk.ChunkSize * InfoTile.TileSize)));
                var LeftMostTilesPos = (int)(tilePos.Item1 - tilesPerScreen.Item1);
                var TopMostTilesPos = (int)(tilePos.Item2 - tilesPerScreen.Item2);


                for (int x = LeftMostTilesPos - 1; x < LeftMostTilesPos + tilesPerScreen.Item1 + 2; x++)
                {
                    for (int y = TopMostTilesPos - 1; y < TopMostTilesPos + tilesPerScreen.Item2 + 2; y++)
                    {
                        if (x >= 0 && x < ChunkCountX && y >= 0 && y < ChumkCountY)
                        {
                            var chunk = GetChunk(x, y);
                            if (chunk != null)
                            {
                                drawbleChunk.Add(chunk);
                            }
                        }
                    }
                }
                

                 return drawbleChunk;
            });
            drawbleChunk = chunks.Result;

            Task.Run(() =>
            {
                try
                {
                    lock (lockes)
                    {
                        _entities.ForEach(entity =>
                        {
                            entity.Update(deltaTime);
                        });

                        _physicsWorld.Step(deltaTime, _entities, drawbleChunk);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ошибка в физическом мире: " + ex.Message);
                }
            });

            Game.SetCameraPosition(_entities[0].Position);
        }

        /// <summary>
        /// Рисование мира
        /// </summary>
        /// <param name="target"></param>
        /// <param name="states"></param>
        public void Draw(RenderTarget target, RenderStates states)
        {
            states.Transform *= Transform;

            for (int i = 0; i < drawbleChunk.Count; i++)
            {
                if (drawbleChunk[i] != null)
                {
                    drawbleChunk[i].Draw(target, states);
                }
            }

            lock (lockes)
            {
                _entities.ForEach(entity => { entity.Draw(target, states); });
            }
        }

        public Player? GetPlayer()
        {
            return _entities.OfType<Player>().FirstOrDefault();
        }

        /// <summary>
        /// Выбросить предмет
        /// </summary>
        /// <param name="infoItem"> Предмет </param>
        /// <param name="position"> Позиция </param>
        public void DropItem(Item.Item infoItem, Vector2f position)
        {
            _entities.Add(new DropItem(infoItem, new AABB(16, 16), this) { Position = position });
        }

        /// <summary>
        /// Выбросить предмет, на основе данных плитки будет определен предмет для выбрасывания
        /// </summary>
        /// <param name="tile"> Плитка </param>
        /// <param name="position"> Позиция </param>
        public void DropItem(InfoTile tile, Vector2f position)
        {
            if (Enum.GetNames<ItemList>().Contains(tile.Type.ToString()))
            {
                DropItem(Items.GetItem((ItemList)Enum.Parse(typeof(ItemList), tile.Type.ToString())), position);
            }
            else
            {
                DropItem(Items.GetItem(ItemList.None), position);
            }
        }

        public DropItem? GetItem(DropItem dropItem)
        {
            return _entities.OfType<DropItem>().FirstOrDefault(item => item != null && item.Item == dropItem.Item);
        }

        public bool RemoveItem(DropItem dropItem)
        {
            if (_entities.Contains(dropItem))
            {
                var index = _entities.IndexOf(dropItem);

                if (index < 0 || index >= _entities.Count)
                    return false;

                return _entities.Remove(dropItem);
            }

            return false;
        }
    }
}
