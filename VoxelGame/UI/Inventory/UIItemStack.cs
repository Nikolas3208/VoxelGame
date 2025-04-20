using SFML.Graphics;
using VoxelGame.Graphics;
using VoxelGame.Item;
using VoxelGame.Resources;

namespace VoxelGame.UI.Inventory
{
    public class UIItemStack : UIBase
    {
        private InfoItem _infoItem;
        private Sprite _sprite;

        private int _itemCount;

        public ItemType ItemType => _infoItem.Type;

        public int ItemCount
        {
            get => _itemCount;
            set
            {
                if (value < _infoItem.MaxCoutnInStack)
                {
                    _itemCount = value;
                }

                if (value == 0)
                {
                    var perent = Perent as UIInventoryCell;
                    if (perent != null)
                    {
                        perent.ItemStack = null;
                    }
                }
            }
        }

        public bool IsFull => _itemCount >= _infoItem.MaxCoutnInStack;

        public UIItemStack(InfoItem infoItem)
        {
            _infoItem = infoItem;

            SpriteSheet spriteSheet;

            switch(infoItem.Type)
            {
                case ItemType.Tile:
                    spriteSheet = AssetManager.GetSpriteSheet("terrain");
                    break;
                default:
                    spriteSheet = AssetManager.GetSpriteSheet("items");
                    break;
            }

            _sprite = spriteSheet.Sprite;
            _sprite.TextureRect = spriteSheet.GetTextureRect(infoItem.SpriteIndex);
        }

        public override void Update(float deltaTime)
        {

        }

        public override void Draw(RenderTarget target, RenderStates states)
        {
            states.Transform *= Transform;

            target.Draw(_sprite, states);
        }
    }
}
