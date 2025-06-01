using SFML.Graphics;
using SFML.System;
using VoxelGame.Meths;
using VoxelGame.Physics;
using VoxelGame.Resources;
using VoxelGame.Worlds;

namespace VoxelGame.Entitys
{
    public class NpcSlime : Npc
    {
        private const float TIME_WAIT_JUMP = 1f;

        private float waitTimer = 0f;

        public NpcSlime(World world, AABB aabb) : base(world, aabb, NpcType.Enemy)
        {
            rect = new RectangleShape(aabb.Max - aabb.Min);
            rect.Texture = TextureManager.GetTexture("NPC_1");
            rect.TextureRect = new IntRect(0, 0, 32, 26);
            rect.FillColor = new Color((byte)Random.Shared.Next(0, 255), (byte)Random.Shared.Next(0, 255), (byte)Random.Shared.Next(0, 255));

            DropItem = Item.ItemList.Gel;

            Health = 22;
        }

        public override void OnCollided(Entity other, Vector2f normal, float depth)
        {
            base.OnCollided(other, normal, depth);


        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            if (!isFall)
            {
                if (waitTimer >= TIME_WAIT_JUMP)
                {
                    AddVelocity(GetJumpVelocity());
                    waitTimer = 0f;
                    isFall = true;
                }
                else
                {
                    waitTimer += 0.05f;
                    velocity.X = 0f;
                }

                rect.TextureRect = new IntRect(0, 0, 32, 26);
            }
            else
                rect.TextureRect = new IntRect(0, 26, 32, 26);
        }

        public override void Damege(float damage, Vector2f normalAttack)
        {
            base.Damege(damage, normalAttack);

            AddVelocity(GetJumpVelocity(MathHelper.Normalize(normalAttack)));
            waitTimer = 0f;
            isFall = true;

        }

        public override void Attack(IDamageable damageable, Vector2f normalAttack)
        {
            base.Attack(damageable, normalAttack);

            AddVelocity(GetJumpVelocity(MathHelper.Normalize(-normalAttack)));
            waitTimer = 0f;
            isFall = true;
        }

        public override void OnKill()
        {
            base.OnKill();

        }

        public virtual Vector2f GetJumpVelocity()
        {
            return new Vector2f(World.Random.Next(-50, 50), -World.Random.Next(150, 250));
        }

        public virtual Vector2f GetJumpVelocity(Vector2f normal)
        {
            return new Vector2f(World.Random.Next(30, 60) * normal.X, World.Random.Next(75, 150) * normal.Y);
        }
    }
}
