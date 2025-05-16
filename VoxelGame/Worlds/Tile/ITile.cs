using SFML.System;
using VoxelGame.Item;

namespace VoxelGame.Worlds.Tile
{
    public interface ITile
    {
        /// <summary>
        /// Id плитки
        /// </summary>
        int Id { get; set; }

        /// <summary>
        /// Требуемый инструмент
        /// </summary>
        ItemType RequiredTool { get; set; }

        /// <summary>
        /// Требуемаяя мощность инструмента
        /// </summary>
        int RequiredToolPower { get; set; }

        /// <summary>
        /// Выпадаемый предмет, после разрушения
        /// </summary>
        ItemList DropItem { get; set; }

        /// <summary>
        /// Прочность
        /// </summary>
        float Strength { get; set; }

        /// <summary>
        /// Твердый?
        /// </summary>
        bool IsSolid { get; set; }

        /// <summary>
        /// Родительский чанк
        /// </summary>
        Chunk PerentChunk { get; set; }

        /// <summary>
        /// Мировая позиция плитки
        /// </summary>
        Vector2f GlobalPosition { get; set; }

        /// <summary>
        /// Локальная позиция плитки
        /// </summary>
        Vector2f LocalPosition { get; set; }

        /// <summary>
        /// Размер плитки
        /// </summary>
        Vector2f Size { get; set; }

        /// <summary>
        /// Обновляем текстурные координаты
        /// </summary>
        void UpdateView();

        /// <summary>
        /// Лобаем плитку
        /// </summary>
        /// <param name="type"> Тип предмета </param>
        /// <param name="itemPower"> Мошность предмета </param>
        /// <returns> Если здоровье плитки 0 или меньше true </returns>
        bool BreakingTile(ItemType type, float itemPower, float damage);
    }
}
