using SFML.Graphics;
using SFML.System;
using VoxelGame.Item;

namespace VoxelGame.Worlds.Tile
{
    public class TileVegetation : Tile
    {
        private int _vegetationSpriteSizeX = 18;
        private int _vegetationSpriteSizeY = 20;

        private int _randNum = World.Random.Next(0, 5);

        public TileVegetation(TileType type, ItemList dropItem, ItemType requiredTool, int reqiuredToolPower, float strength, bool isSolid, 
            Chunk perentChunk, Tile? upTile, Tile? downTile, Tile? leftTile, Tile? rightTile, WallTile? wall, Vector2f localPosition) 
            : base(type, dropItem, requiredTool, reqiuredToolPower, strength, isSolid, 
                  perentChunk, upTile, downTile, leftTile, rightTile, wall, localPosition)
        {

        }

        public override void UpdateView()
        {
            if(DownTile == null)
            {
                PerentChunk.SetTile((int)LocalPosition.X, (int)LocalPosition.Y, TileType.None);
            }

            // Получаем текстурные координаты
            int x = (int)(_randNum * _vegetationSpriteSizeX);
            int y = 0;

            _vertices[0] = new Vertex(new Vector2f(LocalPosition.X * Tile.TileSize, LocalPosition.Y * Tile.TileSize));
            _vertices[1] = new Vertex(new Vector2f(LocalPosition.X * Tile.TileSize, LocalPosition.Y * Tile.TileSize + Tile.TileSize));
            _vertices[2] = new Vertex(new Vector2f(LocalPosition.X * Tile.TileSize + Tile.TileSize, LocalPosition.Y * Tile.TileSize));
            _vertices[3] = new Vertex(new Vector2f(LocalPosition.X * Tile.TileSize + Tile.TileSize, LocalPosition.Y * Tile.TileSize));
            _vertices[4] = new Vertex(new Vector2f(LocalPosition.X * Tile.TileSize, LocalPosition.Y * Tile.TileSize + Tile.TileSize));
            _vertices[5] = new Vertex(new Vector2f(LocalPosition.X * Tile.TileSize + Tile.TileSize, LocalPosition.Y * Tile.TileSize + Tile.TileSize));

            _vertices[0].TexCoords = new Vector2f(x, y);

            _vertices[1].TexCoords = new Vector2f(x, _vegetationSpriteSizeY + y);
            _vertices[2].TexCoords = new Vector2f(_vegetationSpriteSizeX + x, y);
            _vertices[3].TexCoords = new Vector2f(_vegetationSpriteSizeX + x, y);
            _vertices[4].TexCoords = new Vector2f(x, _vegetationSpriteSizeY + y);

            _vertices[5].TexCoords = new Vector2f(_vegetationSpriteSizeX + x, _vegetationSpriteSizeY + y);

            PerentChunk?.UpdateTileViewByWorldPosition(GlobalPosition, _vertices);
        }
    }
}
