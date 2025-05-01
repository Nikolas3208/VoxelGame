using SFML.Graphics;
using SFML.System;
using VoxelGame.Meths;
using VoxelGame.Worlds;

namespace VoxelGame.Physics
{
    public class PhysicsWorld
    {
        private Vector2f _gravity;

        public PhysicsWorld(Vector2f gravity)
        {
            _gravity = gravity;
        }

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

        public void EntitysStep(float deltaTime, List<Entity> entities)
        {
            // Update all entities in the world
            foreach (var entity in entities)
            {
                entity.Step(deltaTime, _gravity);
            }
        }

        bool CanCollide(Entity a, Entity b)
        {
            return (a.CollidesWith & b.Layer) != 0 && (b.CollidesWith & a.Layer) != 0;
        }

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

        private Vector2f GeBodyMoment(Vector2f normal, Vector2f relativeVeloity)
        {
            float j = -1f * MathHelper.Dot(relativeVeloity, normal);
            j /= 20;

            return j * normal;
        }
    }
}
