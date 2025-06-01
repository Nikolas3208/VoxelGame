using SFML.Graphics;
using SFML.System;
using SFML.Window;
using VoxelGame.Item;

namespace VoxelGame.UI.Inventory
{
    /// <summary>
    /// Класс, представляющий интерфейс крафта в игре.
    /// Отображает доступные рецепты крафта и позволяет создавать предметы.
    /// </summary>
    public class UICraft : UIBase
    {
        /// <summary>
        /// Ссылка на инвентарь игрока, используемый для крафта.
        /// </summary>
        private UIInventory _inventory;

        /// <summary>
        /// Список доступных рецептов крафта.
        /// </summary>
        private List<Craft> crafts;

        /// <summary>
        /// Конструктор интерфейса крафта.
        /// </summary>
        /// <param name="inventory">Ссылка на инвентарь игрока.</param>
        public UICraft(UIInventory inventory)
        {
            IsRectVisible = false; // Скрытие прямоугольника интерфейса

            _inventory = inventory;

            crafts = new List<Craft>(); // Инициализация списка рецептов крафта

            // Инициализация области интерфейса крафта
            rect = new RectangleShape(new Vector2f(5, 8) * UIInventoryCell.CellSize);

            // Создание ячеек для отображения рецептов крафта
            for (int x = 0; x < 5; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    Childs.Add(new UICraftCell() { Position = new Vector2f(x, y) * UIInventoryCell.CellSize, Perent = this });
                }
            }
        }

        /// <summary>
        /// Обновление интерфейса крафта.
        /// </summary>
        /// <param name="deltaTime">Время между кадрами.</param>
        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            // Получение доступных рецептов крафта из инвентаря
            if (_inventory != null)
            {
                crafts = _inventory.GetAvailableCrafts();
            }

            // Обновление ячеек крафта, если кнопка мыши не нажата
            if (!Mouse.IsButtonPressed(Mouse.Button.Left))
            {
                foreach (var c in Childs)
                {
                    (c as UICraftCell)?.SetItem(null!); // Очистка ячеек
                }

                for (int i = 0; i < crafts.Count; i++)
                {
                    (Childs[i] as UICraftCell)?.SetCraft(crafts[i]); // Установка рецептов в ячейки
                }
            }
        }

        /// <summary>
        /// Создание предмета по его типу.
        /// </summary>
        /// <param name="itemList">Тип предмета из перечисления <see cref="ItemList"/>.</param>
        /// <returns>Созданный предмет или null, если крафт невозможен.</returns>
        public Item.Item? CraftItem(ItemList itemList)
        {
            var item = Items.GetItem(itemList);
            if (item == null)
                return null;

            var craft = item.Craft;
            if (craft == null)
                return null;

            // Добавление созданного предмета в инвентарь
            if (_inventory.AddItem(item, craft.OutCount))
            {
                // Уменьшение количества необходимых ресурсов в инвентаре
                for (int i = 0; i < craft.Items.Length; i++)
                {
                    var itemStack = _inventory.GetItemStackByItem(craft.Items[i].Item);
                    if (itemStack != null)
                        itemStack.ItemCount -= craft.Items[i].Count;
                }

                return item;
            }

            return null;
        }

        /// <summary>
        /// Создание предмета по его экземпляру.
        /// </summary>
        /// <param name="item">Экземпляр предмета.</param>
        /// <returns>Созданный предмет или null, если крафт невозможен.</returns>
        public Item.Item? CraftItem(Item.Item item)
        {
            if (item == null)
                return null;

            var craft = item.Craft;
            if (craft == null)
                return null;

            // Добавление созданного предмета в инвентарь
            if (_inventory.AddItem(item, craft.OutCount))
            {
                // Уменьшение количества необходимых ресурсов в инвентаре
                for (int i = 0; i < craft.Items.Length; i++)
                {
                    var itemStack = _inventory.GetItemStackByItem(craft.Items[i].Item);
                    if (itemStack != null)
                        itemStack.ItemCount -= craft.Items[i].Count;
                }

                return item;
            }

            return null;
        }
    }
}