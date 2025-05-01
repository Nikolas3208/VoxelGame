using SFML.Graphics;
using SFML.System;

namespace VoxelGame.Worlds.Tile
{
    public abstract class TileEntity : Transformable, Drawable
    {
        protected RectangleShape rect;

        public TileType Type { get; }
        public Vector2f Size { get => rect.Size; }

        public TileEntity(TileType type)
        {
            Type = type;

            rect = new RectangleShape(new Vector2f(InfoTile.TileSize,  InfoTile.TileSize));
        }

        public TileEntity(TileType type, Vector2f size)
        {
            Type = type;
            rect = new RectangleShape(size);
        }

        public virtual void Draw(RenderTarget target, RenderStates states)
        {
            states.Transform *= Transform;

            target.Draw(rect, states);
        }

        public abstract void Use();
    }
}
