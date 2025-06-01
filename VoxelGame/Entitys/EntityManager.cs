using SFML.Graphics;
using VoxelGame.Physics;
using VoxelGame.Worlds;

namespace VoxelGame.Entitys
{
    /// <summary>  
    /// Класс для управления сущностями в игре.  
    /// Позволяет добавлять, удалять, обновлять и рисовать сущности.  
    /// </summary>  
    public class EntityManager
    {
        /// <summary>  
        /// Максимальное количество сущностей, которое может быть в менеджере.  
        /// </summary>  
        public const int MaxEntityCount = 100;

        /// <summary>  
        /// Ссылка на объект мира, в котором находятся сущности.  
        /// </summary>  
        private World _world;

        /// <summary>  
        /// Список всех сущностей, управляемых этим менеджером.  
        /// </summary>  
        private List<Entity> _entities = new(MaxEntityCount);

        /// <summary>  
        /// Свойство, возвращающее текущее количество сущностей, управляемых менеджером.  
        /// </summary>  
        public int EntityCount => _entities.Count;

        /// <summary>  
        /// Конструктор класса EntityManager.  
        /// Инициализирует пустой список сущностей и устанавливает ссылку на мир.  
        /// </summary>  
        /// <param name="world">Мир, в котором находятся сущности.</param>  
        public EntityManager(World world)
        {
            _world = world;
        }

        /// <summary>  
        /// Добавляет сущность в список.  
        /// Присваивает уникальный идентификатор и устанавливает ссылку на менеджер.  
        /// </summary>  
        /// <param name="entity">Сущность, которую нужно добавить.</param>  
        public void AddEntity(Entity entity)
        {
            if (_entities.Count <= MaxEntityCount)
            {
                entity.Id = _entities.Count; // Присваиваем уникальный ID сущности  
                entity.EntityManager = this; // Устанавливаем ссылку на менеджер сущностей  
                _entities.Add(entity);
            }
        }

        /// <summary>  
        /// Удаляет сущность из списка.  
        /// </summary>  
        /// <param name="entity">Сущность, которую нужно удалить.</param>  
        /// <returns>Возвращает true, если сущность была успешно удалена, иначе false.</returns>  
        public bool RemoveEntity(Entity entity)
        {
            return _entities.Remove(entity);
        }

        /// <summary>  
        /// Удаляет сущность из списка по указанному индексу.  
        /// Проверяет, что индекс находится в пределах допустимого диапазона,  
        /// и выбрасывает исключение, если он выходит за пределы.  
        /// </summary>  
        /// <param name="index">Индекс сущности, которую нужно удалить.</param>  
        /// <returns>Возвращает true, если сущность была успешно удалена.</returns>
        public bool RemoveAtEntity(int index)
        {
            if (index < 0 || index >= _entities.Count)
                throw new ArgumentOutOfRangeException(nameof(index), "Индекс выходит за пределы списка.");
            
            _entities.RemoveAt(index);

            return true;
        }

        /// <summary>  
        /// Получает сущность по индексу.  
        /// </summary>  
        /// <param name="index">Индекс сущности в списке.</param>  
        /// <returns>Сущность, находящаяся по указанному индексу.</returns>  
        /// <exception cref="ArgumentOutOfRangeException">Выбрасывается, если индекс выходит за пределы списка.</exception>  
        public Entity GetEntity(int index)
        {
            if (index < 0 || index >= _entities.Count)
                throw new ArgumentOutOfRangeException(nameof(index), "Индекс выходит за пределы списка.");

            return _entities[index];
        }

        /// <summary>  
        /// Возвращает список сущностей указанного типа.  
        /// Использует LINQ для фильтрации сущностей по типу.  
        /// </summary>  
        /// <typeparam name="T">Тип сущности, которую нужно получить.</typeparam>  
        /// <returns>Список сущностей указанного типа.</returns>  
        public List<T> GetEntitisOfType<T>() where T : Entity
        {
            return _entities.OfType<T>().ToList();
        }

        /// <summary>  
        /// Возвращает список всех сущностей.  
        /// </summary>  
        /// <returns>Список сущностей.</returns>  
        public List<Entity> GetEntities()
        {
            return _entities;
        }

        /// <summary>  
        /// Обновляет все сущности и видимые чанки.  
        /// </summary>  
        /// <param name="deltaTime">Время между кадрами, используемое для обновления.</param>  
        /// <param name="visibleChunk">Список видимых чанков, используемый для обновления сущностей.</param>  
        public void Update(float deltaTime, List<Chunk> visibleChunk)
        {
            foreach (var chunk in visibleChunk)
            {
                var randDouble = Random.Shared.NextDouble(); // Генерация случайного числа от 0 до 1  

                // Проверка на случай, если чанки не загружены  
                if (chunk == null) continue;

                if (randDouble < 0.03) // 3% шанс на обновление чанка  
                {
                    chunk.Update(deltaTime);
                }
            }

            for(int i = 0; i < _entities.Count; i++)
            {
                var entity = _entities[i];

                if (entity == null)
                    continue; // Пропускаем, если сущность равна null

                entity.Update(deltaTime);
            }
        }

        /// <summary>  
        /// Рисует все сущности.  
        /// </summary>  
        /// <param name="target">Цель отрисовки (например, окно).</param>  
        /// <param name="states">Состояния рендера.</param>  
        public void Draw(RenderTarget target, RenderStates states)
        {
            for(int i = 0; i < _entities.Count; i++)
            {
                var entity = _entities[i];
                if (entity == null)
                    continue; // Пропускаем, если сущность равна null

                entity.Draw(target, states);
            }
        }
    }
}
