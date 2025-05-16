using SFML.Graphics;
using SFML.System;
using System;
using System.Reflection;
using VoxelGame.Item;

namespace VoxelGame.Worlds.Tile
{
    public class TileTree : Tile
    {
        public const int TreeSize = 20;
        public const int TreeTopSize = 60;
        private int _treeSpriteSize = 22;
        private int _treeTopSpriteSize = 80;


        public TileTree(TileType type, ItemList dropItem, ItemType requiredTool, int reqiuredToolPower, float strength, bool isSolid, 
            Chunk perentChunk, Tile? upTile, Tile? downTile, Tile? leftTile, Tile? rightTile, WallTile? wall, Vector2f localPosition, bool treeTopTile = false) 
            : base(type, dropItem, requiredTool, reqiuredToolPower, strength, isSolid,
                  perentChunk, upTile, downTile, leftTile, rightTile, wall, localPosition)
        {
            IsTreeTop = treeTopTile;

            UpdateView();
        }

        public override void UpdateView()
        {
            Vector2u texturePosFraq = new Vector2u(0, 0); // Позиция спрайта на текстурном атласе

            if (!IsTreeTop)
            {
                switch (Type)
                {
                    default:
                        if ((_upTile == null || _upTile != null) && _downTile != null && (_leftTile == null || !_leftTile.IsTree) && (_rightTile == null || !_rightTile.IsTree))
                        {
                            int i = World.Random.Next(0, 3); // Случайное число от 0 до 2
                            texturePosFraq = new Vector2u(0, (uint)i);
                        }
                        break;
                }

                // Получаем текстурные координаты
                int x = (int)(texturePosFraq.X * _treeSpriteSize);
                int y = (int)(texturePosFraq.Y * _treeSpriteSize);

                _vertices[0] = new Vertex(new Vector2f(LocalPosition.X * Tile.TileSize, LocalPosition.Y * Tile.TileSize));
                _vertices[1] = new Vertex(new Vector2f(LocalPosition.X * Tile.TileSize, LocalPosition.Y * Tile.TileSize + TileTree.TreeSize));
                _vertices[2] = new Vertex(new Vector2f(LocalPosition.X * Tile.TileSize + Tile.TileSize, LocalPosition.Y * Tile.TileSize));
                _vertices[3] = new Vertex(new Vector2f(LocalPosition.X * Tile.TileSize + Tile.TileSize, LocalPosition.Y * Tile.TileSize));
                _vertices[4] = new Vertex(new Vector2f(LocalPosition.X * Tile.TileSize, LocalPosition.Y * Tile.TileSize + TileTree.TreeSize));
                _vertices[5] = new Vertex(new Vector2f(LocalPosition.X * Tile.TileSize + Tile.TileSize, LocalPosition.Y * Tile.TileSize + TileTree.TreeSize));

                _vertices[0].TexCoords = new Vector2f(x, y);

                _vertices[1].TexCoords = new Vector2f(x, _treeSpriteSize + y);
                _vertices[2].TexCoords = new Vector2f(_treeSpriteSize + x, y);
                _vertices[3].TexCoords = new Vector2f(_treeSpriteSize + x, y);
                _vertices[4].TexCoords = new Vector2f(x, _treeSpriteSize + y);

                _vertices[5].TexCoords = new Vector2f(_treeSpriteSize + x, _treeSpriteSize + y);
            }
            else
            {
                _vertices[0] = new Vertex(new Vector2f(LocalPosition.X * Tile.TileSize, LocalPosition.Y * Tile.TileSize) - new Vector2f(23, 32), new Vector2f(0, 0));
                _vertices[1] = new Vertex(new Vector2f(LocalPosition.X * Tile.TileSize, LocalPosition.Y * Tile.TileSize + TreeTopSize) - new Vector2f(23, 32), new Vector2f(0, _treeTopSpriteSize));
                _vertices[2] = new Vertex(new Vector2f(LocalPosition.X * Tile.TileSize + TreeTopSize, LocalPosition.Y * Tile.TileSize) - new Vector2f(23, 32), new Vector2f(_treeTopSpriteSize, 0));
                _vertices[3] = new Vertex(new Vector2f(LocalPosition.X * Tile.TileSize + TreeTopSize, LocalPosition.Y * Tile.TileSize) - new Vector2f(23, 32), new Vector2f(_treeTopSpriteSize, 0));
                _vertices[4] = new Vertex(new Vector2f(LocalPosition.X * Tile.TileSize, LocalPosition.Y * Tile.TileSize + TreeTopSize) - new Vector2f(23, 32), new Vector2f(0, _treeTopSpriteSize));
                _vertices[5] = new Vertex(new Vector2f(LocalPosition.X * Tile.TileSize + TreeTopSize, LocalPosition.Y * Tile.TileSize + TreeTopSize) - new Vector2f(23, 32), new Vector2f(_treeTopSpriteSize, _treeTopSpriteSize));
            }
            PerentChunk?.UpdateTileViewByWorldPosition(GlobalPosition, _vertices);
        }
    }
}
