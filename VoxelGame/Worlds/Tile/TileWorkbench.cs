using SFML.Graphics;
using SFML.System;
using VoxelGame.Item;

namespace VoxelGame.Worlds.Tile
{
    public class TileWorkbench : Tile
    {
        private int _workbanchSpriteX = 16;
        private int _workbanchSpriteY = 18;

        private TileWorkbench _perentTile;

        public new TileWorkbench PerentTile { get => _perentTile; set { _perentTile = value; base.PerentTile = value; UpdateView(); } }

        public TileWorkbench(TileType type, ItemList dropItem, ItemType requiredTool, int reqiuredToolPower, float strength, bool isSolid,
                    Chunk perentChunk, Tile? upTile, Tile? downTile, Tile? leftTile, Tile? rightTile, WallTile? wall, Vector2f localPosition) 
            : base(type, dropItem, requiredTool, reqiuredToolPower, strength, isSolid, perentChunk, upTile, downTile, leftTile, rightTile, wall, localPosition)
        {
            Size = new Vector2f(2, 1);
        }

        public override void UpdateView()
        {
            if (PerentTile != null)
            {
                if (PerentTile == LeftTile && RightTile is not TileWorkbench)
                {
                    GenerateTileMesh(1, 0);
                }
                else if (PerentTile == RightTile && LeftTile is not TileWorkbench)
                {
                    GenerateTileMesh(0, 0);
                }
            }
            else if (PerentTile == null)
            {
                if (LeftTile == null && RightTile is not TileWorkbench || LeftTile is TileWorkbench)
                {
                    GenerateTileMesh(1, 0);
                }
                else if (RightTile == null && LeftTile is not TileWorkbench || RightTile is TileWorkbench)
                {
                    GenerateTileMesh(0, 0);
                }
            }
        }

        public void GenerateTileMesh(int x, int y)
        {
            _vertices[0] = new Vertex(new Vector2f(LocalPosition.X * Tile.TileSize, LocalPosition.Y * Tile.TileSize));
            _vertices[1] = new Vertex(new Vector2f(LocalPosition.X * Tile.TileSize, LocalPosition.Y * Tile.TileSize + Tile.TileSize));
            _vertices[2] = new Vertex(new Vector2f(LocalPosition.X * Tile.TileSize + Tile.TileSize, LocalPosition.Y * Tile.TileSize));
            _vertices[3] = new Vertex(new Vector2f(LocalPosition.X * Tile.TileSize + Tile.TileSize, LocalPosition.Y * Tile.TileSize));
            _vertices[4] = new Vertex(new Vector2f(LocalPosition.X * Tile.TileSize, LocalPosition.Y * Tile.TileSize + Tile.TileSize));
            _vertices[5] = new Vertex(new Vector2f(LocalPosition.X * Tile.TileSize + Tile.TileSize, LocalPosition.Y * Tile.TileSize + Tile.TileSize));

            x = (int)(x * _workbanchSpriteX + x * 2);
            y = (int)(y * _workbanchSpriteY + y * 2);

            _vertices[0].TexCoords = new Vector2f(x, y);

            _vertices[1].TexCoords = new Vector2f(x, _workbanchSpriteY + y);
            _vertices[2].TexCoords = new Vector2f(_workbanchSpriteX + x, y);
            _vertices[3].TexCoords = new Vector2f(_workbanchSpriteX + x, y);
            _vertices[4].TexCoords = new Vector2f(x, _workbanchSpriteY + y);

            _vertices[5].TexCoords = new Vector2f(_workbanchSpriteX + x, _workbanchSpriteY + y);

            PerentChunk?.UpdateTileViewByWorldPosition(GlobalPosition, _vertices);
        }
    }
}