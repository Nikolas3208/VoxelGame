using SFML.Graphics;
using SFML.System;
using VoxelGame.Meths;
using VoxelGame.Worlds;

namespace VoxelGame.Physics
{
    public class PhysicsWorld
    {
        /// <summary>
        /// Сила гравитаии и направление
        /// </summary>
        private Vector2f _gravity;

        /// <summary>
        /// Физический мир
        /// </summary>
        /// <param name="gravity"></param>
        public PhysicsWorld(Vector2f gravity)
        {
            _gravity = gravity;
        }

        /// <summary>
        /// Шаг физического мира
        /// </summary>
        /// <param name="deltaTime"> Время кадра </param>
        /// <param name="entities"> Список сушностей </param>
        /// <param name="chunks"> Список чанков </param>
        public void Step(float deltaTime, List<Entity> entities, List<Chunk> chunks)
        {
            // Update all entities in the world
            EntitysStep(deltaTime, entities);
            // Check for collisions
            for (int i = 0; i < entities.Count; i++)
            {
                var entity = entities[i];

                for (int j = 0; j < chunks.Count; j++)
                {
                    var chunk = chunks[j];

                    // Check if the entity is colliding with the chunk
                    if (entity.GetAABB().Intersect(chunk.GetAABB()))
                    {
                        var colliders = chunk.GetColliders();

                        // Check for collisions with other entities
                        foreach (var collider in colliders)
                        {
                            if (entity.GetAABB().Intersect(collider, out var normal, out var depth))
                            {
                                // Handle collision
                                entity.Move(normal * depth);

                                ResolveCollisionWhithEntityAndChunk(entity, normal, depth);
                                entity.OnCollided(null, normal, depth);

                                entity.Color = Color.Black;
                            }
                            else
                            {
                                entity.Color = Color.White;
                            }
                        }
                    }
                }

                // Check for collisions with other entities
                for(int j = i + 1; j < entities.Count; j++)
                {
                    var otherEntity = entities[j];

                    if (entity.GetAABB().Intersect(otherEntity.GetAABB(), out var normal, out var depth))
                    {
                        // Handle collision
                        if (CanCollide(entity, otherEntity))
                        {
                            entity.Move(normal * depth);
                            otherEntity.Move(-normal * depth);
                            ResolveCollisionWhithEntitys(entity, otherEntity, normal, depth);
                        }
                        entity.OnCollided(otherEntity, normal, depth);
                        otherEntity.OnCollided(entity, -normal, depth);
                    }
                }
            }
        }

        /// <summary>
        /// Шаг сушностей
        /// </summary>
        /// <param name="deltaTime"> Время кадра </param>
        /// <param name="entities"> Список сушностей </param>
        public void EntitysStep(float deltaTime, List<Entity> entities)
        {
            // Update all entities in the world
            foreach (var entity in entities)
            {
                entity.Step(deltaTime, _gravity);
            }
        }

        /// <summary>
        /// Могут ли эти сушности столкнуться
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        bool CanCollide(Entity a, Entity b)
        {
            return (a.CollidesWith & b.Layer) != 0 && (b.CollidesWith & a.Layer) != 0;
        }

        /// <summary>
        /// Разрешение колиззи между сушностью и чанком
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="normal"></param>
        /// <param name="depth"></param>
        private void ResolveCollisionWhithEntityAndChunk(Entity entity, Vector2f normal, float depth)
        {
            Vector2f relativeVelocity = entity.GetVelocity();

            float contacVelosityMag = MathHelper.Dot(relativeVelocity, normal);

            if (contacVelosityMag > 0f)
            {
                return;
            }

            Vector2f impulse = GeBodyMoment(normal, relativeVelocity);

            entity.AddVelocity(impulse * 0.142f * 30);

            var tangent = relativeVelocity - contacVelosityMag * normal;
            tangent = MathHelper.Normalize(tangent);

            impulse = GeBodyMoment(tangent, relativeVelocity);

            entity.AddVelocity(impulse * 0.142f);
        }

        /// <summary>
        /// Разрешение коллизии между сушностями
        /// </summary>
        /// <param name="entityA"></param>
        /// <param name="entityB"></param>
        /// <param name="normal"></param>
        /// <param name="depth"></param>
        private void ResolveCollisionWhithEntitys(Entity entityA, Entity entityB, Vector2f normal, float depth)
        {
            Vector2f relativeVelocity = entityB.GetVelocity() - entityA.GetVelocity();

            float contacVelosityMag = MathHelper.Dot(relativeVelocity, normal);

            if (contacVelosityMag > 0f)
            {
                return;
            }

            Vector2f impulse = GeBodyMoment(normal, relativeVelocity);

            entityA.AddVelocity(-impulse * 0.142f * 30);
            entityB.AddVelocity(impulse * 0.142f * 30);
        }

        /// <summary>
        /// Получить момент тела
        /// </summary>
        /// <param name="normal"></param>
        /// <param name="relativeVeloity"></param>
        /// <returns></returns>
        private Vector2f GeBodyMoment(Vector2f normal, Vector2f relativeVeloity)
        {
            float j = -1f * MathHelper.Dot(relativeVeloity, normal);
            j /= 20;

            return j * normal;
        }
    }
}
