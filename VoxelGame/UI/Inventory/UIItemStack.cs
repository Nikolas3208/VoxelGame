using SFML.Graphics;
using SFML.System;
using VoxelGame.Graphics;
using VoxelGame.Item;
using VoxelGame.Resources;

namespace VoxelGame.UI.Inventory
{
    public class UIItemStack : UIBase
    {
        private Sprite _sprite;
        private Text _text;

        private int _itemCount;

        public Item.Item Item;

        public ItemType ItemType => Item.Type;

        public int ItemCount
        {
            get => _itemCount;
            set
            {
                if (value < Item.MaxCoutnInStack + 1)
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

        public bool IsFull => _itemCount >= Item.MaxCoutnInStack;

        public UIItemStack(Item.Item infoItem)
        {
            Item = infoItem;

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
            _sprite.Scale = new Vector2f(2f, 2f);
            _sprite.Origin = new Vector2f(spriteSheet.SubWidth, spriteSheet.SubHeight) / 2;
            _sprite.Position = new Vector2f(80, 80) / 2;

            _text = new Text(ItemCount.ToString(), AssetManager.GetFont("Arial"));
            _text.Position = _sprite.Position - new Vector2f(10, 50);
            _text.FillColor = Color.Black;

            ItemCount = 1;
        }

        public override void Update(float deltaTime)
        {
            _text.DisplayedString = ItemCount.ToString();

            var perent = Perent as UIInventoryCell;

            if(perent.IsSelected)
            {
                _sprite.Position = new Vector2f(54, 54) / 2;
            }
            else
            {
                _sprite.Position = new Vector2f(52, 52) / 2;
            }
        }

        public override void Draw(RenderTarget target, RenderStates states)
        {
            states.Transform *= Transform;

            target.Draw(_sprite, states);

            if (Item.MaxCoutnInStack != 1)
                target.Draw(_text, states);
        }
    }
}
