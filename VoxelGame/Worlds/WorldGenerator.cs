using SFML.System;
using System;
using VoxelGame.Meths;
using VoxelGame.Worlds.Tile;

namespace VoxelGame.Worlds;

public static class WorldGenerator
{
    private static PerlinNoise _perlin;
    public static World GenerateWorld(int width, int height, int seed = -1)
    {
        _perlin = seed != -1 ? new PerlinNoise(seed) : new PerlinNoise((int)DateTime.Now.Ticks);

        World world = new World(width, height, _perlin.Seed);
        World.Random = _perlin.Random;

        for (int x = 0; x < width / Chunk.ChunkSize; x++)
        {
            for (int y = 0; y < height / Chunk.ChunkSize; y++)
            {
                Chunk chunk = GenerateChunk(world, _perlin, x * Chunk.ChunkSize, y * Chunk.ChunkSize);
                world.SetChunk(x, y, chunk);
            }
        }

        return world;
    }

    private static Chunk GenerateChunk(World world, PerlinNoise perlin, int x, int y)
    {
        int[] heightMap = new int[Chunk.ChunkSize];

        for (int cX = 0; cX < Chunk.ChunkSize; cX++)
        {
            float terrain = perlin.Noise(cX + x, 0, octaves: 5, frequency: 0.03f, amplitude: 1f, persistence: 0.5f);
            float mountains = perlin.Noise(cX + x, 0, octaves: 3, frequency: 0.05f, amplitude: 1.8f, persistence: 0.4f);
            float blend = perlin.Noise(cX + x, 0, octaves: 2, frequency: 0.02f, amplitude: 1f, persistence: 0.5f);
            float blendNormalized = (blend + 1f) / 2f; // от 0 до 1

            float height =
                world.BaseHeight +
                (1f - blendNormalized) * terrain * 20f +
                blendNormalized * mountains * 45f;

            heightMap[cX] = (int)height;
        }

        Chunk chunk = new Chunk(world);
        chunk.X = x / Tile.Tile.TileSize;
        chunk.Y = y / Tile.Tile.TileSize;
        chunk.Position = new Vector2f(x, y) * Tile.Tile.TileSize;
        chunk.Id = x / Chunk.ChunkSize * (int)(world.ChunkCountX / Chunk.ChunkSize) + y / Chunk.ChunkSize;

        for (int cX = 0; cX < Chunk.ChunkSize; cX++)
        {
            for (int cY = heightMap[cX] - y; cY < Chunk.ChunkSize; cY++)
            {
                // --- Пещерный шум ---
                float caveNoise = perlin.Noise((x + cX) * 0.5f, (y + cY) * 0.5f, octaves: 3, frequency: 0.05f, amplitude: 1.2f, persistence: 0.5f);
                float caveThreshold = 0.19f;

                // Если шум выше порога — это пещера
                bool isCave = caveNoise > caveThreshold && cY + y > world.BaseHeight - 6f;

                if (!isCave) // Только если это не пещера
                {
                    int depth = cY + y - heightMap[cX];
                    float blend = depth / (float)16f;
                    blend = Math.Clamp(blend, 0f, 1f);

                    float noise = (perlin.Noise(x + cX, y + cY, 2, 0.1f, 1f, 0.5f) + 1f) / 2f;
                    blend = (blend + noise * 0.3f) / 1.26f;

                    bool setWall = cY > heightMap[cX] - y + 1;

                    if (blend < 0.5f)
                        chunk.SetTile(cX, cY, TileType.Ground, setWall: setWall);
                    else
                    {
                        chunk.SetTile(cX, cY, TileType.Stone, setWall: setWall);
                    }
                }
                else
                {
                    // Пустое место — пещера
                    var tile = chunk.GetTile(cX, cY);

                    if (tile != null)
                        chunk.SetWall(cX, cY, Tiles.TileTypeToWallType(tile.Type));
                    else
                        chunk.SetWall(cX, cY, WallType.StoneWall);
                }
            }
        }

        // Генерация травы
        for (int cX = 0; cX < Chunk.ChunkSize; cX++)
        {
            int cY = heightMap[cX] - y;
            var tile = chunk.GetTile(cX, cY);

            // Если это земля и над ней пусто, то ставим траву
            if (tile != null && tile.Type == TileType.Ground && chunk.GetTile(cX, cY - 1) == null)
            {
                chunk.SetTile(cX, cY, TileType.Grass);

                if (_perlin.Random.NextDouble() < 0.30) // 30%
                {
                    // Шанс на траву
                    chunk.SetTile(cX, cY - 1, TileType.Vegetation);
                }
            }
        }

        // Генерация деревьев
        AddTrees(world, chunk);

        // Генерация руды
        GenerateIronOre(world, chunk);

        return chunk;
    }

    private static void AddTrees(World world, Chunk chunk)
    {
        for (int x = 2; x < Chunk.ChunkSize - 2; x++) // отступы, чтобы не вылезти за границы
        {
            for (int y = 0; y < Chunk.ChunkSize; y++)
            {
                var tile = chunk.GetTile(x, y);
                if (tile != null && tile.Type == TileType.Grass)
                {
                    // Шанс на дерево
                    if (_perlin.Random.NextDouble() < 0.09) // 9%
                    {
                        TileType treeType = _perlin.Random.Next(0, 2) == 1 ? TileType.Oak : TileType.Oak;

                        int treeHeight = _perlin.Random.Next(6, 12);

                        // Ставим ствол
                        for (int h = 1; h <= treeHeight; h++)
                        {
                            if(!chunk.SetTile(x, y - h, treeType))
                            {
                                world.GetChunkByWorldPosition(chunk.Position - new Vector2f(0, 1))?.SetTile(x, Chunk.ChunkSize + (y - h), treeType);
                            }
                        }

                        // Ставим листву
                        int top = y - treeHeight - 1;
                        if (!chunk.SetTile(x, top, TileType.Leaves))
                        {
                            if (world.GetChunk(chunk.X, chunk.Y - 1)?.GetTile(x, Chunk.ChunkSize + top) == null)
                                world.GetChunk(chunk.X, chunk.Y - 1)?.SetTile(x, Chunk.ChunkSize + top, TileType.Leaves);
                        }

                        // Переход к следующему дереву (не ставим рядом)
                        x += 3;
                        break;
                    }
                }
            }
        }
    }

    private static void GenerateIronOre(World world, Chunk chunk)
    {
        for (int x = 0; x < Chunk.ChunkSize; x++)
        {
            for (int y = 0; y < Chunk.ChunkSize; y++)
            {
                // Генерируем руду только в глубине мира
                if (y + chunk.Position.Y > world.BaseHeight + 10)
                {
                    var tile = chunk.GetTile(x, y);

                    // Только если это камень
                    if (tile != null && tile.Type == TileType.Stone)
                    {
                        // 2D шум для пятен руды
                        float oreNoise = _perlin.Noise((x + chunk.Position.X) * 0.6f, (y + chunk.Position.Y) * 0.6f, octaves: 3, frequency: 0.09f, amplitude: 1.2f, persistence: 0.6f);
                        float oreThreshold = 0.32f;

                        if (oreNoise > oreThreshold)
                        {
                            chunk.SetTile(x, y, TileType.IronOre);
                        }
                    }
                }
            }
        }
    }
}