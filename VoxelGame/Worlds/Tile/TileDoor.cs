using SFML.Graphics;
using SFML.System;
using VoxelGame.Item;

namespace VoxelGame.Worlds.Tile
{
    public class TileDoor : Tile
    {
        public TileDoor(TileType type, ItemList dropItem, ItemType requiredTool, int reqiuredToolPower, float strength, bool isSolid, Chunk perentChunk, Tile? upTile, Tile? downTile, Tile? leftTile, Tile? rightTile, WallTile? wall, Vector2f localPosition) : base(type, dropItem, requiredTool, reqiuredToolPower, strength, isSolid, perentChunk, upTile, downTile, leftTile, rightTile, wall, localPosition)
        {
            TextureName = "DoorClose";
            Size = new Vector2f(1, 3);
        }

        public override void UpdateView()
        {
            Vector2u texturePosFraq = new Vector2u(0, 0); // Позиция спрайта на текстурном атласе

            if(UpTile != null && UpTile is TileDoor && DownTile != null && DownTile is TileDoor)
            {
                texturePosFraq = new Vector2u(0, 1);
            }
            else if(UpTile != null && UpTile is TileDoor && DownTile != null && DownTile is not TileDoor)
            {
                texturePosFraq = new Vector2u(0, 2);

            }
            else if (UpTile is not TileDoor && DownTile != null && DownTile is TileDoor)
            {
                texturePosFraq = new Vector2u(0, 0);

            }

            // Получаем текстурные координаты
            int x = (int)(texturePosFraq.X * Tile.TileSize + texturePosFraq.X * 2);
            int y = (int)(texturePosFraq.Y * Tile.TileSize + texturePosFraq.Y * 2);

            _vertices[0] = new Vertex(new Vector2f(LocalPosition.X * Tile.TileSize, LocalPosition.Y * Tile.TileSize));
            _vertices[1] = new Vertex(new Vector2f(LocalPosition.X * Tile.TileSize, LocalPosition.Y * Tile.TileSize + Tile.TileSize));
            _vertices[2] = new Vertex(new Vector2f(LocalPosition.X * Tile.TileSize + Tile.TileSize, LocalPosition.Y * Tile.TileSize));
            _vertices[3] = new Vertex(new Vector2f(LocalPosition.X * Tile.TileSize + Tile.TileSize, LocalPosition.Y * Tile.TileSize));
            _vertices[4] = new Vertex(new Vector2f(LocalPosition.X * Tile.TileSize, LocalPosition.Y * Tile.TileSize + Tile.TileSize));
            _vertices[5] = new Vertex(new Vector2f(LocalPosition.X * Tile.TileSize + Tile.TileSize, LocalPosition.Y * Tile.TileSize + Tile.TileSize));

            _vertices[0].TexCoords = new Vector2f(x, y);

            _vertices[1].TexCoords = new Vector2f(x, Tile.TileSize + y);
            _vertices[2].TexCoords = new Vector2f(Tile.TileSize + x, y);
            _vertices[3].TexCoords = new Vector2f(Tile.TileSize + x, y);
            _vertices[4].TexCoords = new Vector2f(x, Tile.TileSize + y);

            _vertices[5].TexCoords = new Vector2f(Tile.TileSize + x, Tile.TileSize + y);

            PerentChunk?.UpdateTileViewByWorldPosition(GlobalPosition, _vertices);
        }
    }
}
