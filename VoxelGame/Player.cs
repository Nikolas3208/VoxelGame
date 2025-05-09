using SFML.Graphics;
using SFML.System;
using SFML.Window;
using VoxelGame.Item;
using VoxelGame.Meths;
using VoxelGame.Physics;
using VoxelGame.UI;
using VoxelGame.UI.Inventory;
using VoxelGame.Worlds;

namespace VoxelGame
{
    public class Player : Entity
    {
        private float maxSpeed = 150;
        private bool isJumped = false;
        private UIInventory _inventory;
        private Item.Item? _selectedItem;

        public Player(World world, AABB aabb) : base(world, aabb)
        {
            rect.FillColor = Color.Red;
            _inventory = new UIInventory(new Vector2f(800, 100));

            UIManager.AddWindow(_inventory);

            _inventory.AddItem(Items.GetItem(ItemList.WoodPickaxe));
            _inventory.AddItem(Items.GetItem(ItemList.WoodAxe));
            _inventory.AddItem(Items.GetItem(ItemList.WoodShovel));
            _inventory.AddItem(Items.GetItem(ItemList.Chest), 64);
            _inventory.AddItem(Items.GetItem(ItemList.BoardWall), 64);
            _inventory.AddItem(Items.GetItem(ItemList.Board), 64);
        }

        public override void Update(float deltaTime)
        {
            if (!_inventory.IsFullInventoryVisible)
                MouseUpdate();
            _inventory.Position = new Vector2f(-480, Game.GetWindowSize().Y - 250) / 2 + Position;

            for (int i = 0; i < 9; i++)
            {
                if (Keyboard.IsKeyPressed((Keyboard.Key)i + 27))
                {
                    _inventory.SetSelectedCell(i);
                }
            }

            if(Keyboard.IsKeyPressed(Keyboard.Key.Num0))
                _inventory.SetSelectedCell(9);

            if(Keyboard.IsKeyPressed(Keyboard.Key.E))
            {
                if (_inventory.IsFullInventoryVisible)
                    _inventory.HideInventory();
                else 
                    _inventory.ShowInventory();
            }

            _selectedItem = _inventory.GetItemWithSelectedCell();
            if (!_inventory.IsFullInventoryVisible)
            {
                bool isRightMove = Keyboard.IsKeyPressed(Keyboard.Key.D);
                bool isLeftMove = Keyboard.IsKeyPressed(Keyboard.Key.A);
                bool isJump = Keyboard.IsKeyPressed(Keyboard.Key.Space);
                bool isRunning = Keyboard.IsKeyPressed(Keyboard.Key.LShift) || Keyboard.IsKeyPressed(Keyboard.Key.RShift);

                bool isMove = isLeftMove || isRightMove;

                if (isRunning && isMove && !isFall)
                {
                    maxSpeed = 250;
                }
                else
                {
                    maxSpeed = 150;
                }

                if (isMove)
                {
                    if (isRightMove && velocity.X < maxSpeed)
                    {
                        velocity += new Vector2f(5, 0);
                    }
                    else if (isLeftMove && velocity.X > -maxSpeed)
                    {
                        velocity += new Vector2f(-5, 0);
                    }
                }
                else
                {
                    velocity = new Vector2f(0, velocity.Y);
                }

                if (isJump && !isFall && !isJumped)
                {
                    velocity += new Vector2f(0, -200);
                    isFall = true;
                    isJumped = true;
                }
                else if (!isJump)
                    isJumped = false;

                if (Keyboard.IsKeyPressed(Keyboard.Key.F))
                {
                    _inventory.CraftItem(ItemList.Board);
                }
                if (Keyboard.IsKeyPressed(Keyboard.Key.C))
                {
                    _inventory.CraftItem(ItemList.Stick);
                }
            }
        }

        float breakingSpeed = 1f;

        private void MouseUpdate()
        {
            Vector2i mousePos = (Vector2i)Game.GetMousePositionByWorld();

            var chunk = world.GetChunkByWorldPosition((Vector2f)mousePos);

            bool mouseHoveredPlayer = !new FloatRect(Position, rect.Size).Contains(mousePos.X, mousePos.Y);
            bool distanceToTile = true;// MathHelper.DistanceSquared((Vector2f)mousePos, Position) < 25000;
            bool mouseLeftClick = Mouse.IsButtonPressed(Mouse.Button.Left);
            bool mouseRightClick = Mouse.IsButtonPressed(Mouse.Button.Right);

            if (chunk != null)
            {
                if (mouseHoveredPlayer)
                {
                    if (distanceToTile)
                    {
                        if (_selectedItem != null)
                            breakingSpeed = _selectedItem.Speed;
                        else
                            breakingSpeed = 1f;

                        var tile = chunk.GetTileByWorldPosition((Vector2f)mousePos);
                        if (tile != null && mouseLeftClick && _selectedItem != null && _selectedItem.Type == ItemType.Tool)
                        {
                            chunk.BreakingTailByWorldPosition((Vector2f)mousePos, 0.01f * breakingSpeed);
                        }
                        else if (tile != null && mouseLeftClick && _selectedItem != null && _selectedItem.Type == ItemType.Tile)
                        {
                            chunk.BreakingTailByWorldPosition((Vector2f)mousePos, 0.01f);
                        }
                        else if (tile == null && mouseLeftClick && _selectedItem != null && _selectedItem.Type == ItemType.Tile)
                        {
                            var itemTile = _selectedItem as ItemTile;


                            if (chunk.SetTileByWorldPosition((Vector2f)mousePos, itemTile!.TileType, itemTile!.IsWall))
                                _inventory.GetSelectedCell()!.ItemStack!.ItemCount -= 1;
                        }
                        else if (mouseRightClick)
                        {
                            var tileEntity = chunk.GetTileEntityByWorldPosition((Vector2f)mousePos);
                            if (tileEntity != null)
                            {
                                tileEntity.Use();
                            }
                        }
                    }
                }
            }
        }

        public override void OnCollided(Entity other, Vector2f normal, float depth)
        {
            base.OnCollided(other, normal, depth);

            if (other == null)
                return;

            if (other.Layer is CollisionLayer.Item)
            {
                var item = other as DropItem;
                if (item != null)
                {
                    if (_inventory.AddItem(item.Item))
                    {
                        world.RemoveItem(item);
                    }
                }
            }
        }

        public override void Draw(RenderTarget target, RenderStates states)
        {
            base.Draw(target, states);
        }
    }
}
