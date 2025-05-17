using SFML.Graphics;
using SFML.System;
using SFML.Window;
using VoxelGame.Graphics;
using VoxelGame.Item;
using VoxelGame.Meths;
using VoxelGame.Physics;
using VoxelGame.Resources;
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
        private float _playerAnimationSpeed = 0.1f;

        // Спрайты с анимацией
        private AnimSprite asHair;         // Волосы
        private AnimSprite asHead;         // Голова
        private AnimSprite asShirt;        // Рубашка
        private AnimSprite asUndershirt;   // Рукава
        private AnimSprite asHands;        // Кисти
        private AnimSprite asPants;         // Ноги
        private AnimSprite asShoes;        // Обувь

        private float breakingSpeed = 1f;

        public Color HairColor = new Color(0, 240, 10);  // Цвет волос
        public Color BodyColor = new Color(255, 229, 186);  // Цвет кожи
        public Color ShirtColor = new Color(255, 255, 0);  // Цвет куртки
        public Color PantsColor = new Color(0, 76, 135);  // Цвет штанов

        public List<CraftTool> CraftTools { get; set; }

        public Player(World world, AABB aabb) : base(world, aabb)
        {
            _inventory = new UIInventory(new Vector2f(UIInventoryCell.CellSize * 10, UIInventoryCell.CellSize));
            _inventory.Player = this;

            UIManager.AddWindow(_inventory);

            _inventory.AddItem(Items.GetItem(ItemList.CopperPickaxe), 1);
            _inventory.AddItem(Items.GetItem(ItemList.CopperAxe), 1);
            _inventory.AddItem(Items.GetItem(ItemList.Stove), 1);
            _inventory.AddItem(Items.GetItem(ItemList.OakBoard), 64);
            _inventory.AddItem(Items.GetItem(ItemList.OakBoardWall), 64);
            _inventory.AddItem(Items.GetItem(ItemList.Torch), 64);
            _inventory.AddItem(Items.GetItem(ItemList.Door), 2);

            CraftTools = new List<CraftTool>();

            Origin = new Vector2f(-rect.Size.X / 2, 0);
            Visible = false;

            CreateAnimation();
        }

        public void CreateAnimation()
        {
            var ssHair = AssetManager.GetSpriteSheet("Player_Hair_1", 1, 14, true, 0);
            var ssHead = AssetManager.GetSpriteSheet("Player_Head", 1, 20, true, 0);
            var ssShirt = AssetManager.GetSpriteSheet("Player_Shirt", 1, 20, true, 0);
            var ssUndershirt = AssetManager.GetSpriteSheet("Player_Undershirt", 1, 20, true, 0);
            var ssHands = AssetManager.GetSpriteSheet("Player_Hands", 1, 20, true, 0);
            var ssPants = AssetManager.GetSpriteSheet("Player_Pants", 1, 20, true, 0);
            var ssShoes = AssetManager.GetSpriteSheet("Player_Shoes", 1, 20, true, 0);

            // Волосы
            asHair = new AnimSprite(ssHair);
            asHair.Position = new Vector2f(0, 19);
            asHair.Color = HairColor;
            asHair.AddAnimation("idle", new Animation(
                new AnimationFrame(0, 0, 0.1f)));
            asHair.AddAnimation("tool", new Animation(
                new AnimationFrame(0, 0, 1f)));
            asHair.AddAnimation("jump", new Animation(
                new AnimationFrame(0, 5, 0.1f)));
            asHair.AddAnimation("run", new Animation(
                new AnimationFrame(0, 0, 0.1f),
                new AnimationFrame(0, 1, 0.1f),
                new AnimationFrame(0, 2, 0.1f),
                new AnimationFrame(0, 3, 0.1f),
                new AnimationFrame(0, 4, 0.1f),
                new AnimationFrame(0, 5, 0.1f),
                new AnimationFrame(0, 6, 0.1f),
                new AnimationFrame(0, 7, 0.1f),
                new AnimationFrame(0, 8, 0.1f),
                new AnimationFrame(0, 9, 0.1f),
                new AnimationFrame(0, 10, 0.1f),
                new AnimationFrame(0, 11, 0.1f),
                new AnimationFrame(0, 12, 0.1f),
                new AnimationFrame(0, 13, 0.1f)
            ));

            // Голова
            asHead = new AnimSprite(ssHead);
            asHead.Position = new Vector2f(0, 19);
            asHead.Color = BodyColor;
            asHead.AddAnimation("idle", new Animation(
                new AnimationFrame(0, 0, 0.1f)));
            asHead.AddAnimation("tool", new Animation(
                new AnimationFrame(0, 0, 1f)));
            asHead.AddAnimation("jump", new Animation(
                new AnimationFrame(0, 5, 0.1f)));
            asHead.AddAnimation("run", new Animation(
                new AnimationFrame(0, 6, 0.1f),
                new AnimationFrame(0, 7, 0.1f),
                new AnimationFrame(0, 8, 0.1f),
                new AnimationFrame(0, 9, 0.1f),
                new AnimationFrame(0, 10, 0.1f),
                new AnimationFrame(0, 11, 0.1f),
                new AnimationFrame(0, 12, 0.1f),
                new AnimationFrame(0, 13, 0.1f),
                new AnimationFrame(0, 14, 0.1f),
                new AnimationFrame(0, 15, 0.1f),
                new AnimationFrame(0, 16, 0.1f),
                new AnimationFrame(0, 17, 0.1f),
                new AnimationFrame(0, 18, 0.1f),
                new AnimationFrame(0, 19, 0.1f)
            ));

            // Рубашка
            asShirt = new AnimSprite(ssShirt);
            asShirt.Position = new Vector2f(0, 19);
            asShirt.Color = ShirtColor;
            asShirt.AddAnimation("idle", new Animation(
                new AnimationFrame(0, 0, 0.1f)));
            asShirt.AddAnimation("tool", new Animation(
                new AnimationFrame(0, 0, 0.5f),
                new AnimationFrame(0, 1, 0.5f),
                new AnimationFrame(0, 2, 0.5f),
                new AnimationFrame(0, 3, 0.5f),
                new AnimationFrame(0, 4, 0.5f)));
            asShirt.AddAnimation("jump", new Animation(
                new AnimationFrame(0, 5, 0.1f)));
            asShirt.AddAnimation("run", new Animation(
                new AnimationFrame(0, 6, 0.1f),
                new AnimationFrame(0, 7, 0.1f),
                new AnimationFrame(0, 8, 0.1f),
                new AnimationFrame(0, 9, 0.1f),
                new AnimationFrame(0, 10, 0.1f),
                new AnimationFrame(0, 11, 0.1f),
                new AnimationFrame(0, 12, 0.1f),
                new AnimationFrame(0, 13, 0.1f),
                new AnimationFrame(0, 14, 0.1f),
                new AnimationFrame(0, 15, 0.1f),
                new AnimationFrame(0, 16, 0.1f),
                new AnimationFrame(0, 17, 0.1f),
                new AnimationFrame(0, 18, 0.1f),
                new AnimationFrame(0, 19, 0.1f)
            ));

            // Рукава
            asUndershirt = new AnimSprite(ssUndershirt);
            asUndershirt.Position = new Vector2f(0, 19);
            asUndershirt.AddAnimation("idle", new Animation(
                new AnimationFrame(0, 0, 1f)));
            asUndershirt.AddAnimation("tool", new Animation(
                new AnimationFrame(0, 0, 0.5f),
                new AnimationFrame(0, 1, 0.5f),
                new AnimationFrame(0, 2, 0.5f),
                new AnimationFrame(0, 3, 0.5f),
                new AnimationFrame(0, 4, 0.5f)));
            asUndershirt.AddAnimation("jump", new Animation(
                new AnimationFrame(0, 5, 0.1f)));
            asUndershirt.AddAnimation("run", new Animation(
                new AnimationFrame(0, 6, 0.1f),
                new AnimationFrame(0, 7, 0.1f),
                new AnimationFrame(0, 8, 0.1f),
                new AnimationFrame(0, 9, 0.1f),
                new AnimationFrame(0, 10, 0.1f),
                new AnimationFrame(0, 11, 0.1f),
                new AnimationFrame(0, 12, 0.1f),
                new AnimationFrame(0, 13, 0.1f),
                new AnimationFrame(0, 14, 0.1f),
                new AnimationFrame(0, 15, 0.1f),
                new AnimationFrame(0, 16, 0.1f),
                new AnimationFrame(0, 17, 0.1f),
                new AnimationFrame(0, 18, 0.1f),
                new AnimationFrame(0, 19, 0.1f)
            ));

            // Кисти
            asHands = new AnimSprite(ssHands);
            asHands.Position = new Vector2f(0, 19);
            asHands.Color = BodyColor;
            asHands.AddAnimation("idle", new Animation(
                new AnimationFrame(0, 0, 0.1f)));
            asHands.AddAnimation("tool", new Animation(
                new AnimationFrame(0, 0, 0.5f),
                new AnimationFrame(0, 1, 0.5f),
                new AnimationFrame(0, 2, 0.5f),
                new AnimationFrame(0, 3, 0.5f),
                new AnimationFrame(0, 4, 0.5f)));
            asHands.AddAnimation("jump", new Animation(
                new AnimationFrame(0, 5, 0.1f)));
            asHands.AddAnimation("run", new Animation(
                new AnimationFrame(0, 6, 0.1f),
                new AnimationFrame(0, 7, 0.1f),
                new AnimationFrame(0, 8, 0.1f),
                new AnimationFrame(0, 9, 0.1f),
                new AnimationFrame(0, 10, 0.1f),
                new AnimationFrame(0, 11, 0.1f),
                new AnimationFrame(0, 12, 0.1f),
                new AnimationFrame(0, 13, 0.1f),
                new AnimationFrame(0, 14, 0.1f),
                new AnimationFrame(0, 15, 0.1f),
                new AnimationFrame(0, 16, 0.1f),
                new AnimationFrame(0, 17, 0.1f),
                new AnimationFrame(0, 18, 0.1f),
                new AnimationFrame(0, 19, 0.1f)
            ));

            // Ноги
            asPants = new AnimSprite(ssPants);
            asPants.Color = PantsColor;
            asPants.Position = new Vector2f(0, 19);
            asPants.AddAnimation("idle", new Animation(
                new AnimationFrame(0, 0, 0.1f)));
            asPants.AddAnimation("tool", new Animation(
                new AnimationFrame(0, 0, 0.1f)));
            asPants.AddAnimation("jump", new Animation(
                new AnimationFrame(0, 5, 0.1f)));
            asPants.AddAnimation("run", new Animation(
                new AnimationFrame(0, 6, 0.1f),
                new AnimationFrame(0, 7, 0.1f),
                new AnimationFrame(0, 8, 0.1f),
                new AnimationFrame(0, 9, 0.1f),
                new AnimationFrame(0, 10, 0.1f),
                new AnimationFrame(0, 11, 0.1f),
                new AnimationFrame(0, 12, 0.1f),
                new AnimationFrame(0, 13, 0.1f),
                new AnimationFrame(0, 14, 0.1f),
                new AnimationFrame(0, 15, 0.1f),
                new AnimationFrame(0, 16, 0.1f),
                new AnimationFrame(0, 17, 0.1f),
                new AnimationFrame(0, 18, 0.1f),
                new AnimationFrame(0, 19, 0.1f)
            ));

            // Обувь
            asShoes = new AnimSprite(ssShoes);
            asShoes.Position = new Vector2f(0, 19);
            asShoes.AddAnimation("idle", new Animation(
                new AnimationFrame(0, 0, 1f)));
            asShoes.AddAnimation("tool", new Animation(
                new AnimationFrame(0, 0, 1f)));
            asShoes.AddAnimation("jump", new Animation(
                new AnimationFrame(0, 5, 1f)));
            asShoes.AddAnimation("run", new Animation(
                new AnimationFrame(0, 6, 0.1f),
                new AnimationFrame(0, 7, 0.1f),
                new AnimationFrame(0, 8, 0.1f),
                new AnimationFrame(0, 9, 0.1f),
                new AnimationFrame(0, 10, 0.1f),
                new AnimationFrame(0, 11, 0.1f),
                new AnimationFrame(0, 12, 0.1f),
                new AnimationFrame(0, 13, 0.1f),
                new AnimationFrame(0, 14, 0.1f),
                new AnimationFrame(0, 15, 0.1f),
                new AnimationFrame(0, 16, 0.1f),
                new AnimationFrame(0, 17, 0.1f),
                new AnimationFrame(0, 18, 0.1f),
                new AnimationFrame(0, 19, 0.1f)
            ));
        }

        public override void Update(float deltaTime)
        {
            asUndershirt.Color = Color;
            asShoes.Color = Color;

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

                        Scale = new Vector2f(1, 1);
                        Origin = new Vector2f(-rect.Size.X / 2, 0);
                    }
                    else if (isLeftMove && velocity.X > -maxSpeed)
                    {
                        velocity += new Vector2f(-5, 0);

                        Scale = new Vector2f(-1, 1);   
                        Origin = new Vector2f(rect.Size.X / 2, 0);
                    }

                    asHead.Play("run");
                    asHair.Play("run");
                    asShirt.Play("run");
                    asUndershirt.Play("run");
                    asHands.Play("run");
                    asPants.Play("run");
                    asShoes.Play("run");
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

                    asHead.Play("jump");
                    asHair.Play("jump");
                    asShirt.Play("jump");
                    asUndershirt.Play("jump");
                    asHands.Play("jump");
                    asPants.Play("jump");
                    asShoes.Play("jump");
                }
                else if (!isJump)
                    isJumped = false;

                if(!isMove && !isJump)
                {
                    asHead.Play("idle");
                    asHair.Play("idle");
                    asShirt.Play("idle");
                    asUndershirt.Play("idle");
                    asHands.Play("idle");
                    asPants.Play("idle");
                    asShoes.Play("idle");
                }
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

                        if (tile != null)
                        {
                            DebugRender.AddText(mousePos, $"TileType: {tile.Type} \n" +
                                                          $"Tile Strength: {tile.Strength} \n " +
                                                          $"Tile perent: {tile.PerentTile}");
                        }
                        else if (wall != null)
                        {
                            DebugRender.AddText(mousePos, $"WallType: {wall.Type} \n" +
                                                          $"Wall Strength: {wall.Strength}");
                        }

                        if (mouseRightClick)
                        {
                            if (tile == null && _selectedItem != null && _selectedItem.Type == ItemType.Tile)
                            {
                                if (chunk.SetTileByWorldPosition(mousePos, Tiles.ItemListToTileType(_selectedItem.ItemList), isPlayerSet: true))
                                {
                                    _inventory.GetSelectedCell()!.ItemStack!.ItemCount -= 1;
                                }
                            }
                            else if (wall == null && _selectedItem != null && _selectedItem.Type == ItemType.Wall)
                            {
                                if (chunk.SetWallByWorldPosition(mousePos, Tiles.ItemListToWallType(_selectedItem.ItemList), isPlayerSet: true))
                                {
                                    _inventory.GetSelectedCell()!.ItemStack!.ItemCount -= 1;
                                }
                            }
                        }
                        else if (mouseLeftClick)
                        {
                            if (tile != null && itemIsTool)
                            {
                                var tool = _selectedItem as ItemTool;

                                chunk.BreakingTileByWorldPosition(mousePos, tool!.Type, tool.Power, tool.BreakingSpeed * 0.05f);
                            }
                            else if(tile != null && !itemIsTool)
                            {
                                chunk.BreakingTileByWorldPosition(mousePos, ItemType.None, 0, 0.1f);
                            }
                            else if (wall != null && itemIsTool)
                            {
                                var tool = _selectedItem as ItemTool;

                                chunk.BreakingWallByWorldPosition(mousePos, tool!.Type, tool.Power, tool.BreakingSpeed * 0.05f);
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

            states.Transform *= Transform;

            target.Draw(asHead, states);
            target.Draw(asHair, states);
            target.Draw(asShirt, states);
            target.Draw(asUndershirt, states);
            target.Draw(asHands, states);
            target.Draw(asPants, states);
            target.Draw(asShoes, states);
        }
    }
}
