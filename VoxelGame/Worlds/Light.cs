using SFML.Graphics;
using SFML.System;
using VoxelGame.Meths;
using VoxelGame.Worlds.Tile;

namespace VoxelGame.Worlds
{
    public class Light
    {
        public const int LightRange = 7; // Радиус освещения
        public const int LightRangeWall = 7; // Радиус освещения стен
        public const int LightRangePoint = 7; // Радиус освещения точек света
        public const int LightRangePointWall = 7; // Радиус освещения точек света стен

        private static byte[,] _lightMap = new byte[Chunk.ChunkSize, Chunk.ChunkSize];
        private static byte[,] _lightMapWall = new byte[Chunk.ChunkSize, Chunk.ChunkSize];

        public static Color BaseColor { get; set; } = Color.White;
        public static void LightingChunk(Chunk chunk, List<Chunk> visibleChunk, List<Vector2f> pointsLight)
        {
            for (int x = 0; x < Chunk.ChunkSize; x++)
            {
                LightTopDown(chunk, x);
                LightDownTop(chunk, x);
            }


            for (int x = 0; x < Chunk.ChunkSize; x++)
            {
                LightLeftToRight(chunk, x);
                LightRightToLeft(chunk, x);
            }

            //FixDarkTiles(chunk);
        }

        public static void ClearLight(List<Chunk> chunks)
        {
            foreach (var chunk in chunks)
            {
                for (int x = 0; x < Chunk.ChunkSize; x++)
                {
                    for (int y = 0; y < Chunk.ChunkSize; y++)
                    {
                        if (chunk.GetTile(x, y) != null)
                        {
                            chunk.UpdateTileColor(Color.Black, x, y, true);
                        }
                        if (chunk.GetWall(x, y) != null)
                        {
                            chunk.UpdateWallColor(Color.Black, x, y, true);
                        }
                    }
                }
            }
        }

        /// <summary>  
        /// Распространяет свет от исходной плитки или стены к соседней плитке или стене.  
        /// </summary>  
        /// <param name="chunk">Чанк, содержащий плитки или стены.</param>  
        /// <param name="x">X-координата целевой плитки или стены.</param>  
        /// <param name="y">Y-координата целевой плитки или стены.</param>  
        /// <param name="fromX">X-координата исходной плитки или стены.</param>  
        /// <param name="fromY">Y-координата исходной плитки или стены.</param>  
        /// <param name="wall">Указывает, распространяется ли свет для стен (<c>true</c>) или плиток (<c>false</c>).</param>  
        /// <remarks>  
        /// Этот метод вычисляет интенсивность света на основе исходной плитки или стены и обновляет целевую плитку или стену.  
        /// Если обе соседние плитки или стены пусты, применяется базовый цвет света.  
        /// </remarks>  
        private static void PropagateLight(Chunk chunk, int x, int y, int fromX, int fromY, bool wall)
        {
            // Получаем целевую плитку или стену
            ITile? tile = chunk.GetTile(x, y, getOtherChunk: true);

            // Получаем исходную плитку
            ITile? tileFrom = chunk.GetTile(fromX, fromY, getOtherChunk: true);

            // Получаем противоположную стену (если есть)
            ITile? tileFromOpposite = chunk.GetWall(fromX, fromY, getOtherChunk: true);

            // Если распространяем свет для стен, обновляем ссылки на стены
            if (wall)
            {
                tile = chunk.GetWall(x, y, getOtherChunk: true);
                tileFrom = chunk.GetWall(fromX, fromY, getOtherChunk: true);
                tileFromOpposite = chunk.GetTile(fromX, fromY, getOtherChunk: true);
            } 

            // Если целевая плитка или стена отсутствует, выходим из метода
            if (tile == null)
                return;

            // Если соседние плитки или стены пусты, применяем базовый цвет света
            if (tileFrom == null && tileFromOpposite == null)
            {
                if (!wall)
                {
                    chunk.UpdateTileColor(BaseColor, x, y, useOtherChunk: true);
                }
                else
                {
                    chunk.UpdateWallColor(BaseColor, x, y, useOtherChunk: true);
                }
                return;
            }

            // Вычисляем интенсивность света от соседей
            int light = 0;

            // Если есть исходная плитка, получаем её цвет
            if (tileFrom != null)
            {
                if (!wall)
                {
                    light = chunk.GetTileColor(fromX, fromY, useOtherChunk: true).R;
                }
                else
                {
                    light = chunk.GetWallColor(fromX, fromY, useOtherChunk: true).R;
                }
            }
            // Если есть противоположная стена, получаем её цвет
            else if (tileFromOpposite != null)
            {
                if (!wall)
                {
                    light = chunk.GetTileColor(fromX, fromY, useOtherChunk: true).R;
                }
                else
                {
                    light = chunk.GetWallColor(fromX, fromY, useOtherChunk: true).R;
                }
            }

            // Уменьшаем интенсивность света в зависимости от радиуса освещения
            if (wall)
                light -= byte.MaxValue / LightRange;
            else
                light -= byte.MaxValue / LightRangeWall;

            // Ограничиваем значение света в диапазоне от 0 до 255
            light = Math.Clamp(light, 0, 255);

            // Если распространяем свет для плиток
            if (!wall)
            {
                int currentLight = chunk.GetTileColor(x, y, useOtherChunk: true).R;

                // Обновляем цвет плитки, если новая интенсивность света больше текущей
                if (light > currentLight)
                    chunk.UpdateTileColor(new Color((byte)light, (byte)light, (byte)light), x, y, useOtherChunk: true);
            }
            // Если распространяем свет для стен
            else
            {
                int currentLight = chunk.GetWallColor(x, y, useOtherChunk: true).R;

                // Обновляем цвет стены, если новая интенсивность света больше текущей
                if (light > currentLight)
                    chunk.UpdateWallColor(new Color((byte)light, (byte)light, (byte)light), x, y, useOtherChunk: true);
            }
        }

        public static void LightTopDown(Chunk chunk, int x)
        {
            for (int y = 0; y < Chunk.ChunkSize; y++)
            {
                PropagateLight(chunk, x, y, x, y - 1, false);
                PropagateLight(chunk, x, y, x, y - 1, true);
            }
        }

        public static void LightDownTop(Chunk chunk, int x)
        {
            for (int y = Chunk.ChunkSize; y >= 0; y--)
            {
                PropagateLight(chunk, x, y, x, y + 1, false);
                PropagateLight(chunk, x, y, x, y + 1, true);
            }
        }

        public static void LightLeftToRight(Chunk chunk, int y)
        {
            for (int x = 0; x < Chunk.ChunkSize; x++)
            {
                PropagateLight(chunk, x, y, x - 1, y, false);
                PropagateLight(chunk, x, y, x - 1, y, true);
            }
        }

        public static void LightRightToLeft(Chunk chunk, int y)
        {
            for (int x = Chunk.ChunkSize; x >= 0; x--)
            {
                PropagateLight(chunk, x, y, x + 1, y, false);
                PropagateLight(chunk, x, y, x + 1, y, true);
            }
        }

        /// <summary>  
        /// Updates the lighting for the current chunk and visible chunks based on light sources.  
        /// </summary>  
        /// <param name="currentChunk">The chunk currently being processed.</param>  
        /// <param name="visibleChunks">A list of chunks visible to the player.</param>  
        public static void LightPoint(Chunk currentChunk, List<Chunk> visibleChunks)
        {
            var lightSources = currentChunk.GetTilesByType(TileType.Torch).Select(s => s.GlobalPosition).ToList();

            LightPoint(currentChunk, visibleChunks, lightSources);
        }

        public static void LightPoint(Chunk currentChunk, List<Chunk> visibleChunks, List<Vector2f> lightSources)
        {
            foreach (var chunk in visibleChunks)
            {
                foreach (var light in lightSources)
                {
                    Vector2f lightGlobalPos = light;

                    if(MathHelper.Distance(lightGlobalPos, chunk.Position + new Vector2f(Chunk.ChunkSize / 2, Chunk.ChunkSize / 2)) > LightRangePoint * 3 * Tile.Tile.TileSize)
                        continue;

                    DebugRender.AddRectangle(chunk.GetAABB(), Color.Red);

                    for (int x = 0; x < Chunk.ChunkSize; x++)
                    {
                        for (int y = 0; y < Chunk.ChunkSize; y++)
                        {
                            var tile = chunk.GetTile(x, y, true);
                            var tileWall = chunk.GetWall(x, y, true);

                            if (tile == null && tileWall == null)
                                continue;

                            if (tile != null)
                            {
                                Vector2f tileGlobalPos = tile.GlobalPosition;
                                float distance = MathHelper.Distance(lightGlobalPos, tileGlobalPos);

                                if (distance > LightRangePoint * Tile.Tile.TileSize)
                                    continue;

                                byte previousColor = chunk.GetTileColor(x, y, true).R;

                                int newLight = byte.MaxValue - (int)(distance / Tile.Tile.TileSize) * (byte.MaxValue / LightRangePoint);

                                if (newLight < 0)
                                    continue;

                                newLight = Math.Clamp(newLight, 0, 255);

                                if (newLight > previousColor)
                                {
                                    chunk.LightColorTile[x, y] = (byte)newLight;
                                    chunk.UpdateTileColor(new Color((byte)newLight, (byte)newLight, (byte)newLight), x, y, true);
                                }
                            }

                            if (tileWall != null)
                            {
                                Vector2f tileGlobalPos = tileWall.GlobalPosition;
                                float distance = MathHelper.Distance(lightGlobalPos, tileGlobalPos);

                                if (distance > LightRangePoint * Tile.Tile.TileSize)
                                    continue;

                                byte previousColorWall = chunk.GetWallColor(x, y, true).R;

                                int newLight = byte.MaxValue - (int)(distance / Tile.Tile.TileSize) * (byte.MaxValue / LightRangePointWall);

                                if (newLight < 0)
                                    continue;

                                newLight = Math.Clamp(newLight, 0, 255);

                                if (newLight > previousColorWall)
                                {
                                    chunk.LightColorWall[x, y] = (byte)newLight;
                                    chunk.UpdateWallColor(new Color((byte)newLight, (byte)newLight, (byte)newLight), x, y, true);
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Calculates the global light intensity based on the given time parameter.
        /// </summary>
        /// <param name="time">A value representing the time, where 0.0 corresponds to night, 0.5 corresponds to day, and 1.0 corresponds to night again.</param>
        /// <returns>A <see cref="Color"/> representing the calculated global light intensity.</returns>
        public static Color CalculateGlobalLight(float time)
        {
            // Симметричная синусоида: 0.0 → ночь, 0.5 → день, 1.0 → ночь
            float intensity = 0.05f + 0.95f * MathF.Sin(time * MathF.PI); // 0.2..1.0

            byte light = (byte)(intensity * 255);
            BaseColor = new Color(light, light, light);

            return BaseColor;
        }
    }
}


