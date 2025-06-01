using SFML.Graphics;
using SFML.System;
using SFML.Window;
using VoxelGame.Entitys;
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
        public int ChunkCountY;

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

        private EntityManager entityManager;

        private List<Vector2f> _lightsPosition;

        private Player _player;

        public static bool BaseDrawDebug = false;
        public static bool ChunkBorderDrawDebug = false;
        public static bool EntitiesDrawDebug = false;
        public static bool CollideDrawDebug = false;

        /// <summary>
        /// Средння высота мира
        /// </summary>
        public float BaseHeight { get; set; } = 300;

        public static Random Random { get; set; }

        public RectangleShape BackgroundRect { get; set; }

        /// <summary>
        /// Мир
        /// </summary>
        /// <param name="width"> Ширина в плитках </param>
        /// <param name="height"> Высота в плитках </param>
        /// <param name="seed"> Сид мира </param>
        public World(int width, int height, int seed)
        {
            ChunkCountX = width / Chunk.ChunkSize;
            ChunkCountY = height / Chunk.ChunkSize;
            _seed = seed;

            _physicsWorld = new PhysicsWorld(new Vector2f(0, 200));

            _chanks = new Chunk[ChunkCountX * ChunkCountY];
            drawbleChunk = new List<Chunk>();
            _lightsPosition = new List<Vector2f>();

            entityManager = new EntityManager(this);

            var maxFullScreenModeSize = new Vector2f(VideoMode.FullscreenModes[0].Width, VideoMode.FullscreenModes[0].Height) + new Vector2f(100,100);

            BackgroundRect = new RectangleShape(maxFullScreenModeSize);
            BackgroundRect.Texture = TextureManager.GetTexture("Background_0");
            BackgroundRect.Origin = BackgroundRect.Size / 2;
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

            if (x < 0 || x >= ChunkCountX || y < 0 || y >= ChunkCountY)
                return;

            int index = (x * ChunkCountX) + y;

            if (index >= _chanks.Length)
                return;

            chunk.GenerateColliders();
            chunk.IsComplate = true;
            _chanks[index] = chunk;
        }

        public void GenColliders()
        {
            Game.WorldLoaded = true;

            int startX = ChunkCountX * Chunk.ChunkSize * Tile.Tile.TileSize / 2;
            int startY = (int)BaseHeight * Tile.Tile.TileSize;

            var chunk = GetChunkByWorldPosition(new Vector2f(startX, startY));
            if (chunk != null)
            {
                for (int i = 0; i < BaseHeight; i++)
                {
                    var tile = chunk.GetTileByWorldPosition(new Vector2f(startX, startY - i), true);

                    if (tile != null && tile.Type == Tile.TileType.Grass)
                    {
                        startY -= i;
                        break;
                    }
                }
            }

            if(startY == (int)BaseHeight * Tile.Tile.TileSize)
            {
                chunk = GetChunkByWorldPosition(new Vector2f(startX, startY));
                if (chunk != null)
                {
                    for (int x = -100; x < 100; x++)
                    {
                        for (int y = 0; y < BaseHeight; y++)
                        {
                            var tile = chunk.GetTileByWorldPosition(new Vector2f(startX + x, startY - y), true);

                            if (tile != null && tile.Type == Tile.TileType.Grass)
                            {
                                startY -= y;
                                startX += x;
                                break;
                            }
                        }
                    }
                }
            }


            for (int x = 0; x < ChunkCountX; x++)
            {
                for (int y = 0; y < ChunkCountY; y++)
                {
                    int index = (x * ChunkCountX) + y;

                    if (index >= _chanks.Length)
                        continue;

                    if (_chanks[index] != null && _chanks[index].IsComplate)
                    {
                        _chanks[index].GenerateColliders();
                    }
                }
            }

            _player = new Player(this, new AABB(28, 46)) { Position = new Vector2f(startX, startY - 6 * 16), StartPosition = Position };
            entityManager.AddEntity(_player);
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

            if (x < 0 || x >= ChunkCountX || y < 0 || y >= ChunkCountY)
                return null;

            int index = (x * ChunkCountX) + y;

            if (index >= _chanks.Length)
                return null;

            return _chanks[index];
        }

        /// <summary>
        /// Получить чанк по глобальным координатам
        /// </summary>
        /// <param name="position"> Глобальная позиция чанка </param>
        /// <returns> Чанк, null если координаты вышли за границы локальных координат или чанк был null </returns>
        public Chunk? GetChunkByWorldPosition(Vector2f position)
        {
            int x = (int)(position.X / Chunk.ChunkSize / Tile.Tile.TileSize);
            int y = (int)(position.Y / Chunk.ChunkSize / Tile.Tile.TileSize);
            return GetChunk(x, y);
        }

        object lockes = new object();
        float timeOfDay = 0.5f; // 0.0 — полночь, 0.5 — полдень, 1.0 — снова полночь
        float timeSpeed = 0.0001f; // скорость времени

        /// <summary>
        /// Обновление мира
        /// </summary>
        /// <param name="deltaTime"> Время кадра </param>
        public void Update(float deltaTime)
        {
            if (!Game.WorldLoaded)
                return;

            timeOfDay += timeSpeed * deltaTime;
            if (timeOfDay > 1f) timeOfDay -= 1f;

            if (timeOfDay > 0.15f && timeOfDay < 0.75f)
            {
                AudioManager.PlaySuond("Overworld-Day");
                AudioManager.SetVolumeSound("Overworld-Night_1", 50);

                if(timeOfDay > 0.18f)
                    AudioManager.StopSound("Overworld-Night_1");
            }
            else
            {
                AudioManager.SetVolumeSound("Overworld-Day", 50);

                if (timeOfDay < 0.138f || timeOfDay > 0.73f)
                {
                    AudioManager.StopSound("Overworld-Day");
                }

                AudioManager.PlaySuond("Overworld-Night_1");
            }

            BackgroundRect.Position = _player.Position;
            BackgroundRect.FillColor = Light.BaseColor;

            drawbleChunk = new List<Chunk>();

            {
                var chunkPos = (_player.Position.X / (Chunk.ChunkSize * Tile.Tile.TileSize), _player.Position.Y / (Chunk.ChunkSize * Tile.Tile.TileSize));
                var chunksPerScreen = (MathF.Ceiling(Game.GetWindowSizeWithZoom().X / (Chunk.ChunkSize * Tile.Tile.TileSize)), MathF.Ceiling(Game.GetWindowSizeWithZoom().Y / (Chunk.ChunkSize * Tile.Tile.TileSize)));
                var LeftMostTilesPos = (int)(chunkPos.Item1 - chunksPerScreen.Item1 / 2);
                var TopMostTilesPos = (int)(chunkPos.Item2 - chunksPerScreen.Item2 / 2);

                for (int x = LeftMostTilesPos; x < LeftMostTilesPos + chunksPerScreen.Item1 + 1; x++)
                {
                    for (int y = TopMostTilesPos; y < TopMostTilesPos + chunksPerScreen.Item2 + 1; y++)
                    {
                        if (x >= 0 && x < ChunkCountX && y >= 0 && y < ChunkCountY)
                        {
                            var chunk = GetChunk(x, y);
                            if (chunk != null)
                            {
                                chunk.Update(deltaTime);
                                drawbleChunk.Add(chunk);

                                if (ChunkBorderDrawDebug)
                                {
                                    DebugMenu.DrawChunkBorders(chunk);
                                }
                            }
                        }
                    }
                }
            }

            entityManager.Update(deltaTime, drawbleChunk);

            Light.ClearLight(drawbleChunk);

            Light.CalculateGlobalLight(timeOfDay);

            foreach (var chunk in drawbleChunk)
            {
                Light.LightingChunk(chunk, drawbleChunk, new List<Vector2f>());
                Light.LightPoint(chunk, drawbleChunk);
                Light.LightPoint(chunk, drawbleChunk, _lightsPosition);
            }

            _lightsPosition.Clear();

            //Task.Run(() =>
            //{
            try
            {
                lock (lockes)
                {
                    _physicsWorld.Step(deltaTime, entityManager.GetEntities(), drawbleChunk);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка в физическом мире: " + ex.Message);
            }
            //});

            Game.SetCameraPosition(_player.Position);

            if (BaseDrawDebug)
            {
                DebugMenu.DrawBaseInfo(drawbleChunk.Count, entityManager.GetEntities().Count  , deltaTime, timeOfDay, _player);
            }
            if (EntitiesDrawDebug)
            {
                DebugMenu.DrawEntityCollider(entityManager.GetEntities());
            }
        }

        /// <summary>
        /// Рисование мира
        /// </summary>
        /// <param name="target"></param>
        /// <param name="states"></param>
        public void Draw(RenderTarget target, RenderStates states)
        {
            if (!Game.WorldLoaded)
                return;

            states.Transform *= Transform;

            target.Draw(BackgroundRect, states);

            for (int i = 0; i < drawbleChunk.Count; i++)
            {
                if (drawbleChunk[i] != null)
                {
                    drawbleChunk[i].DrawWall(target, states);
                }
            }

            for (int i = 0; i < drawbleChunk.Count; i++)
            {
                if (drawbleChunk[i] != null)
                {
                    drawbleChunk[i].DrawTile(target, states);
                }
            }

            entityManager.Draw(target, states);

        }

        /// <summary>
        /// Получить игрока
        /// </summary>
        /// <returns></returns>
        public Player? GetPlayer()
        {
            return _player;
        }

        /// <summary>
        /// Выбросить предмет
        /// </summary>
        /// <param name="infoItem"> Предмет </param>
        /// <param name="position"> Позиция </param>
        public void DropItem(Item.Item infoItem, Vector2f position)
        {
            entityManager.AddEntity(new DropItem(infoItem, new AABB(16, 16), this) { Position = position });
        }

        /// <summary>
        /// Выбросить предмет, на основе данных плитки будет определен предмет для выбрасывания
        /// </summary>
        /// <param name="tile"> Плитка </param>
        /// <param name="position"> Позиция </param>
        public void DropItem(Tile.Tile tile, Vector2f position)
        {
            if (Enum.GetNames<ItemList>().Contains(tile.Type.ToString()))
            {
                DropItem(Items.GetItem(tile.DropItem), position);
            }
            else
            {
                DropItem(Items.GetItem(ItemList.None), position);
            }
        }

        public void DropItem(ItemList item, Vector2f position)
        {
            if (item != ItemList.None)
                DropItem(Items.GetItem(item), position);
        }

        /// <summary>
        /// Получить выброшеный предмет
        /// </summary>
        /// <param name="dropItem"></param>
        /// <returns></returns>
        public DropItem? GetItem(DropItem dropItem)
        {
            return entityManager.GetEntitisOfType<DropItem>().FirstOrDefault(item => item != null && item.Item == dropItem.Item);
        }

        /// <summary>
        /// Удалить выброшеный предмет
        /// </summary>
        /// <param name="dropItem"></param>
        /// <returns></returns>
        public bool RemoveItem(DropItem dropItem)
        {
            if (entityManager.GetEntities().Contains(dropItem))
            {
                var index = entityManager.GetEntities().IndexOf(dropItem);

                if (index < 0 || index >= entityManager.EntityCount)
                    return false;

                return entityManager.RemoveEntity(dropItem);
            }

            return false;
        }

        public void AddEntity(Entity entity)
        {
            if (entity != null)
            {
                entityManager.AddEntity(entity);
            }
        }

        public void RemoveEntity(Entity entity)
        {
            if (entity != null)
            {
                entityManager.RemoveEntity(entity);
            }
        }

        public void RemoveEntity(int index)
        {
            if (index >= 0 && index < entityManager.EntityCount)
            {
                entityManager.RemoveAtEntity(index);
            }
        }

        public Entity GetEntity(int index)
        {
            return entityManager.GetEntity(index);
        }

        public void AddLight(Vector2f position)
        {
            _lightsPosition.Add(position);
        }
    }
}
