using SFML.Graphics;
using SFML.System;
using VoxelGame.Item;

namespace VoxelGame.Worlds.Tile
{
    public class TileStove : Tile
    {
        private int _stoveSpriteSize = 16;

        public TileStove(TileType type, ItemList dropItem, ItemType requiredTool, int reqiuredToolPower, float strength, bool isSolid,
            Chunk perentChunk, Tile? upTile, Tile? downTile, Tile? leftTile, Tile? rightTile, WallTile? wall, Vector2f localPosition) 
            : base(type, dropItem, requiredTool, reqiuredToolPower, strength, isSolid,
                  perentChunk, upTile, downTile, leftTile, rightTile, wall, localPosition)
        {
            Size = new Vector2f(3, 2);
        }

        public override void UpdateView()
        {
            Vector2u texturePosFraq = new Vector2u(0, 0); // Позиция спрайта на текстурном атласе

            if(RightTile != null && RightTile is TileStove && DownTile != null && DownTile is TileStove && LeftTile is not TileStove)
            {
                texturePosFraq = new Vector2u(0, 0);
            }
            else if(RightTile != null && RightTile is TileStove && DownTile != null 
                && DownTile is TileStove && LeftTile != null && LeftTile is TileStove)
            {
                texturePosFraq = new Vector2u(1, 0);
            }
            else if (RightTile is not TileStove && DownTile != null
                && DownTile is TileStove && LeftTile != null && LeftTile is TileStove)
            {
                texturePosFraq = new Vector2u(2, 0);
            }
            else if(RightTile != null && RightTile is TileStove && UpTile != null 
                && UpTile is TileStove && LeftTile is not TileStove)
            {
                texturePosFraq = new Vector2u(0, 1);
            }
            else if (RightTile != null && RightTile is TileStove && UpTile != null && UpTile is TileStove
                && LeftTile != null && LeftTile is TileStove)
            {
                texturePosFraq = new Vector2u(1, 1);
            }
            else if(RightTile is not TileStove && UpTile != null && UpTile is TileStove
                && LeftTile != null && LeftTile is TileStove)
            {
                texturePosFraq = new Vector2u(2, 1);
            }

            // Получаем текстурные координаты
            int x = (int)(texturePosFraq.X * _stoveSpriteSize + texturePosFraq.X * 2);
            int y = (int)(texturePosFraq.Y * _stoveSpriteSize + texturePosFraq.Y * 2);

            _vertices[0] = new Vertex(new Vector2f(LocalPosition.X * Tile.TileSize, LocalPosition.Y * Tile.TileSize));
            _vertices[1] = new Vertex(new Vector2f(LocalPosition.X * Tile.TileSize, LocalPosition.Y * Tile.TileSize + Tile.TileSize));
            _vertices[2] = new Vertex(new Vector2f(LocalPosition.X * Tile.TileSize + Tile.TileSize, LocalPosition.Y * Tile.TileSize));
            _vertices[3] = new Vertex(new Vector2f(LocalPosition.X * Tile.TileSize + Tile.TileSize, LocalPosition.Y * Tile.TileSize));
            _vertices[4] = new Vertex(new Vector2f(LocalPosition.X * Tile.TileSize, LocalPosition.Y * Tile.TileSize + Tile.TileSize));
            _vertices[5] = new Vertex(new Vector2f(LocalPosition.X * Tile.TileSize + Tile.TileSize, LocalPosition.Y * Tile.TileSize + Tile.TileSize));

            _vertices[0].TexCoords = new Vector2f(x, y);

            _vertices[1].TexCoords = new Vector2f(x, _stoveSpriteSize + y);
            _vertices[2].TexCoords = new Vector2f(_stoveSpriteSize + x, y);
            _vertices[3].TexCoords = new Vector2f(_stoveSpriteSize + x, y);
            _vertices[4].TexCoords = new Vector2f(x, _stoveSpriteSize + y);

            _vertices[5].TexCoords = new Vector2f(_stoveSpriteSize + x, _stoveSpriteSize + y);

            PerentChunk?.UpdateTileViewByWorldPosition(GlobalPosition, _vertices);
        }
    }
}
