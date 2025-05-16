using SFML.Graphics;
using SFML.System;
using VoxelGame.Item;

namespace VoxelGame.Worlds.Tile
{
    public class TileTorch : Tile
    {
        private int _torchSpriteSize = 22;

        public TileTorch(TileType type, ItemList dropItem, ItemType requiredTool, int reqiuredToolPower, float strength, bool isSolid, 
            Chunk perentChunk, Tile? upTile, Tile? downTile, Tile? leftTile, Tile? rightTile, WallTile? wall, Vector2f localPosition) 
            : base(type, dropItem, requiredTool, reqiuredToolPower, strength, isSolid,
                  perentChunk, upTile, downTile, leftTile, rightTile, wall, localPosition)
        {

        }

        public override void UpdateView()
        {
            Vector2u texturePosFraq = new Vector2u(0, 0); // Позиция спрайта на текстурном атласе

            if (WallTile != null || DownTile != null)
            {
                texturePosFraq = new Vector2u(0, 0);
            }
            else if (WallTile == null && LeftTile != null)
            {
                texturePosFraq = new Vector2u(1, 0);
            }
            else if (WallTile == null && RightTile != null)
            {
                texturePosFraq = new Vector2u(2, 0);
            }

            // Получаем текстурные координаты
            int x = (int)(texturePosFraq.X * _torchSpriteSize);
            int y = (int)(texturePosFraq.Y * _torchSpriteSize);

            _vertices[0] = new Vertex(new Vector2f(LocalPosition.X * Tile.TileSize, LocalPosition.Y * Tile.TileSize));
            _vertices[1] = new Vertex(new Vector2f(LocalPosition.X * Tile.TileSize, LocalPosition.Y * Tile.TileSize + Tile.TileSize));
            _vertices[2] = new Vertex(new Vector2f(LocalPosition.X * Tile.TileSize + Tile.TileSize, LocalPosition.Y * Tile.TileSize));
            _vertices[3] = new Vertex(new Vector2f(LocalPosition.X * Tile.TileSize + Tile.TileSize, LocalPosition.Y * Tile.TileSize));
            _vertices[4] = new Vertex(new Vector2f(LocalPosition.X * Tile.TileSize, LocalPosition.Y * Tile.TileSize + Tile.TileSize));
            _vertices[5] = new Vertex(new Vector2f(LocalPosition.X * Tile.TileSize + Tile.TileSize, LocalPosition.Y * Tile.TileSize + Tile.TileSize));

            _vertices[0].TexCoords = new Vector2f(x, y);

            _vertices[1].TexCoords = new Vector2f(x, _torchSpriteSize - 2 + y);
            _vertices[2].TexCoords = new Vector2f(_torchSpriteSize - 2 + x, y);
            _vertices[3].TexCoords = new Vector2f(_torchSpriteSize - 2 + x, y);
            _vertices[4].TexCoords = new Vector2f(x, _torchSpriteSize - 2 + y);

            _vertices[5].TexCoords = new Vector2f(_torchSpriteSize - 2 + x, _torchSpriteSize - 2 + y);

            PerentChunk?.UpdateTileViewByWorldPosition(GlobalPosition, _vertices);
        }
    }
}
