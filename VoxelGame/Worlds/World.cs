using SFML.Graphics;
using SFML.System;
using VoxelGame.Item;
using VoxelGame.Npc;
using VoxelGame.Physics;
using VoxelGame.Physics.Collision;
using VoxelGame.Physics.Collision.Colliders;
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
        /// Список тел которые будут обрабатываться в физическом мире
        /// </summary>
        private List<RigidBody> _bodies;

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

            _physicsWorld = new PhysicsWorld();

            _chanks = new Chunk[ChunkCountX * ChumkCountY];
            drawbleChunk = new List<Chunk>();
            _bodies = new List<RigidBody>();
            _entities = new List<Entity>();
            _dropItems = new List<DropItem>();

            var body = new RigidBody(Polygon.BoxPolygon(32, 64), 3, 0);

            _entities.Add(new Player(body, this));
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

        /// <summary>
        /// Обновление мира
        /// </summary>
        /// <param name="deltaTime"> Время кадра </param>
        public void Update(float deltaTime)
        {
            _dropItems.ForEach(item => item.Update(deltaTime));

            _bodies.Clear();

            _entities.ForEach(entity => { entity.Update(deltaTime); _bodies.Add(entity.GetBody()); });
            _dropItems.ForEach(item => { item.Update(deltaTime); _bodies.Add(item.GetBody()); });

            Task<List<Chunk>> chunks = Task.Run(() =>
            {
                var drawbleChunk = new List<Chunk>();
                var tilePos = (Game.GetCameraPosition().X / Chunk.ChunkSize / InfoTile.TileSize, Game.GetCameraPosition().Y / Chunk.ChunkSize / InfoTile.TileSize);
                var tilesPerScreen = (MathF.Ceiling(Game.GetWindowSize().X / (Chunk.ChunkSize * InfoTile.TileSize)), MathF.Ceiling(Game.GetWindowSize().Y / (Chunk.ChunkSize * InfoTile.TileSize)));
                var LeftMostTilesPos = (int)(tilePos.Item1 - tilesPerScreen.Item1);
                var TopMostTilesPos = (int)(tilePos.Item2 - tilesPerScreen.Item2);


                for (int x = LeftMostTilesPos; x < LeftMostTilesPos + tilesPerScreen.Item1 + 2; x++)
                {
                    for (int y = TopMostTilesPos; y < TopMostTilesPos + tilesPerScreen.Item2 + 2; y++)
                    {
                        if (x >= 0 && x < ChunkCountX / Chunk.ChunkSize && y >= 0 && y < ChumkCountY / Chunk.ChunkSize)
                        {
                            var chunk = GetChunk(x, y);
                            if (chunk != null)
                            {
                                drawbleChunk.Add(chunk);

                                foreach (var entity in _entities)
                                {
                                    if (CollisionDetected.AABBsIntersect(chunk.GetAABB(), entity.GetBody().GetAABB()))
                                    {
                                        foreach (var body in chunk.GetBodies())
                                            if (CollisionDetected.AABBsIntersect(entity.GetBody().GetAABB(), body.GetAABB()))
                                            {
                                                _bodies.Add(body);
                                                break;
                                            }
                                    }
                                }
                                foreach (var entity in _dropItems)
                                {
                                    if (CollisionDetected.AABBsIntersect(chunk.GetAABB(), entity.GetBody().GetAABB()))
                                    {
                                        foreach (var body in chunk.GetBodies())
                                            if (CollisionDetected.AABBsIntersect(entity.GetBody().GetAABB(), body.GetAABB()))
                                            {
                                                _bodies.Add(body);
                                                break;
                                            }
                                    }
                                }
                            }
                        }
                    }
                }

                return drawbleChunk;
            });
            drawbleChunk = chunks.Result;
            _physicsWorld.Step(deltaTime, 20, _bodies);
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

            for (int i = 0; i < _bodies.Count; i++)
            {
                if (_bodies[i] != null)
                {
                    DebugRender.AddRectangle(_bodies[i].GetPolygon());
                }
            }

            _entities.ForEach(entity => entity.Draw(target, states));
            _dropItems.ForEach(item => item.Draw(target, states));
        }

        /// <summary>
        /// Выбросить предмет
        /// </summary>
        /// <param name="infoItem"> Предмет </param>
        /// <param name="position"> Позиция </param>
        public void DropItem(InfoItem infoItem, Vector2f position)
        {
            _dropItems.Add(new DropItem(infoItem, _physicsWorld.CreateBody(Polygon.BoxPolygon(16, 16), 1, 0)) { Position = position });
        }

        /// <summary>
        /// Выбросить предмет, на основе данных плитки будет определен предмет для выбрасывания
        /// </summary>
        /// <param name="tile"> Плитка </param>
        /// <param name="position"> Позиция </param>
        public void DropItem(InfoTile tile, Vector2f position)
        {
            var dropItem = new DropItem(ItemTileByTile.GetInfoItemByTile(tile), _physicsWorld.CreateBody(Polygon.BoxPolygon(16, 16), 1, 0)) { Position = position};
            _dropItems.Add(dropItem);
        }
    }
}
