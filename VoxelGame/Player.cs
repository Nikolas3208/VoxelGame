using SFML.Graphics;
using SFML.System;
using SFML.Window;
using VoxelGame.Item;
using VoxelGame.Meths;
using VoxelGame.Physics;
using VoxelGame.UI;
using VoxelGame.UI.Inventory;
using VoxelGame.Worlds;
using VoxelGame.Worlds.Tile;

namespace VoxelGame
{
    public class Player : Entity
    {
        private float maxSpeed = 100;
        private bool isJumped = false;
        private bool _inventoryOpened = false;
        private UIInventory _inventory;
        private Item.Item? _selectedItem;

        private float breakingSpeed = 1f;

        public List<CraftTool> CraftTools { get; set; }

        public Player(World world, AABB aabb) : base(world, aabb)
        {
            rect.FillColor = Color.Red;

            _inventory = new UIInventory(new Vector2f(UIInventoryCell.CellSize * 10, UIInventoryCell.CellSize));
            _inventory.Player = this;

            UIManager.AddWindow(_inventory);

            _inventory.AddItem(Items.GetItem(ItemList.CopperPickaxe), 1);
            _inventory.AddItem(Items.GetItem(ItemList.CopperAxe), 1);

            CraftTools = new List<CraftTool>();
        }

        public override void Update(float deltaTime)
        {
            MouseUpdate();
            _inventory.Position = new Vector2f(-Game.GetWindowSize().X / 2 * Game.GetZoom(), Game.GetWindowSize().Y - 250) / 4 + Position;

            var chunk = world.GetChunkByWorldPosition(Position);

            CraftTools.Clear();
            CraftTools.Add(CraftTool.None);

            if (chunk != null)
            {
                for (int i = 0; i < Enum.GetNames<CraftTool>().Length - 1; i++)
                {
                    var tileTypeStr = (Enum.GetNames<CraftTool>()[i]).ToString();

                    var tileType = Enum.Parse<TileType>(tileTypeStr);
                    var tile = chunk.GetTileByType(tileType);

                    if (tile != null && MathHelper.Distance(Position, tile.GlobalPosition) < 3 * 16)
                    {
                        CraftTools.Add(Enum.Parse<CraftTool>(tileTypeStr));
                    }
                }
            }

            for (int i = 0; i < 9; i++)
            {
                if (Keyboard.IsKeyPressed((Keyboard.Key)i + 27))
                {
                    _inventory.SetSelectedCell(i);
                }
            }

            if (Keyboard.IsKeyPressed(Keyboard.Key.Num0))
                _inventory.SetSelectedCell(9);

            if (Keyboard.IsKeyPressed(Keyboard.Key.E) && !_inventoryOpened)
            {
                if (_inventory.IsFullInventoryVisible)
                    _inventory.HideInventory();
                else
                    _inventory.ShowInventory();

                _inventoryOpened = true;
            }
            else if (!Keyboard.IsKeyPressed(Keyboard.Key.E))
            {
                _inventoryOpened = false;
            }

            _selectedItem = _inventory.GetItemWithSelectedCell();
            {
                bool isRightMove = Keyboard.IsKeyPressed(Keyboard.Key.D);
                bool isLeftMove = Keyboard.IsKeyPressed(Keyboard.Key.A);
                bool isJump = Keyboard.IsKeyPressed(Keyboard.Key.Space);
                bool isRunning = Keyboard.IsKeyPressed(Keyboard.Key.LShift) || Keyboard.IsKeyPressed(Keyboard.Key.RShift);

                bool isMove = isLeftMove || isRightMove;

                if (isRunning && isMove && !isFall)
                {
                    maxSpeed = 200;
                }
                else
                {
                    maxSpeed = 100;
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
            }
        }


        private void MouseUpdate()
        {
            Vector2f mousePos = Game.GetMousePositionByWorld();

            var chunk = world.GetChunkByWorldPosition(mousePos);

            bool mouseHoveredPlayer = new FloatRect(Position, rect.Size).Contains(mousePos.X, mousePos.Y);
            bool distanceToTile = MathHelper.DistanceSquared(mousePos, Position) < 25000;
            bool mouseLeftClick = Mouse.IsButtonPressed(Mouse.Button.Left);
            bool mouseRightClick = Mouse.IsButtonPressed(Mouse.Button.Right);
            bool itemIsTool = _selectedItem != null && (_selectedItem.Type == ItemType.Axe || _selectedItem.Type == ItemType.Hammer || _selectedItem.Type == ItemType.Pickaxe);
            bool itemIsPickaxe = _selectedItem != null && _selectedItem.Type == ItemType.Pickaxe;
            bool itemIsAxe = _selectedItem != null && _selectedItem.Type == ItemType.Axe;

            breakingSpeed = _selectedItem != null && _selectedItem is ItemTool ? (_selectedItem as ItemTool)!.BreakingSpeed : 1f;

            if (chunk != null)
            {
                if (!mouseHoveredPlayer)
                {
                    if (distanceToTile)
                    {
                        var tile = chunk.GetTileByWorldPosition(mousePos);
                        var wall = chunk.GetWallByWorldPosition(mousePos);

                        if(tile != null)
                        {
                            DebugRender.AddText(mousePos, $"TileType: {tile.Type} \n" +
                                                          $"Tile lockalPosition: {tile.LocalPosition}");
                        }

                        if (mouseRightClick)
                        {
                            if (tile == null && _selectedItem != null && _selectedItem.Type == ItemType.Tile)
                            {
                                if (chunk.SetTileByWorldPosition(mousePos, Tiles.ItemListToTileType(_selectedItem.ItemList)))
                                {
                                    _inventory.GetSelectedCell()!.ItemStack!.ItemCount -= 1;
                                }
                            }
                            else if(wall == null && _selectedItem != null && _selectedItem.Type == ItemType.Wall)
                            {
                                if (chunk.SetWallByWorldPosition(mousePos, Tiles.ItemListToWallType(_selectedItem.ItemList)))
                                {
                                    _inventory.GetSelectedCell()!.ItemStack!.ItemCount -= 1;
                                }
                            }
                        }
                        else if(mouseLeftClick)
                        {
                            if(tile != null && itemIsTool)
                            {
                                var tool = _selectedItem as ItemTool;

                                chunk.BreakingTileByWorldPosition(mousePos, tool!.Type, tool.Power, tool.BreakingSpeed * 0.05f);
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
