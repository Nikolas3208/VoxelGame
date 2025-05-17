using SFML.Graphics;
using SFML.System;
using SFML.Window;
using VoxelGame.Meths;
using VoxelGame.Worlds.Tile;

namespace VoxelGame.Worlds
{
    public class Light
    {
        private static byte[,] _lightMap = new byte[Chunk.ChunkSize, Chunk.ChunkSize];
        private static byte[,] _lightMapWall = new byte[Chunk.ChunkSize, Chunk.ChunkSize];

        public static Color BaseColor { get; set; } = Color.White;
        public static void LightingChunk(Chunk chunk, List<Chunk> visibleChunk, List<Vector2f> pointsLight)
        {

            ClearLight(chunk);

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
        }

        private static void ClearLight(Chunk chunk)
        {
            for (int x = 0; x < Chunk.ChunkSize; x++)
            {
                for (int y = 0; y < Chunk.ChunkSize; y++)
                {
                    if(chunk.GetTile(x, y) != null)
                    {
                        chunk.UpdateTileColor(Color.Black, x, y);
                    }
                    if(chunk.GetWall(x, y) != null)
                    {
                        chunk.UpdateWallColor(Color.Black, x, y);
                    }
                }
            }
        }

        private static void ClearLight(List<Chunk> chunks)
        {
            foreach (var chunk in chunks)
            {
                for (int x = 0; x < Chunk.ChunkSize; x++)
                {
                    for (int y = 0; y < Chunk.ChunkSize; y++)
                    {
                        if (chunk.GetTile(x, y) != null)
                        {
                            chunk.UpdateTileColor(Color.Black, x, y);
                        }
                        if (chunk.GetWall(x, y) != null)
                        {
                            chunk.UpdateWallColor(Color.Black, x, y);
                        }
                    }
                }
            }
        }

        private static void PropagateLight(Chunk chunk, int x, int y, int fromX, int fromY, bool wall)
        {
            ITile? tile = chunk.GetTile(x, y, true);
            ITile? tileFrom = chunk.GetTile(fromX, fromY, true);
            ITile? tileFromOpposite = chunk.GetWall(fromX, fromY, true);

            if (wall)
            {
                tile = chunk.GetWall(x, y, true);
                tileFrom = chunk.GetWall(fromX, fromY, true);
                tileFromOpposite = chunk.GetTile(fromX, fromY, true);
            }

            if (tile == null)
                return;

            // Если соседние плитки пустые — пускаем базовый свет
            if (tileFrom == null && tileFromOpposite == null)
            {
                if (!wall)
                {
                    chunk.UpdateTileColor(BaseColor, x, y);
                }
                else
                {
                    chunk.UpdateWallColor(BaseColor, x, y);
                }

                return;
            }

            // Смотрим свет от соседей
            int light = 0;
            if (tileFrom != null)
            {
                if (!wall)
                {
                    light = chunk.GetTileColor(fromX, fromY).R;
                }
                else
                {
                    light = chunk.GetWallColor(fromX, fromY).R;
                }
            }
            else if (tileFromOpposite != null)
            {
                if (!wall)
                {
                    light = chunk.GetTileColor(fromX, fromY).R;
                }
                else
                {
                    light = chunk.GetWallColor(fromX, fromY).R;
                }
            }

            light -= 36;
            light = Math.Clamp(light, 0, 255);

            if (!wall)
            {
                int currentLight = chunk.GetTileColor(x, y).R;

                if (light > currentLight)
                    chunk.UpdateTileColor(new Color((byte)light, (byte)light, (byte)light), x, y);
            }
            else
            {
                int currentLight = chunk.GetWallColor(x, y).R;

                if (light > currentLight)
                    chunk.UpdateWallColor(new Color((byte)light, (byte)light, (byte)light), x, y);
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

        public static void LightPoint(Chunk chunk, List<Vector2f> pointsLight)
        {
            var lights = chunk.GetTilesByType(TileType.Torch);

            foreach (var light in lights)
            {
                for (int x = 0; x < Chunk.ChunkSize; x++)
                {
                    for (int y = 0; y < Chunk.ChunkSize; y++)
                    {
                        if (chunk.GetTile(x, y) != null)
                        {
                            Vector2f tilePos = new Vector2f(x, y) * Tile.Tile.TileSize + chunk.Position;

                            float tileDistanceToLight = MathHelper.Distance(light.GlobalPosition, tilePos);

                            int newLight = byte.MaxValue - (int)(tileDistanceToLight / Tile.Tile.TileSize) * 36;

                            if (newLight < 0)
                                continue;

                            if (newLight > chunk.GetTileColor(x, y).R && newLight > _lightMap[x, y])
                            {
                                _lightMap[x, y] = (byte)newLight;
                                chunk.UpdateTileColor(new Color((byte)_lightMap[x, y], (byte)_lightMap[x, y], (byte)_lightMap[x, y]), x, y);
                            }
                        }

                        if (chunk.GetWall(x, y) != null)
                        {
                            Vector2f tilePos = new Vector2f(x, y) * WallTile.WallSize + chunk.Position;

                            float tileDistanceToLight = MathHelper.Distance(light.GlobalPosition, tilePos);

                            int newLight = byte.MaxValue - (int)tileDistanceToLight / Tile.Tile.TileSize * 36;

                            if (newLight < 0)
                            {
                                newLight = 0;
                            }

                            if (newLight > chunk.GetWallColor(x, y).R && newLight > _lightMapWall[x, y])
                            {
                                if (newLight > 255)
                                    newLight = 255;

                                _lightMapWall[x, y] = (byte)newLight;

                                chunk.UpdateWallColor(new Color((byte)_lightMapWall[x, y], (byte)_lightMapWall[x, y], (byte)_lightMapWall[x, y]), x, y);
                            }
                        }
                    }
                }
            }
        }

        public static void LightPoint(Chunk currentChunk, List<Chunk> visibleChunks, List<Vector2f> pointsLight)
        {

            var lightSources = currentChunk.GetTilesByType(TileType.Torch);

            currentChunk.LightColorTile = new byte[Chunk.ChunkSize, Chunk.ChunkSize];
            currentChunk.LightColorWall = new byte[Chunk.ChunkSize, Chunk.ChunkSize];

            foreach (var chunk in visibleChunks)
            {
                foreach (var light in lightSources)
                {
                    Vector2f lightGlobalPos = light.GlobalPosition;

                    for (int x = 0; x < Chunk.ChunkSize; x++)
                    {
                        for (int y = 0; y < Chunk.ChunkSize; y++)
                        {
                            var tile = chunk.GetTile(x, y, true);
                            var tileWall = chunk.GetWall(x, y);

                            if (tile == null && tileWall == null)
                                continue;

                            

                            if (tile != null)
                            {
                                Vector2f tileGlobalPos = tile.GlobalPosition;
                                float distance = MathHelper.Distance(lightGlobalPos, tileGlobalPos);

                                byte previousColor = chunk.GetTileColor(x, y).R;
                                byte previousColorWall = chunk.GetWallColor(x, y).R;

                                int newLight = byte.MaxValue - (int)(distance / Tile.Tile.TileSize) * 26;

                                if (newLight < 0)
                                    continue;

                                newLight = Math.Clamp(newLight, 0, 255);

                                if (newLight > previousColor && newLight > chunk.LightColorTile[x, y])
                                {
                                    chunk.LightColorTile[x, y] = (byte)newLight;
                                    chunk.UpdateTileColor(new Color((byte)newLight, (byte)newLight, (byte)newLight), x, y);
                                }
                            }

                            if (tileWall != null)
                            {
                                Vector2f tileGlobalPos = tileWall.GlobalPosition;
                                float distance = MathHelper.Distance(lightGlobalPos, tileGlobalPos);

                                byte previousColor = chunk.GetTileColor(x, y).R;
                                byte previousColorWall = chunk.GetWallColor(x, y).R;

                                int newLight = byte.MaxValue - (int)(distance / Tile.Tile.TileSize) * 26;

                                if (newLight < 0)
                                    continue;

                                newLight = Math.Clamp(newLight, 0, 255);

                                if (newLight > previousColorWall && newLight > chunk.LightColorWall[x, y])
                                {
                                    chunk.LightColorWall[x, y] = (byte)newLight;
                                    chunk.UpdateWallColor(new Color((byte)newLight, (byte)newLight, (byte)newLight), x, y);
                                }
                            }
                        }
                    }
                }
            }
        }


        public static Color CalculateGlobalLight(float t)
        {
            // Симметричная синусоида: 0.0 → ночь, 0.5 → день, 1.0 → ночь
            float intensity = 0.05f + 0.95f * MathF.Sin(t * MathF.PI); // 0.2..1.0

            byte light = (byte)(intensity * 255);
            BaseColor = new Color(light, light, light);

            return BaseColor;
        }
    }
}


