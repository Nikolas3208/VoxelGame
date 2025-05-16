using SFML.Graphics;
using SFML.System;
using SFML.Window;
using VoxelGame.Graphics.Animation;
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

        private Animator _animatorHair;
        private Animator _animatorHead;
        private Animator _animatorShirt;
        private Animator _animatorUndershirt;
        private Animator _animatorHands;
        private Animator _animatorPants;
        private Animator _animatorShoes;

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

            _animatorHair = new Animator();
            _animatorHead = new Animator();
            _animatorShirt = new Animator();
            _animatorUndershirt = new Animator();
            _animatorHands = new Animator();
            _animatorPants = new Animator();
            _animatorShoes = new Animator();

            UIManager.AddWindow(_inventory);

            _inventory.AddItem(Items.GetItem(ItemList.CopperPickaxe), 1);
            _inventory.AddItem(Items.GetItem(ItemList.CopperAxe), 1);
            _inventory.AddItem(Items.GetItem(ItemList.Stove), 1);
            _inventory.AddItem(Items.GetItem(ItemList.Torch), 64);

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

            //Стоит волосы
            Animation idleHair = new Animation("idleHeir") { Position = new Vector2f(0, 22) };
            idleHair.SetAnimationFrames(new AnimationFrame(0, _playerAnimationSpeed));
            idleHair.SetAnimSprite(new AnimSprite(ssHair) { Color = HairColor });

            //Инстумент волосы
            Animation toolHair = new Animation("toolHeir") { Position = new Vector2f(0, 22) };
            toolHair.SetAnimationFrames(new AnimationFrame(0, _playerAnimationSpeed));
            toolHair.SetAnimSprite(new AnimSprite(ssHair) { Color = HairColor });

            //Прыжок волосы
            Animation jumHair = new Animation("jumHands") { Position = new Vector2f(0, 22) };
            jumHair.SetAnimationFrames(new AnimationFrame(0, _playerAnimationSpeed));
            jumHair.SetAnimSprite(new AnimSprite(ssHair) { Color = HairColor });

            //Бег волосы
            Animation runHair = new Animation("runHeir") { Position = new Vector2f(0, 22) };
            runHair.SetAnimationFrames(
                new AnimationFrame(0, _playerAnimationSpeed),
                new AnimationFrame(1, _playerAnimationSpeed),
                new AnimationFrame(2, _playerAnimationSpeed),
                new AnimationFrame(3, _playerAnimationSpeed),
                new AnimationFrame(4, _playerAnimationSpeed),
                new AnimationFrame(5, _playerAnimationSpeed),
                new AnimationFrame(6, _playerAnimationSpeed),
                new AnimationFrame(7, _playerAnimationSpeed),
                new AnimationFrame(8, _playerAnimationSpeed),
                new AnimationFrame(9, _playerAnimationSpeed),
                new AnimationFrame(10, _playerAnimationSpeed),
                new AnimationFrame(11, _playerAnimationSpeed),
                new AnimationFrame(12, _playerAnimationSpeed),
                new AnimationFrame(13, _playerAnimationSpeed));
            runHair.SetAnimSprite(new AnimSprite(ssHair) { Color = HairColor });

            _animatorHair.AddAnimation("Idle", idleHair);
            _animatorHair.AddAnimation("Tool", toolHair);
            _animatorHair.AddAnimation("Run", runHair);
            _animatorHair.AddAnimation("Jump", jumHair);

            //Стоит голова
            Animation idleHead = new Animation("idleHead") { Position = new Vector2f(0, 22) };
            idleHead.SetAnimationFrames(new AnimationFrame(0, _playerAnimationSpeed));
            idleHead.SetAnimSprite(new AnimSprite(ssHead) { Color = BodyColor });

            //Инстумент голова
            Animation toolHead = new Animation("toolHead") { Position = new Vector2f(0, 22) };
            toolHead.SetAnimationFrames(new AnimationFrame(0, _playerAnimationSpeed));
            toolHead.SetAnimSprite(new AnimSprite(ssHead) { Color = BodyColor });

            //Прыжок голова
            Animation jumHead = new Animation("jumHead") { Position = new Vector2f(0, 22) };
            jumHead.SetAnimationFrames(new AnimationFrame(5, _playerAnimationSpeed));
            jumHead.SetAnimSprite(new AnimSprite(ssHead) { Color = BodyColor });

            //Бег голова
            Animation runHead = new Animation("runHead") { Position = new Vector2f(0, 22) };
            runHead.SetAnimationFrames(
                new AnimationFrame(6, _playerAnimationSpeed),
                new AnimationFrame(7, _playerAnimationSpeed),
                new AnimationFrame(8, _playerAnimationSpeed),
                new AnimationFrame(9, _playerAnimationSpeed),
                new AnimationFrame(10, _playerAnimationSpeed),
                new AnimationFrame(11, _playerAnimationSpeed),
                new AnimationFrame(12, _playerAnimationSpeed),
                new AnimationFrame(13, _playerAnimationSpeed),
                new AnimationFrame(14, _playerAnimationSpeed),
                new AnimationFrame(15, _playerAnimationSpeed),
                new AnimationFrame(16, _playerAnimationSpeed),
                new AnimationFrame(17, _playerAnimationSpeed),
                new AnimationFrame(18, _playerAnimationSpeed),
                new AnimationFrame(19, _playerAnimationSpeed));
            runHead.SetAnimSprite(new AnimSprite(ssHead) { Color = BodyColor });

            _animatorHead.AddAnimation("Idle", idleHead);
            _animatorHead.AddAnimation("Tool", toolHead);
            _animatorHead.AddAnimation("Run", runHead);
            _animatorHead.AddAnimation("Jump", jumHead);

            //Стоит куртка
            Animation idleShirt = new Animation("idleShirt") { Position = new Vector2f(0, 22) };
            idleShirt.SetAnimationFrames(new AnimationFrame(0, _playerAnimationSpeed));
            idleShirt.SetAnimSprite(new AnimSprite(ssShirt) { Color = ShirtColor });

            //Инстумент куртка
            Animation toolShirt = new Animation("toolShirt") { Position = new Vector2f(0, 22) };
            toolShirt.SetAnimationFrames(
                new AnimationFrame(0, _playerAnimationSpeed),
                new AnimationFrame(1, _playerAnimationSpeed),
                new AnimationFrame(2, _playerAnimationSpeed),
                new AnimationFrame(3, _playerAnimationSpeed),
                new AnimationFrame(4, _playerAnimationSpeed));
            toolShirt.SetAnimSprite(new AnimSprite(ssShirt) { Color = ShirtColor });

            //Прыжок куртка
            Animation jumpShirt = new Animation("jumpShirt") { Position = new Vector2f(0, 22) };
            jumpShirt.SetAnimationFrames(new AnimationFrame(5, _playerAnimationSpeed));
            jumpShirt.SetAnimSprite(new AnimSprite(ssShirt) { Color = ShirtColor });

            //Бег куртка
            Animation runShirt = new Animation("runShirt") { Position = new Vector2f(0, 22) };
            runShirt.SetAnimationFrames(
                new AnimationFrame(6, _playerAnimationSpeed),
                new AnimationFrame(7, _playerAnimationSpeed),
                new AnimationFrame(8, _playerAnimationSpeed),
                new AnimationFrame(9, _playerAnimationSpeed),
                new AnimationFrame(10, _playerAnimationSpeed),
                new AnimationFrame(11, _playerAnimationSpeed),
                new AnimationFrame(12, _playerAnimationSpeed),
                new AnimationFrame(13, _playerAnimationSpeed),
                new AnimationFrame(14, _playerAnimationSpeed),
                new AnimationFrame(15, _playerAnimationSpeed),
                new AnimationFrame(16, _playerAnimationSpeed),
                new AnimationFrame(17, _playerAnimationSpeed),
                new AnimationFrame(18, _playerAnimationSpeed),
                new AnimationFrame(19, _playerAnimationSpeed));
            runShirt.SetAnimSprite(new AnimSprite(ssShirt) { Color = ShirtColor });

            _animatorShirt.AddAnimation("Idle", idleShirt);
            _animatorShirt.AddAnimation("Tool", toolShirt);
            _animatorShirt.AddAnimation("Run", runShirt);
            _animatorShirt.AddAnimation("Jump", jumpShirt);

            //Стоит рукав
            Animation idleUndershirt = new Animation("idleUndershirt") { Position = new Vector2f(0, 22) };
            idleUndershirt.SetAnimationFrames(new AnimationFrame(0, _playerAnimationSpeed));
            idleUndershirt.SetAnimSprite(new AnimSprite(ssUndershirt));

            //Инстумент куртка
            Animation toolUndershirt = new Animation("toolUndershirt") { Position = new Vector2f(0, 22) };
            toolUndershirt.SetAnimationFrames(
                new AnimationFrame(0, _playerAnimationSpeed),
                new AnimationFrame(1, _playerAnimationSpeed),
                new AnimationFrame(2, _playerAnimationSpeed),
                new AnimationFrame(3, _playerAnimationSpeed),
                new AnimationFrame(4, _playerAnimationSpeed));
            toolUndershirt.SetAnimSprite(new AnimSprite(ssUndershirt));

            //Прыжок куртка
            Animation jumpUndershirt = new Animation("jumpUndershirt") { Position = new Vector2f(0, 22) };
            jumpUndershirt.SetAnimationFrames(new AnimationFrame(5, _playerAnimationSpeed));
            jumpUndershirt.SetAnimSprite(new AnimSprite(ssUndershirt));

            //Бег рукава
            Animation runUndershirt = new Animation("runUndershirt") { Position = new Vector2f(0, 22) };
            runUndershirt.SetAnimationFrames(
                new AnimationFrame(6,_playerAnimationSpeed),
                new AnimationFrame(7,_playerAnimationSpeed),
                new AnimationFrame(8,_playerAnimationSpeed),
                new AnimationFrame(9, _playerAnimationSpeed),
                new AnimationFrame(10,_playerAnimationSpeed),
                new AnimationFrame(11,_playerAnimationSpeed),
                new AnimationFrame(12,_playerAnimationSpeed),
                new AnimationFrame(13,_playerAnimationSpeed),
                new AnimationFrame(14,_playerAnimationSpeed),
                new AnimationFrame(15,_playerAnimationSpeed),
                new AnimationFrame(16,_playerAnimationSpeed),
                new AnimationFrame(17,_playerAnimationSpeed),
                new AnimationFrame(18,_playerAnimationSpeed),
                new AnimationFrame(19, _playerAnimationSpeed));
            runUndershirt.SetAnimSprite(new AnimSprite(ssUndershirt));

            _animatorUndershirt.AddAnimation("Idle", idleUndershirt);
            _animatorUndershirt.AddAnimation("Tool", toolUndershirt);
            _animatorUndershirt.AddAnimation("Run", runUndershirt);
            _animatorUndershirt.AddAnimation("Jump", jumpUndershirt);

            //Стоит руки
            Animation idleHands = new Animation("idleHands") { Position = new Vector2f(0, 22) };
            idleHands.SetAnimationFrames(new AnimationFrame(0, _playerAnimationSpeed));
            idleHands.SetAnimSprite(new AnimSprite(ssHands) { Color = BodyColor });

            //Инстумент руки
            Animation toolHands = new Animation("toolHands") { Position = new Vector2f(0, 22) };
            toolHands.SetAnimationFrames(
                new AnimationFrame(0, _playerAnimationSpeed),
                new AnimationFrame(1, _playerAnimationSpeed),
                new AnimationFrame(2, _playerAnimationSpeed),
                new AnimationFrame(3, _playerAnimationSpeed),
                new AnimationFrame(4, _playerAnimationSpeed));
            toolHands.SetAnimSprite(new AnimSprite(ssHands) { Color = BodyColor });

            //Прыжок руки
            Animation jumpHands = new Animation("jumpHands") { Position = new Vector2f(0, 22) };
            jumpHands.SetAnimationFrames(new AnimationFrame(5, _playerAnimationSpeed));
            jumpHands.SetAnimSprite(new AnimSprite(ssHands) { Color = BodyColor });

            //Бег руки
            Animation runHands = new Animation("runHands") { Position = new Vector2f(0, 22) };
            runHands.SetAnimationFrames(
                new AnimationFrame(6, _playerAnimationSpeed),
                new AnimationFrame(7, _playerAnimationSpeed),
                new AnimationFrame(8, _playerAnimationSpeed),
                new AnimationFrame(9, _playerAnimationSpeed),
                new AnimationFrame(10, _playerAnimationSpeed),
                new AnimationFrame(11, _playerAnimationSpeed),
                new AnimationFrame(12, _playerAnimationSpeed),
                new AnimationFrame(13, _playerAnimationSpeed),
                new AnimationFrame(14, _playerAnimationSpeed),
                new AnimationFrame(15, _playerAnimationSpeed),
                new AnimationFrame(16, _playerAnimationSpeed),
                new AnimationFrame(17, _playerAnimationSpeed),
                new AnimationFrame(18, _playerAnimationSpeed),
                new AnimationFrame(19, _playerAnimationSpeed));
            runHands.SetAnimSprite(new AnimSprite(ssHands) { Color = BodyColor });

            _animatorHands.AddAnimation("Idle", idleHands);
            _animatorHands.AddAnimation("Tool", toolHands);
            _animatorHands.AddAnimation("Run", runHands);
            _animatorHands.AddAnimation("Jump", jumpHands);

            //Стоит штаны
            Animation idlePants = new Animation("idlePants") { Position = new Vector2f(0, 22) };
            idlePants.SetAnimationFrames(new AnimationFrame(0, _playerAnimationSpeed));
            idlePants.SetAnimSprite(new AnimSprite(ssPants) { Color = PantsColor });

            //Инстумент штаны
            Animation toolPants = new Animation("toolPants") { Position = new Vector2f(0, 22) };
            toolPants.SetAnimationFrames(new AnimationFrame(0, _playerAnimationSpeed));
            toolPants.SetAnimSprite(new AnimSprite(ssPants) { Color = PantsColor });

            //Прыжок штаны
            Animation jumpPants = new Animation("jumpPants") { Position = new Vector2f(0, 22) };
            jumpPants.SetAnimationFrames(new AnimationFrame(5, _playerAnimationSpeed));
            jumpPants.SetAnimSprite(new AnimSprite(ssPants) { Color = PantsColor });

            //Бег штаны
            Animation runPants = new Animation("runPants") { Position = new Vector2f(0, 22) };
            runPants.SetAnimationFrames(
                new AnimationFrame(6, _playerAnimationSpeed),
                new AnimationFrame(7, _playerAnimationSpeed),
                new AnimationFrame(8, _playerAnimationSpeed),
                new AnimationFrame(9, _playerAnimationSpeed),
                new AnimationFrame(10, _playerAnimationSpeed),
                new AnimationFrame(11, _playerAnimationSpeed),
                new AnimationFrame(12, _playerAnimationSpeed),
                new AnimationFrame(13, _playerAnimationSpeed),
                new AnimationFrame(14, _playerAnimationSpeed),
                new AnimationFrame(15, _playerAnimationSpeed),
                new AnimationFrame(16, _playerAnimationSpeed),
                new AnimationFrame(17, _playerAnimationSpeed),
                new AnimationFrame(18, _playerAnimationSpeed),
                new AnimationFrame(19, _playerAnimationSpeed));
            runPants.SetAnimSprite(new AnimSprite(ssPants) { Color = PantsColor });

            _animatorPants.AddAnimation("Idle", idlePants);
            _animatorPants.AddAnimation("Tool", toolPants);
            _animatorPants.AddAnimation("Run", runPants);
            _animatorPants.AddAnimation("Jump", jumpPants);


            //Стоит ноги
            Animation idleShoes = new Animation("idleShoes") { Position = new Vector2f(0, 22) };
            idleShoes.SetAnimationFrames(new AnimationFrame(0, _playerAnimationSpeed));
            idleShoes.SetAnimSprite(new AnimSprite(ssShoes));

            //Инстумент ноги
            Animation toolShoes = new Animation("toolShoes") { Position = new Vector2f(0, 22) };
            toolShoes.SetAnimationFrames(new AnimationFrame(0, _playerAnimationSpeed));
            toolShoes.SetAnimSprite(new AnimSprite(ssShoes));

            //Прыжок ноги
            Animation jumpShoes = new Animation("jumpShoes") { Position = new Vector2f(0, 22) };
            jumpShoes.SetAnimationFrames(new AnimationFrame(5, _playerAnimationSpeed));
            jumpShoes.SetAnimSprite(new AnimSprite(ssShoes));

            //Бег ноги
            Animation runShoes = new Animation("runShoes") { Position = new Vector2f(0, 22) };
            runShoes.SetAnimationFrames(
                new AnimationFrame(6, _playerAnimationSpeed),
                new AnimationFrame(7, _playerAnimationSpeed),
                new AnimationFrame(8, _playerAnimationSpeed),
                new AnimationFrame(9, _playerAnimationSpeed),
                new AnimationFrame(10, _playerAnimationSpeed),
                new AnimationFrame(11, _playerAnimationSpeed),
                new AnimationFrame(12, _playerAnimationSpeed),
                new AnimationFrame(13, _playerAnimationSpeed),
                new AnimationFrame(14, _playerAnimationSpeed),
                new AnimationFrame(15, _playerAnimationSpeed),
                new AnimationFrame(16, _playerAnimationSpeed),
                new AnimationFrame(17, _playerAnimationSpeed),
                new AnimationFrame(18, _playerAnimationSpeed),
                new AnimationFrame(19, _playerAnimationSpeed));
            runShoes.SetAnimSprite(new AnimSprite(ssShoes));

            _animatorShoes.AddAnimation("Idle", idleShoes);
            _animatorShoes.AddAnimation("Tool", toolShoes);
            _animatorShoes.AddAnimation("Run", runShoes);
            _animatorShoes.AddAnimation("Jump", jumpShoes);
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

                        Scale = new Vector2f(1, 1);
                        Origin = new Vector2f(-rect.Size.X / 2, 0);
                    }
                    else if (isLeftMove && velocity.X > -maxSpeed)
                    {
                        velocity += new Vector2f(-5, 0);

                        Scale = new Vector2f(-1, 1);   
                        Origin = new Vector2f(rect.Size.X / 2, 0);
                    }

                    _animatorHair.Play("Run");
                    _animatorHead.Play("Run");
                    _animatorShirt.Play("Run");
                    _animatorUndershirt.Play("Run");
                    _animatorHands.Play("Run");
                    _animatorPants.Play("Run");
                    _animatorShoes.Play("Run");
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

                    _animatorHair.Play("Jump");
                    _animatorHead.Play("Jump");
                    _animatorShirt.Play("Jump");
                    _animatorUndershirt.Play("Jump");
                    _animatorHands.Play("Jump");
                    _animatorPants.Play("Jump");
                    _animatorShoes.Play("Jump");
                }
                else if (!isJump)
                    isJumped = false;

                if(!isMove && !isJump)
                {
                    _animatorHair.Play("Idle");
                    _animatorHead.Play("Idle");
                    _animatorShirt.Play("Idle");
                    _animatorUndershirt.Play("Idle");
                    _animatorHands.Play("Idle");
                    _animatorPants.Play("Idle");
                    _animatorShoes.Play("Idle");
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
                                                          $"Tile Strength: {tile.Strength}");
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

            target.Draw(_animatorHead, states);
            target.Draw(_animatorHair, states);
            target.Draw(_animatorShirt, states);
            target.Draw(_animatorUndershirt, states);
            target.Draw(_animatorHands, states);
            target.Draw(_animatorPants, states);
            target.Draw(_animatorShoes, states);
        }
    }
}
