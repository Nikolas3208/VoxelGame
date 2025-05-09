using SFML.Graphics;
using SFML.System;
using VoxelGame.Resources;

namespace VoxelGame.Worlds.Tile
{
    public enum TileEntityType
    { 
        Chest
    }

    public abstract class TileEntity : Transformable, Drawable
    {
        protected RectangleShape rect;

        public int Id { get; set; }
        public TileEntityType Type { get; }

        public Vector2f Size { get => rect.Size; }

        public Chunk? Chunk { get; set; }

        public TileEntity(TileEntityType type)
        {
            Type = type;

            rect = new RectangleShape(new Vector2f(InfoTile.TileSize, InfoTile.TileSize));
            rect.Texture = AssetManager.GetSpriteSheet("terrain").Texture;
            rect.TextureRect = AssetManager.GetSpriteSheet("terrain").GetTextureRect(27);
            //rect.Origin = -rect.Size / 2;
        }

        public TileEntity(TileEntityType type, Vector2f size) :this(type)
        {
            rect.Size = size;
        }

        public virtual void Update(float deltaTime)
        {

        }

        public virtual void Draw(RenderTarget target, RenderStates states)
        {
            states.Transform *= Transform;

            target.Draw(rect, states);
        }

        public abstract void Use();
    }
}
