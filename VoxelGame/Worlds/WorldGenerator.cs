using SFML.System;
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
        chunk.Position = new Vector2f(x, y) * InfoTile.TileSize;
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

                    if (blend < 0.5f)
                        chunk.SetTile(cX, cY, TileType.Ground);
                    else
                    {
                        chunk.SetTile(cX, cY, TileType.Stone);
                    }
                }
                else
                {
                    // Пустое место — пещера
                    chunk.SetTile(cX, cY, null);
                }
            }
        }

        // Генерация травы
        for (int cX = 0; cX < Chunk.ChunkSize; cX++)
        {
            int cY = heightMap[cX] - y;
            var tile = chunk.GetTile(cX, cY);

            if (tile != null && tile.Type == TileType.Ground)
            {
                chunk.SetTile(cX, cY, TileType.Grass);
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
                        int treeHeight = _perlin.Random.Next(6, 12);

                        // Ставим ствол
                        for (int h = 1; h <= treeHeight; h++)
                        {
                            if(!chunk.SetTile(x, y - h, TileType.Wood))
                            {
                                world.GetChunkByWorldPosition(chunk.Position - new Vector2f(0, 1))?.SetTile(x, Chunk.ChunkSize + (y - h), TileType.Wood);
                            }
                        }

                        // Ставим листву
                        int top = y - treeHeight;
                        for (int dx = -2; dx <= 2; dx++)
                        {
                            for (int dy = -2; dy <= 2; dy++)
                            {
                                int leafX = x + dx;
                                int leafY = top + dy;

                                float dist = MathF.Abs(dx) + MathF.Abs(dy); // крест
                                if (dist <= 2)
                                {
                                    // Только если нет другого блока
                                    if (chunk.GetTile(leafX, leafY) == null)
                                    {
                                        if (!chunk.SetTile(leafX, leafY, TileType.Leaves))
                                        {
                                            if (world.GetChunkByWorldPosition(chunk.Position - new Vector2f(0, 1))?.GetTile(leafX, Chunk.ChunkSize + leafY) == null)
                                                world.GetChunkByWorldPosition(chunk.Position - new Vector2f(0, 1))?.SetTile(leafX, Chunk.ChunkSize + leafY, TileType.Leaves);
                                        }
                                    }
                                    else
                                    {

                                    }
                                }
                            }
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