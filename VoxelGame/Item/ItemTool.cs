namespace VoxelGame.Item
{
    public class ItemTool : Item
    {
        /// <summary>
        /// Предмет инструмент
        /// </summary>
        /// <param name="type"> Тип предмета </param>
        /// <param name="description"> Описание </param>
        /// <param name="strength"> Прочность </param>
        /// <param name="speed"> Скорость </param>
        /// <param name="damage"> Урон </param>
        /// <param name="spriteIndex"> Идентефикатор спрайта на листе </param>
        public ItemTool(ItemType type, string description, float strength, float speed,
            float damage, int spriteIndex) 
            : base(type, type.ToString(), description, strength, damage, 
                  spriteIndex, 1)
        {
            Speed = speed;
        }
    }
}
