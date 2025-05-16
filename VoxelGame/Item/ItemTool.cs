namespace VoxelGame.Item
{
    public class ItemTool : Item
    {
        public float Power { get; set; } = 1.0f;
        public float BreakingSpeed => Speed * Power;

        /// <summary>
        /// Предмет инструмент
        /// </summary>
        /// <param name="type"> Тип предмета </param>
        /// <param name="description"> Описание </param>
        /// <param name="strength"> Прочность </param>
        /// <param name="speed"> Скорость </param>
        /// <param name="damage"> Урон </param>
        public ItemTool(ItemType type, string description, float strength, float speed, float power,
            float damage) 
            : base(type, type.ToString(), description, strength, damage, 1)
        {
            Power = power;
            Speed = speed;
        }
    }
}
