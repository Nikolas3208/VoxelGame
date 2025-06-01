using SFML.Graphics;
using SFML.System;
using SFML.Window;
using VoxelGame.Entitys;
using VoxelGame.Graphics.Animations;
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
    /// <summary>
    /// Перечисление возможных анимаций игрока.
    /// </summary>
    public enum PlayerAnim
    {
        /// <summary>
        /// Анимация ожидания (статическое состояние игрока).
        /// </summary>
        Idle,   // Ожидание

        /// <summary>
        /// Анимация бега (движение игрока).
        /// </summary>
        Run,    // Бег

        /// <summary>
        /// Анимация прыжка (игрок в воздухе).
        /// </summary>
        Jump,   // Прыжок

        /// <summary>
        /// Анимация использования инструмента (например, кирки или молота).
        /// </summary>
        Tool    // Использование инструмента
    }

    /// <summary>
    /// Класс, реализующий игрового персонажа-игрока.
    /// Управляет состоянием, анимациями, инвентарём, взаимодействием с миром и предметами.
    /// </summary>
    public class Player : Npc
    {
        /// <summary>
        /// Максимальная скорость передвижения игрока.
        /// </summary>
        private float maxSpeed = 100;

        /// <summary>
        /// Флаг, совершён ли прыжок.
        /// </summary>
        private bool isJumped = false;

        /// <summary>
        /// Флаг, открыт ли инвентарь.
        /// </summary>
        private bool _inventoryOpened = false;

        /// <summary>
        /// UI-инвентарь игрока.
        /// </summary>
        private UIInventory _inventory;

        
        ///<summary>  
        /// Ссылка на сундук, с которым взаимодействует игрок.  
        /// Используется для открытия инвентаря сундука и взаимодействия с его содержимым.  
        /// </summary>  
        private TileChest? _chest;

        /// <summary>
        /// Текущий выбранный предмет в инвентаре.
        /// </summary>
        private Item.Item? _selectedItem;

        /// <summary>
        /// Скорость анимации игрока (обычная).
        /// </summary>
        private float _playerAnimationSpeed = 0.2f;

        /// <summary>
        /// Скорость анимации при использовании инструмента.
        /// </summary>
        private float _playerAnimationSpeedTool = 4.3f;

        /// <summary>
        /// Сущность выпавшего предмета (если есть).
        /// </summary>
        private DropItem _itemEntity;

        /// <summary>
        /// Смещение анимации относительно базовой позиции.
        /// </summary>
        private Vector2f _animOffset = new Vector2f(0, 19);

        /// <summary>
        /// Стартовая позиция игрока.
        /// </summary>
        public Vector2f StartPosition = new Vector2f(0, 0);

        /// <summary>
        /// Текст для отображения таймера (например, при смерти).
        /// </summary>
        private Text _timerText;

        /// <summary>
        /// Индекс выбранной ячейки инвентаря.
        /// </summary>
        private int _selectedCellIndex = 0;

        /// <summary>
        /// Аниматор для управления анимациями игрока.
        /// </summary>
        private Animator _playerAnimator;

        /// <summary>
        /// Скорость разрушения блоков выбранным инструментом.
        /// </summary>
        private float breakingSpeed = 1f;

        /// <summary>
        /// Цвет волос игрока.
        /// </summary>
        public static Color HairColor = new Color(0, 240, 10);

        /// <summary>
        /// Цвет кожи игрока.
        /// </summary>
        public static Color BodyColor = new Color(255, 229, 186);

        /// <summary>
        /// Цвет рубашки игрока.
        /// </summary>
        public static Color ShirtColor = new Color(255, 255, 0);

        /// <summary>
        /// Цвет штанов игрока.
        /// </summary>
        public static Color PantsColor = new Color(0, 76, 135);

        /// <summary>
        /// Список доступных инструментов для крафта рядом с игроком.
        /// </summary>
        public List<CraftTool> CraftTools { get; set; }

        /// <summary>
        /// Конструктор игрока.
        /// </summary>
        /// <param name="world">Мир, в котором находится игрок.</param>
        /// <param name="aabb">Коллайдер игрока.</param>
        public Player(World world, AABB aabb) : base(world, aabb, NpcType.Friendly)
        {
            _inventory = new UIInventory(new Vector2f(UIInventoryCell.CellSize * 10, UIInventoryCell.CellSize));
            _inventory.Player = this;

            _playerAnimator = new Animator();

            hitSoundName = "Player_Hit_";
            killSoundName = "Player_Killed_";

            NpcType = NpcType.Player;

            _timerText = new Text(" ", TextureManager.GetFont("Arial"));

            UIManager.AddWindow(_inventory);

            // Добавление стартовых предметов в инвентарь
            _inventory.AddItem(Items.GetItem(ItemList.CopperPickaxe), 1);
            _inventory.AddItem(Items.GetItem(ItemList.CopperAxe), 1);
            _inventory.AddItem(Items.GetItem(ItemList.CopperSword), 1);
            _inventory.AddItem(Items.GetItem(ItemList.Stove), 1);
            _inventory.AddItem(Items.GetItem(ItemList.OakBoard), 64);
            _inventory.AddItem(Items.GetItem(ItemList.OakBoardWall), 64);
            _inventory.AddItem(Items.GetItem(ItemList.Torch), 64);
            _inventory.AddItem(Items.GetItem(ItemList.IronIngot), 64);
            _inventory.AddItem(Items.GetItem(ItemList.CopperIngot), 64);
            _inventory.AddItem(Items.GetItem(ItemList.Chest), 64);

            CraftTools = new List<CraftTool>();

            Origin = new Vector2f(-rect.Size.X / 2, 0);
            Visible = false;

            // Подписка на событие прокрутки колеса мыши для смены выбранной ячейки инвентаря
            Game.Window.MouseWheelScrolled += GameWindow_MouseWheelScrolled;

            CreateAnimation();
        }

        /// <summary>
        /// Обработка прокрутки колеса мыши для смены выбранной ячейки инвентаря.
        /// </summary>
        private void GameWindow_MouseWheelScrolled(object? sender, MouseWheelScrollEventArgs e)
        {
            _selectedCellIndex += (int)e.Delta;

            if (_selectedCellIndex < 0)
                _selectedCellIndex = 9;
            else if (_selectedCellIndex > 9)
                _selectedCellIndex = 0;

            _inventory.SetSelectedCell(_selectedCellIndex);
        }

        /// <summary>
        /// Создание и настройка анимаций для всех частей тела игрока.
        /// </summary>
        public void CreateAnimation()
        {
            var ssHair = TextureManager.GetSpriteSheet("Player_Hair_1", 1, 14, true, 0);
            var ssHead = TextureManager.GetSpriteSheet("Player_Head", 1, 20, true, 0);
            var ssShirt = TextureManager.GetSpriteSheet("Player_Shirt", 1, 20, true, 0);
            var ssUndershirt = TextureManager.GetSpriteSheet("Player_Undershirt", 1, 20, true, 0);
            var ssHands = TextureManager.GetSpriteSheet("Player_Hands", 1, 20, true, 0);
            var ssPants = TextureManager.GetSpriteSheet("Player_Pants", 1, 20, true, 0);
            var ssShoes = TextureManager.GetSpriteSheet("Player_Shoes", 1, 20, true, 0);

            _playerAnimator.AddAnimation("Idle",
                new Animation(ssHead, AnimationType.Default,
                    new AnimationFrame(0, 0, _playerAnimationSpeed, position: _animOffset, color: BodyColor)),
                new Animation(ssHair, AnimationType.Default,
                    new AnimationFrame(0, 0, _playerAnimationSpeed, position: _animOffset, color: HairColor)),
                new Animation(ssShirt, AnimationType.Default,
                    new AnimationFrame(0, 0, _playerAnimationSpeed, position: _animOffset, color: ShirtColor)),
                new Animation(ssUndershirt, AnimationType.Default,
                    new AnimationFrame(0, 0, _playerAnimationSpeed, position: _animOffset)),
                new Animation(ssHands, AnimationType.Default,
                    new AnimationFrame(0, 0, _playerAnimationSpeed, position: _animOffset, color: BodyColor)),
                new Animation(ssPants, AnimationType.Default,
                    new AnimationFrame(0, 0, _playerAnimationSpeed, position: _animOffset, color: PantsColor)),
                new Animation(ssShoes, AnimationType.Default,
                    new AnimationFrame(0, 0, _playerAnimationSpeed, position: _animOffset)));

            // Волосы
            //asHair = new AnimSprite(ssHair);
            //asHair.BasePosition = _animOffset;
            //asHair.Color = HairColor;
            //asHair.AddAnimation("Idle", new Animation(
            //    new AnimationFrame(0, 0, _playerAnimationSpeed)));
            //asHair.AddAnimation("Tool", new Animation(
            //    new AnimationFrame(0, 0, _playerAnimationSpeedTool)));
            //asHair.AddAnimation("Jump", new Animation(
            //    new AnimationFrame(0, 5, _playerAnimationSpeed)));
            //asHair.AddAnimation("Run", new Animation(
            //    new AnimationFrame(0, 0, _playerAnimationSpeed),
            //    new AnimationFrame(0, 1, _playerAnimationSpeed),
            //    new AnimationFrame(0, 2, _playerAnimationSpeed),
            //    new AnimationFrame(0, 3, _playerAnimationSpeed),
            //    new AnimationFrame(0, 4, _playerAnimationSpeed),
            //    new AnimationFrame(0, 5, _playerAnimationSpeed),
            //    new AnimationFrame(0, 6, _playerAnimationSpeed),
            //    new AnimationFrame(0, 7, _playerAnimationSpeed),
            //    new AnimationFrame(0, 8, _playerAnimationSpeed),
            //    new AnimationFrame(0, 9, _playerAnimationSpeed),
            //    new AnimationFrame(0, 10, _playerAnimationSpeed),
            //    new AnimationFrame(0, 11, _playerAnimationSpeed),
            //    new AnimationFrame(0, 12, _playerAnimationSpeed),
            //    new AnimationFrame(0, 13, _playerAnimationSpeed)
            //));

            //// Голова
            //asHead = new AnimSprite(ssHead);
            //asHead.BasePosition = _animOffset;
            //asHead.Color = BodyColor;
            //asHead.AddAnimation("Idle", new Animation(
            //    new AnimationFrame(0, 0, _playerAnimationSpeed)));
            //asHead.AddAnimation("Tool", new Animation(
            //    new AnimationFrame(0, 0, _playerAnimationSpeedTool)));
            //asHead.AddAnimation("Jump", new Animation(
            //    new AnimationFrame(0, 5, _playerAnimationSpeed)));
            //asHead.AddAnimation("Run", new Animation(
            //    new AnimationFrame(0, 6, _playerAnimationSpeed),
            //    new AnimationFrame(0, 7, _playerAnimationSpeed),
            //    new AnimationFrame(0, 8, _playerAnimationSpeed),
            //    new AnimationFrame(0, 9, _playerAnimationSpeed),
            //    new AnimationFrame(0, 10, _playerAnimationSpeed),
            //    new AnimationFrame(0, 11, _playerAnimationSpeed),
            //    new AnimationFrame(0, 12, _playerAnimationSpeed),
            //    new AnimationFrame(0, 13, _playerAnimationSpeed),
            //    new AnimationFrame(0, 14, _playerAnimationSpeed),
            //    new AnimationFrame(0, 15, _playerAnimationSpeed),
            //    new AnimationFrame(0, 16, _playerAnimationSpeed),
            //    new AnimationFrame(0, 17, _playerAnimationSpeed),
            //    new AnimationFrame(0, 18, _playerAnimationSpeed),
            //    new AnimationFrame(0, 19, _playerAnimationSpeed)
            //));

            //// Рубашка
            //asShirt = new AnimSprite(ssShirt);
            //asShirt.BasePosition = _animOffset;
            //asShirt.Color = ShirtColor;
            //asShirt.AddAnimation("Idle", new Animation(
            //    new AnimationFrame(0, 0, _playerAnimationSpeed)));
            //asShirt.AddAnimation("Tool", new Animation(
            //    new AnimationFrame(0, 0, _playerAnimationSpeedTool),
            //    new AnimationFrame(0, 1, _playerAnimationSpeedTool),
            //    new AnimationFrame(0, 2, _playerAnimationSpeedTool),
            //    new AnimationFrame(0, 3, _playerAnimationSpeedTool),
            //    new AnimationFrame(0, 4, _playerAnimationSpeedTool)));
            //asShirt.AddAnimation("Jump", new Animation(
            //    new AnimationFrame(0, 5, _playerAnimationSpeed)));
            //asShirt.AddAnimation("Run", new Animation(
            //    new AnimationFrame(0, 6, _playerAnimationSpeed),
            //    new AnimationFrame(0, 7, _playerAnimationSpeed),
            //    new AnimationFrame(0, 8, _playerAnimationSpeed),
            //    new AnimationFrame(0, 9, _playerAnimationSpeed),
            //    new AnimationFrame(0, 10, _playerAnimationSpeed),
            //    new AnimationFrame(0, 11, _playerAnimationSpeed),
            //    new AnimationFrame(0, 12, _playerAnimationSpeed),
            //    new AnimationFrame(0, 13, _playerAnimationSpeed),
            //    new AnimationFrame(0, 14, _playerAnimationSpeed),
            //    new AnimationFrame(0, 15, _playerAnimationSpeed),
            //    new AnimationFrame(0, 16, _playerAnimationSpeed),
            //    new AnimationFrame(0, 17, _playerAnimationSpeed),
            //    new AnimationFrame(0, 18, _playerAnimationSpeed),
            //    new AnimationFrame(0, 19, _playerAnimationSpeed)
            //));

            //// Рукава
            //asUndershirt = new AnimSprite(ssUndershirt);
            //asUndershirt.BasePosition = _animOffset;
            //asUndershirt.AddAnimation("Idle", new Animation(
            //    new AnimationFrame(0, 0, _playerAnimationSpeed)));
            //asUndershirt.AddAnimation("Tool", new Animation(
            //    new AnimationFrame(0, 0, _playerAnimationSpeedTool),
            //    new AnimationFrame(0, 1, _playerAnimationSpeedTool),
            //    new AnimationFrame(0, 2, _playerAnimationSpeedTool),
            //    new AnimationFrame(0, 3, _playerAnimationSpeedTool),
            //    new AnimationFrame(0, 4, _playerAnimationSpeedTool)));
            //asUndershirt.AddAnimation("Jump", new Animation(
            //    new AnimationFrame(0, 5, _playerAnimationSpeed)));
            //asUndershirt.AddAnimation("Run", new Animation(
            //    new AnimationFrame(0, 6, _playerAnimationSpeed),
            //    new AnimationFrame(0, 7, _playerAnimationSpeed),
            //    new AnimationFrame(0, 8, _playerAnimationSpeed),
            //    new AnimationFrame(0, 9, _playerAnimationSpeed),
            //    new AnimationFrame(0, 10, _playerAnimationSpeed),
            //    new AnimationFrame(0, 11, _playerAnimationSpeed),
            //    new AnimationFrame(0, 12, _playerAnimationSpeed),
            //    new AnimationFrame(0, 13, _playerAnimationSpeed),
            //    new AnimationFrame(0, 14, _playerAnimationSpeed),
            //    new AnimationFrame(0, 15, _playerAnimationSpeed),
            //    new AnimationFrame(0, 16, _playerAnimationSpeed),
            //    new AnimationFrame(0, 17, _playerAnimationSpeed),
            //    new AnimationFrame(0, 18, _playerAnimationSpeed),
            //    new AnimationFrame(0, 19, _playerAnimationSpeed)
            //));

            //// Кисти
            //asHands = new AnimSprite(ssHands);
            //asHands.BasePosition = _animOffset;
            //asHands.Color = BodyColor;
            //asHands.AddAnimation("Idle", new Animation(
            //    new AnimationFrame(0, 0, _playerAnimationSpeed)));
            //asHands.AddAnimation("Tool", new Animation(
            //    new AnimationFrame(0, 0, _playerAnimationSpeedTool),
            //    new AnimationFrame(0, 1, _playerAnimationSpeedTool),
            //    new AnimationFrame(0, 2, _playerAnimationSpeedTool),
            //    new AnimationFrame(0, 3, _playerAnimationSpeedTool),
            //    new AnimationFrame(0, 4, _playerAnimationSpeedTool)));
            //asHands.AddAnimation("Jump", new Animation(
            //    new AnimationFrame(0, 5, _playerAnimationSpeed)));
            //asHands.AddAnimation("Run", new Animation(
            //    new AnimationFrame(0, 6, _playerAnimationSpeed),
            //    new AnimationFrame(0, 7, _playerAnimationSpeed),
            //    new AnimationFrame(0, 8, _playerAnimationSpeed),
            //    new AnimationFrame(0, 9, _playerAnimationSpeed),
            //    new AnimationFrame(0, 10, _playerAnimationSpeed),
            //    new AnimationFrame(0, 11, _playerAnimationSpeed),
            //    new AnimationFrame(0, 12, _playerAnimationSpeed),
            //    new AnimationFrame(0, 13, _playerAnimationSpeed),
            //    new AnimationFrame(0, 14, _playerAnimationSpeed),
            //    new AnimationFrame(0, 15, _playerAnimationSpeed),
            //    new AnimationFrame(0, 16, _playerAnimationSpeed),
            //    new AnimationFrame(0, 17, _playerAnimationSpeed),
            //    new AnimationFrame(0, 18, _playerAnimationSpeed),
            //    new AnimationFrame(0, 19, _playerAnimationSpeed)
            //));

            //// Ноги
            //asPants = new AnimSprite(ssPants);
            //asPants.Color = PantsColor;
            //asPants.BasePosition = _animOffset;
            //asPants.AddAnimation("Idle", new Animation(
            //    new AnimationFrame(0, 0, _playerAnimationSpeed)));
            //asPants.AddAnimation("Tool", new Animation(
            //    new AnimationFrame(0, 0, _playerAnimationSpeedTool)));
            //asPants.AddAnimation("Jump", new Animation(
            //    new AnimationFrame(0, 5, _playerAnimationSpeed)));
            //asPants.AddAnimation("Run", new Animation(
            //    new AnimationFrame(0, 6, _playerAnimationSpeed),
            //    new AnimationFrame(0, 7, _playerAnimationSpeed),
            //    new AnimationFrame(0, 8, _playerAnimationSpeed),
            //    new AnimationFrame(0, 9, _playerAnimationSpeed),
            //    new AnimationFrame(0, 10, _playerAnimationSpeed),
            //    new AnimationFrame(0, 11, _playerAnimationSpeed),
            //    new AnimationFrame(0, 12, _playerAnimationSpeed),
            //    new AnimationFrame(0, 13, _playerAnimationSpeed),
            //    new AnimationFrame(0, 14, _playerAnimationSpeed),
            //    new AnimationFrame(0, 15, _playerAnimationSpeed),
            //    new AnimationFrame(0, 16, _playerAnimationSpeed),
            //    new AnimationFrame(0, 17, _playerAnimationSpeed),
            //    new AnimationFrame(0, 18, _playerAnimationSpeed),
            //    new AnimationFrame(0, 19, _playerAnimationSpeed)
            //));

            //// Обувь
            //asShoes = new AnimSprite(ssShoes);
            //asShoes.BasePosition = _animOffset;
            //asShoes.AddAnimation("Idle", new Animation(
            //    new AnimationFrame(0, 0, _playerAnimationSpeed)));
            //asShoes.AddAnimation("Tool", new Animation(
            //    new AnimationFrame(0, 0, _playerAnimationSpeedTool)));
            //asShoes.AddAnimation("Jump", new Animation(
            //    new AnimationFrame(0, 5, _playerAnimationSpeed)));
            //asShoes.AddAnimation("Run", new Animation(
            //    new AnimationFrame(0, 6, _playerAnimationSpeed),
            //    new AnimationFrame(0, 7, _playerAnimationSpeed),
            //    new AnimationFrame(0, 8, _playerAnimationSpeed),
            //    new AnimationFrame(0, 9, _playerAnimationSpeed),
            //    new AnimationFrame(0, 10, _playerAnimationSpeed),
            //    new AnimationFrame(0, 11, _playerAnimationSpeed),
            //    new AnimationFrame(0, 12, _playerAnimationSpeed),
            //    new AnimationFrame(0, 13, _playerAnimationSpeed),
            //    new AnimationFrame(0, 14, _playerAnimationSpeed),
            //    new AnimationFrame(0, 15, _playerAnimationSpeed),
            //    new AnimationFrame(0, 16, _playerAnimationSpeed),
            //    new AnimationFrame(0, 17, _playerAnimationSpeed),
            //    new AnimationFrame(0, 18, _playerAnimationSpeed),
            //    new AnimationFrame(0, 19, _playerAnimationSpeed)
            //));
        }

        /// <summary>
        /// Таймер для отсчёта времени после смерти игрока.
        /// </summary>
        float timer = 0;
        /// <summary>
        /// Время до возрождения после смерти.
        /// </summary>
        float startTime = 12;

        /// <summary>
        /// Основной метод обновления состояния игрока.
        /// </summary>
        /// <param name="deltaTime">Время между кадрами.</param>
        public override void Update(float deltaTime)
        {
            if(Kill)
            {
                timer += 1;

                if(timer >= 60)
                {
                    startTime -= 1;
                    timer = 0;
                }
                if(startTime <= 0)
                {
                    Position = StartPosition;
                    Kill = false;
                    Health = MaxHealth;
                    UIManager.AddWindow(_inventory);
                }

                _timerText.DisplayedString = startTime.ToString();
                _timerText.FillColor = Color.Red;
                _timerText.Position = Position;

                return;
            }

            base.Update(deltaTime);
            _playerAnimator.Update(deltaTime);

            MouseUpdate();

            //_inventory.Position = -Game.GetWindowSizeWithZoom() / 2;

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
                    _selectedCellIndex = i;
                }
            }

            if (Keyboard.IsKeyPressed(Keyboard.Key.Num0))
                _inventory.SetSelectedCell(9);

            if (Keyboard.IsKeyPressed(Keyboard.Key.E) && !_inventoryOpened)
            {
                if (_inventory.IsFullInventoryVisible)
                {
                    _inventory.HideInventory();

                    UIManager.RemoveWindow(_chest?.GetInventory()!);
                }
                else
                    _inventory.ShowInventory();

                _inventoryOpened = true;
            }
            else if (!Keyboard.IsKeyPressed(Keyboard.Key.E))
            {
                _inventoryOpened = false;

            }

            if (_chest != null)
            {
                if (MathHelper.Distance(Position, _chest.GlobalPosition) > 6 * Tile.TileSize)
                {
                    UIManager.RemoveWindow(_chest?.GetInventory()!);
                }
            }

            _selectedItem = _inventory.GetItemWithSelectedCell();

            if (_selectedItem != null && _selectedItem.ItemList == ItemList.Torch)
                world.AddLight(Position + _animOffset);

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
                        if(_itemEntity != null)
                            _itemEntity.Scale = new Vector2f(1, 1);
                    }
                    else if (isLeftMove && velocity.X > -maxSpeed)
                    {
                        velocity += new Vector2f(-5, 0);

                        Scale = new Vector2f(-1, 1);   
                        Origin = new Vector2f(rect.Size.X / 2, 0);
                        if (_itemEntity != null)
                            _itemEntity.Scale = new Vector2f(-1, 1);
                    }

                    UseAnim(PlayerAnim.Run);
                }
                else
                {
                    velocity = new Vector2f(0, velocity.Y);

                    if (!Mouse.IsButtonPressed(Mouse.Button.Left))
                        UseAnim(PlayerAnim.Idle);
                }

                if (isJump && !isFall && !isJumped)
                {
                    velocity += new Vector2f(0, -200);
                    isFall = true;
                    isJumped = true;
                }
                else if (!isJump)
                    isJumped = false;

                if (isJump)
                {
                    UseAnim(PlayerAnim.Jump);
                }
            }


            if (_itemEntity != null)
            {
                _itemEntity.Scale = new Vector2f(Scale.X, 1);

                //if (asHands.GetCurrentFrame() == (0, 0))
                //{
                //    _itemEntity.Position = Position + new Vector2f(6, 34);
                //    _itemEntity.Rotation = -180;
                //    _itemEntity.SetAABB(new AABB(-_itemEntity.Size / 2 - new Vector2f(12, 0), _itemEntity.Size / 2 + new Vector2f(0, 12)));
                //}
                //else if (asHands.GetCurrentFrame() == (0, 1))
                //{
                //    _itemEntity.Position = Position + new Vector2f(4, 12);
                //    _itemEntity.Rotation = -85;
                //    _itemEntity.SetAABB(new AABB(-_itemEntity.Size / 2 - new Vector2f(12, 12), new Vector2f(0, 12)));
                //}
                //else if (asHands.GetCurrentFrame() == (0, 2))
                //{
                //    _itemEntity.Position = Position + new Vector2f(19, 12);
                //    _itemEntity.Rotation = 0;
                //    _itemEntity.SetAABB(new AABB(new Vector2f(0, -_itemEntity.Size.Y) / 2 + new Vector2f(0, -12), _itemEntity.Size + new Vector2f(0, -12)));
                //}
                //else if (asHands.GetCurrentFrame() == (0, 3))
                //{
                //    _itemEntity.Position = Position + new Vector2f(22, 26);
                //    _itemEntity.Rotation = 65;
                //    _itemEntity.SetAABB(new AABB(new Vector2f(0, _itemEntity.Size.Y) / 2 + new Vector2f(0, -22), _itemEntity.Size + new Vector2f(0, -26 + (_itemEntity.Size.Y / 2))));
                //}
                //else if (asHands.GetCurrentFrame() == (0, 4))
                //{
                //    _itemEntity.Position = Position + new Vector2f(16, 32);
                //    _itemEntity.Rotation = 85;
                //    _itemEntity.SetAABB(new AABB(new Vector2f(0, _itemEntity.Size.Y) / 2 + new Vector2f(0, -22), _itemEntity.Size + new Vector2f(2, -22 + (_itemEntity.Size.Y / 2))));
                //}
            }
        }

        /// <summary>
        /// Обработка взаимодействия игрока с мышью (клик, установка/разрушение блоков и т.д.).
        /// </summary>
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
                            else if (tile != null && tile is IUsedTile usedTile)
                            {
                                usedTile.Use();
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

                            if (_selectedItem is ItemTool itemTool)
                            {
                                AudioManager.PlaySuond("Item_1");

                                if (tile != null && tile.IsRequiredToolAndPower(itemTool!.Type, itemTool.Power))
                                {
                                    chunk.BreakingTileByWorldPosition(mousePos, itemTool!.Type, itemTool.Power, itemTool.BreakingSpeed * 0.5f);
                                    AudioManager.PlaySuond("Dig_0");
                                }

                                else if (wall != null && itemTool.Type == ItemType.Hammer)
                                {
                                    chunk.BreakingWallByWorldPosition(mousePos, itemTool!.Type, itemTool.Power, itemTool.BreakingSpeed * 0.5f);
                                    AudioManager.PlaySuond("Dig_2");
                                }

                            }

                            else if (tile != null && !itemIsTool)
                            {
                                chunk.BreakingTileByWorldPosition(mousePos, ItemType.None, 0, _playerAnimationSpeed);
                            }

                            if(_selectedItem != null && _itemEntity == null)
                            {
                                var texture = TextureManager.GetTexture(_selectedItem.SpriteName);

                                _itemEntity = new DropItem(_selectedItem, new AABB(new Vector2f(texture.Size.X, texture.Size.Y)), world) { IsDrop = false, Position = Position };
                               // _itemEntity.Origin = new Vector2f(0, texture.Size.Y);
                                _itemEntity.UseAnimation();
                                world.AddEntity(_itemEntity);
                            }

                            UseAnim(PlayerAnim.Tool);
                        }
                        else if(!mouseLeftClick)
                        {
                            world.RemoveEntity(_itemEntity);
                            _itemEntity = null;
                        }
                    }
                }
            }
        }

        /// <summary>  
        /// Отображает инвентарь игрока на экране.  
        /// </summary>  
        /// <remarks>  
        /// Этот метод вызывает отображение UI-инвентаря игрока,  
        /// позволяя взаимодействовать с предметами, находящимися в инвентаре.  
        /// </remarks>  
        public void ShowInventory(Tile tile)
        {
            if(tile is TileChest chestTile)
            {
                _chest = chestTile;
            }

            _inventory.ShowInventory();
        }

        /// <summary>
        /// Запустить анимацию для всех частей тела игрока.
        /// </summary>
        /// <param name="anim">Тип анимации.</param>
        public void UseAnim(PlayerAnim anim)
        {
            _playerAnimator.Play("Idle");
        }
        /// <summary>
        /// Анимация предмета в руке
        /// </summary>
        public void ItemAnim()
        {

        }

        /// <summary>
        /// Обработка столкновения игрока с другой сущностью.
        /// </summary>
        public override void OnCollided(Entity other, Vector2f normal, float depth)
        {
            base.OnCollided(other, normal, depth);

            if (other == null)
                return;

            // Если столкновение с предметом - попытка подобрать его в инвентарь
            if (other.Layer is CollisionLayer.Item)
            {
                var item = other as DropItem;
                if (item != null && item.IsDrop)
                {
                    if (_inventory.AddItem(item.Item))
                    {
                        AudioManager.PlaySuond("Grab");
                        world.RemoveItem(item);
                    }
                }
            }
        }

        /// <summary>
        /// Действия при смерти игрока.
        /// </summary>
        public override void OnKill()
        {
            base.OnKill();

            UIManager.RemoveWindow(_inventory);
            Kill = true;
        }

        /// <summary>
        /// Отрисовка игрока и его анимаций.
        /// </summary>
        public override void Draw(RenderTarget target, RenderStates states)
        {
            if (Kill)
            {
                target.Draw(_timerText, states);

                return;
            }

            base.Draw(target, states);

            states.Transform *= Transform;

            target.Draw(_playerAnimator, states);
        }
    }
}
