using SFML.Graphics;
using SFML.System;
using VoxelGame.Meths;
using VoxelGame.Worlds.Tile;

namespace VoxelGame.Worlds
{
    public class Light
    {
        public static Color BaseColor { get; set; } = Color.White;
        public static void LightingChunk(Chunk chunk, List<Vector2f> pointsLight)
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

            LightPoint(chunk, pointsLight);
        }

        public static void LightTopDown(Chunk chunk, int x)
        {
            for (int y = 0; y < Chunk.ChunkSize; y++)
            {
                var tile = chunk.GetTile(x, y);
                var tileUp = chunk.GetTile(x, y - 1);

                if(tile != null && tileUp == null)
                {
                    chunk.UpdateTileColor(BaseColor, x, y);
                }
                if (tile != null && tile.Type == TileType.Leaves)
                {
                    chunk.UpdateTileColor(Color.Green, x, y);
                    continue;
                }

                if (tile != null && tileUp != null)
                {
                    if (tile.Type == TileType.Wood || tileUp.Type == TileType.Wood)
                    {
                        continue;
                    }

                    int light = chunk.GetTileColor(x, y - 1).R;

                    if (tileUp.IsWall || tile.IsWall)
                    {
                        light -= 16;
                    }
                    else
                    {
                        light -= 36;
                    }

                    if(light < 0)
                        light = 0;

                    chunk.UpdateTileColor(new Color((byte)light, (byte)light, (byte)light), x, y);
                }
            }
        }

        public static void LightDownTop(Chunk chunk, int x)
        {
            for (int y = Chunk.ChunkSize; y > 0; y--)
            {
                var tile = chunk.GetTile(x, y);
                var tileUp = chunk.GetTile(x, y + 1);

                if (tile != null && tileUp == null)
                {
                    chunk.UpdateTileColor(BaseColor, x, y);
                }
                if (tile != null && tile.Type == TileType.Leaves)
                {
                    chunk.UpdateTileColor(Color.Green, x, y);
                    continue;
                }

                if (tile != null && tileUp != null)
                {
                    int lightLeftTile = chunk.GetTileColor(x, y + 1).R;
                    int light = chunk.GetTileColor(x, y).R;
                    int accumLight = lightLeftTile;
                    
                    if(tile.IsWall || tileUp.IsWall)
                        accumLight = lightLeftTile - 16;
                    else
                        accumLight = lightLeftTile - 36;

                    if (accumLight < 0)
                        accumLight = 0;

                    if (light < accumLight)
                        light = accumLight;

                    if (light > 255)
                        light = 255;

                    chunk.UpdateTileColor(new Color((byte)light, (byte)light, (byte)light), x, y);
                }
            }
        }

        public static void LightLeftToRight(Chunk chunk, int y)
        {
            for (int x = 0; x < Chunk.ChunkSize; x++)
            {
                var tile = chunk.GetTile(x, y);
                var tileLeft = chunk.GetTile(x - 1, y);

                if(tile != null && tileLeft == null)
                {
                    chunk.UpdateTileColor(BaseColor, x, y);
                }

                if(tile != null && tileLeft != null)
                {
                    int lightLeftTile = chunk.GetTileColor(x - 1, y).R;
                    int light = chunk.GetTileColor(x, y).R;

                    int accumLight = lightLeftTile;

                    if (tile.IsWall || tileLeft.IsWall)
                        accumLight = lightLeftTile - 16;
                    else
                        accumLight = lightLeftTile - 36;

                    if (light < accumLight)
                        light = accumLight;

                    if (light > 255)
                        light = 255;

                    chunk.UpdateTileColor(new Color((byte)light, (byte)light, (byte)light), x, y);
                }
            }
        }

        public static void LightRightToLeft(Chunk chunk, int y)
        {
            for (int x = Chunk.ChunkSize; x > 0; x--)
            {
                var tile = chunk.GetTile(x, y);
                var tileLeft = chunk.GetTile(x + 1, y);

                if (tile != null && tileLeft == null)
                {
                    chunk.UpdateTileColor(BaseColor, x, y);
                }

                if (tile != null && tileLeft != null)
                {
                    int lightLeftTile = chunk.GetTileColor(x + 1, y).R;
                    int light = chunk.GetTileColor(x, y).R;

                    int accumLight = lightLeftTile;

                    if (tile.IsWall || tileLeft.IsWall)
                        accumLight = lightLeftTile - 16;
                    else
                        accumLight = lightLeftTile - 36;

                    if (light < accumLight)
                        light = accumLight;

                    if (light > 255)
                        light = 255;

                    chunk.UpdateTileColor(new Color((byte)light, (byte)light, (byte)light), x, y);
                }
            }
        }

        public static void LightPoint(Chunk chunk, List<Vector2f> pointsLight)
        {
            foreach (var point in pointsLight)
            {
                for (int x = 0; x < Chunk.ChunkSize; x++)
                {
                    for (int y = 0; y < Chunk.ChunkSize; y++)
                    {
                        if (chunk.GetTile(x, y) != null)
                        {
                            Vector2f tilePos = new Vector2f(x, y) * InfoTile.TileSize + chunk.Position;

                            float tileDistanceToPlayer = MathHelper.Distance(point, tilePos);

                            if (tileDistanceToPlayer < 7 * InfoTile.TileSize)
                            {
                                int light = 255 - ((int)((tileDistanceToPlayer / InfoTile.TileSize) - 1) * 26);
                                if (light < 0)
                                    light = 0;

                                var tileColor = chunk.GetTileColor(x, y);

                                //if (tileColor.R > light)
                                    //light = chunk.GetTileColor(x, y).R;

                                if (light > 255)
                                    light = 255;

                                chunk.UpdateTileColor(new Color((byte)light, (byte)light, (byte)light), x, y);
                            }
                        }
                    }
                }
            }
        }
    }
}
