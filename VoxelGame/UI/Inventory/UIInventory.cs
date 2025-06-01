using SFML.Graphics;
using SFML.System;
using SFML.Window;
using VoxelGame.Item;
using VoxelGame.Physics;
using VoxelGame.Resources;

namespace VoxelGame.UI.Inventory
{
    /// <summary>
    /// Класс, представляющий интерфейс инвентаря игрока.
    /// Управляет ячейками инвентаря, добавлением предметов, выбором ячеек и взаимодействием с системой крафта.
    /// </summary>
    public class UIInventory : UIWindow
    {
        /// <summary>
        /// Количество ячеек инвентаря по горизонтали.
        /// </summary>
        public static int CellCountX { get; set; } = 10;

        /// <summary>
        /// Количество ячеек инвентаря по вертикали.
        /// </summary>
        public static int CellCountY { get; set; } = 6;

        /// <summary>
        /// Ссылка на выбранную ячейку инвентаря.
        /// </summary>
        private UIInventoryCell? _selectedCell;

        /// <summary>
        /// Ссылка на игрока, которому принадлежит инвентарь.
        /// </summary>
        public Player? Player { get; set; }

        /// <summary>
        /// Ссылка на интерфейс крафта, связанный с инвентарём.
        /// </summary>
        public UICraft? Craft { get; set; }

        /// <summary>
        /// Флаг, указывающий, отображается ли весь инвентарь.
        /// </summary>
        public bool IsFullInventoryVisible { get; private set; } = false;

        /// <summary>
        /// Конструктор инвентаря.
        /// </summary>
        /// <param name="size">Размер окна инвентаря.</param>
        public UIInventory(Vector2f size) : base(size, "Player Inventory")
        {
            Craft = new UICraft(this); // Инициализация интерфейса крафта

            TitleBarIsVisible = false; // Скрытие заголовка окна
            IsRectVisible = false;    // Скрытие прямоугольника окна

            // Создание верхнего ряда ячеек инвентаря
            for (int i = 0; i < CellCountX; i++)
            {
                Childs.Add(new UIInventoryCell() { Position = new Vector2f(i * UIInventoryCell.CellSize + (UIInventoryCell.CellSize / 2), UIInventoryCell.CellSize / 2), Perent = this });
            }

            SetSelectedCell((UIInventoryCell)Childs[0]); // Установка первой ячейки как выбранной

            // Создание остальных ячеек инвентаря
            for (int x = 0; x < CellCountX; x++)
            {
                for (int y = 1; y < CellCountY; y++)
                {
                    Childs.Add(new UIInventoryCell() { Position = new Vector2f(x * UIInventoryCell.CellSize, -y * UIInventoryCell.CellSize), Perent = this, IsVisible = false, IsUpdate = false });
                }
            }
        }

        /// <summary>
        /// Устанавливает выбранную ячейку инвентаря по индексу.
        /// </summary>
        /// <param name="cellIndex">Индекс ячейки.</param>
        public void SetSelectedCell(int cellIndex)
        {
            if (cellIndex < 0 || cellIndex > 10)
                return;

            SetSelectedCell((UIInventoryCell)Childs[cellIndex]);
        }

        /// <summary>
        /// Устанавливает выбранную ячейку инвентаря.
        /// </summary>
        /// <param name="inventoryCell">Ссылка на ячейку.</param>
        public void SetSelectedCell(UIInventoryCell inventoryCell)
        {
            if (_selectedCell != null)
                _selectedCell.IsSelected = false; // Снимаем выделение с предыдущей ячейки

            if (inventoryCell != null)
                inventoryCell.IsSelected = true; // Устанавливаем выделение на новую ячейку

            _selectedCell = inventoryCell;
        }

        /// <summary>
        /// Добавляет предмет в инвентарь.
        /// </summary>
        /// <param name="item">Предмет, который нужно добавить.</param>
        /// <returns>True, если предмет успешно добавлен.</returns>
        public bool AddItem(DropItem item)
        {
            return AddItem(item.Item, item.ItemCount);
        }

        /// <summary>
        /// Добавляет предмет в инвентарь.
        /// </summary>
        /// <param name="infoItem">Информация о предмете.</param>
        /// <param name="count">Количество предметов.</param>
        /// <returns>True, если предмет успешно добавлен.</returns>
        public bool AddItem(Item.Item infoItem, int count = 1)
        {
            return AddItem(new UIItemStack(infoItem), count);
        }

        /// <summary>
        /// Добавляет предмет в инвентарь.
        /// </summary>
        /// <param name="itemStack">Стак предметов.</param>
        /// <param name="count">Количество предметов.</param>
        /// <returns>True, если предмет успешно добавлен.</returns>
        public bool AddItem(UIItemStack itemStack, int count = 1)
        {
            var cell = GetSuitableCell(itemStack, count); // Поиск подходящей ячейки

            if (cell != null && cell.ItemStack != null)
            {
                cell.ItemStack.ItemCount += count; // Добавляем предметы в существующий стак
            }
            else
            {
                cell = GetEmptyCell(); // Поиск пустой ячейки

                if (cell == null)
                    return false;

                itemStack.ItemCount = count;
                cell.ItemStack = itemStack; // Устанавливаем предметы в пустую ячейку
            }

            return true;
        }

        /// <summary>
        /// Получает предмет из выбранной ячейки.
        /// </summary>
        /// <returns>Предмет или null, если ячейка пуста.</returns>
        public Item.Item? GetItemWithSelectedCell()
        {
            return _selectedCell?.GetItem();
        }

        /// <summary>
        /// Получает список доступных рецептов крафта.
        /// </summary>
        /// <returns>Список рецептов.</returns>
        public List<Craft> GetAvailableCrafts()
        {
            var crafts = new List<Craft>();
            for (int i = 1; i < Enum.GetNames<ItemList>().Length; i++)
            {
                var item = Items.GetItem((ItemList)i);

                if (item == null)
                    continue;

                var iCrafts = item.GetCrafts();

                if (iCrafts == null)
                    continue;

                foreach (var craft in iCrafts)
                {
                    for (int i2 = 0; i2 < craft.Items.Length; i2++)
                    {
                        var itemC = craft.Items[i2];

                        // Проверяем наличие необходимых ресурсов и инструментов
                        if (GetItemCount(itemC.Item) < itemC.Count || !CanTool(Player!.CraftTools, craft.Tool))
                        {
                            break;
                        }

                        if (i2 == craft.Items.Length - 1)
                        {
                            crafts.Add(craft); // Добавляем рецепт, если все условия выполнены
                        }
                    }
                }
            }

            return crafts;
        }

        /// <summary>
        /// Проверяет, доступен ли инструмент для крафта.
        /// </summary>
        /// <param name="a">Список доступных инструментов.</param>
        /// <param name="b">Требуемый инструмент.</param>
        /// <returns>True, если инструмент доступен.</returns>
        bool CanTool(List<CraftTool> a, CraftTool b)
        {
            foreach (var tool in a)
            {
                if ((tool & b) != 0 && (b & tool) != 0) return true;
            }

            return false;
        }

        /// <summary>
        /// Получает количество предметов в инвентаре.
        /// </summary>
        /// <param name="item">Тип предмета.</param>
        /// <returns>Количество предметов.</returns>
        public int GetItemCount(ItemList item)
        {
            var itemStack = GetItemStackByItem(item);

            if (itemStack == null) return 0;

            return itemStack.ItemCount;
        }

        /// <summary>
        /// Получает стак предметов по типу.
        /// </summary>
        /// <param name="item">Тип предмета.</param>
        /// <returns>Стак предметов или null.</returns>
        public UIItemStack? GetItemStackByItem(ItemList item)
        {
            var cell = GetCellByItem(item);

            if (cell == null) return null;

            return cell.ItemStack;
        }

        /// <summary>
        /// Получает ячейку инвентаря по типу предмета.
        /// </summary>
        /// <param name="item">Тип предмета.</param>
        /// <returns>Ячейка инвентаря или null.</returns>
        public UIInventoryCell? GetCellByItem(ItemList item)
        {
            return Childs.Select(c => c as UIInventoryCell).ToList()
                .Find(c => c!.GetItem() != null && c!.GetItem()!.ItemList == item);
        }

        /// <summary>
        /// Получает подходящую ячейку для добавления предметов.
        /// </summary>
        /// <param name="itemStack">Стак предметов.</param>
        /// <param name="count">Количество предметов.</param>
        /// <returns>Ячейка инвентаря или null.</returns>
        private UIInventoryCell? GetSuitableCell(UIItemStack itemStack, int count = 1)
        {
            var type = itemStack.ItemType;

            var cells = Childs.Select(c => c as UIInventoryCell).ToList();

            var cellResult = cells.Find(cell => cell != null && cell.ItemStack != null
                   && cell.ItemStack.ItemType == type && cell.ItemStack.Item.ItemList == itemStack.Item.ItemList && !cell.ItemStack.IsFull);

            return cellResult;
        }

        /// <summary>
        /// Получает пустую ячейку инвентаря.
        /// </summary>
        /// <returns>Пустая ячейка или null.</returns>
        private UIInventoryCell? GetEmptyCell()
        {
            var cells = Childs.Select(c => c as UIInventoryCell).ToList();

            return cells.Find(cell => cell != null && cell.ItemStack == null);
        }

        /// <summary>
        /// Получает выбранную ячейку инвентаря.
        /// </summary>
        /// <returns>Выбранная ячейка или null.</returns>
        public UIInventoryCell? GetSelectedCell()
        {
            return Childs.Select(c => c as UIInventoryCell).ToList().Find(cell => cell!.IsSelected);
        }

        /// <summary>
        /// Отображает весь инвентарь.
        /// </summary>
        public void ShowInventory()
        {
            rect.Size = new Vector2f(UIInventoryCell.CellSize * CellCountX, UIInventoryCell.CellSize * CellCountY);

            IsFullInventoryVisible = true;

            for (int x = 0; x < CellCountX; x++)
            {
                for (int y = 0; y < CellCountY; y++)
                {
                    int index = x + y * CellCountX;

                    var child = Childs[index];
                    child.Position = new Vector2f(x * UIInventoryCell.CellSize - Origin.X + UIInventoryCell.CellSize / 2, y * UIInventoryCell.CellSize + Origin.Y + UIInventoryCell.CellSize / 2);
                    child.IsVisible = true;
                    child.IsUpdate = true;
                    child.CanDrop = true;
                }
            }
        }

        /// <summary>
        /// Скрывает весь инвентарь, оставляя только верхний ряд.
        /// </summary>
        public void HideInventory()
        {
            IsFullInventoryVisible = false;

            rect.Size = new Vector2f(UIInventoryCell.CellSize * CellCountX, UIInventoryCell.CellSize);

            for (int x = 0; x < CellCountX; x++)
            {
                for (int y = 1; y < CellCountY; y++)
                {
                    int index = x + y * CellCountX;

                    var child = Childs[index];

                    child.IsVisible = false;
                    child.IsUpdate = false;
                    child.CanDrag = false;
                    child.CanDrop = false;
                }
            }
        }

        /// <summary>
        /// Обновляет инвентарь.
        /// </summary>
        /// <param name="deltaTime">Время между кадрами.</param>
        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            if (Craft != null)
                Craft.UpdateOver(deltaTime);

            if (IsFullInventoryVisible && Craft != null)
            {
                Craft.Position = new Vector2f(Game.GetWindowSizeWithZoom().X - UIInventoryCell.CellSize * CellCountX / 2 - UIInventoryCell.CellSize / 2, Game.GetWindowSizeWithZoom().Y / 4f) + Position;
                Craft.Update(deltaTime);
            }
        }

        /// <summary>
        /// Обновляет инвентарь при наведении.
        /// </summary>
        /// <param name="deltaTime">Время между кадрами.</param>
        public override void UpdateOver(float deltaTime)
        {
            base.UpdateOver(deltaTime);

            if (!IsFullInventoryVisible && IsHovered && Mouse.IsButtonPressed(Mouse.Button.Left))
            {
                var select = (UIInventoryCell)Childs.Find(c => c.IsHovered)!;
                if (select != null)
                {
                    SetSelectedCell(select);
                }
            }
        }

        /// <summary>
        /// Рисует инвентарь.
        /// </summary>
        /// <param name="target">Цель отрисовки.</param>
        /// <param name="states">Состояния рендера.</param>
        public override void Draw(RenderTarget target, RenderStates states)
        {
            base.Draw(target, states);

            if (IsFullInventoryVisible && Craft != null)
                Craft.Draw(target, states);
        }
    }
}