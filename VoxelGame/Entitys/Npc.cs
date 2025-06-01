using SFML.Graphics;
using SFML.System;
using VoxelGame.Graphics.Particles;
using VoxelGame.Item;
using VoxelGame.Meths;
using VoxelGame.Physics;
using VoxelGame.Resources;
using VoxelGame.Worlds;

namespace VoxelGame.Entitys
{
    public enum NpcType
    {
        None,
        Enemy,
        Friendly,
        Player
    }

    public class Npc : Entity, IDamageable, IAttack
    {
        protected ParticleSystem particleSystem;

        protected string hitSoundName = "NPC_Hit_";
        protected string killSoundName = "NPC_Killed_";

        protected int hitSound = 1;
        protected int killSound = 1;

        public NpcType NpcType { get; set; } = NpcType.None;
        public ItemList DropItem { get; set; } = ItemList.None;

        public float Health { get; set; } = 100;
        public float MaxHealth { get; set; } = 100;
        public float Armor { get; set; } = 3;
        public float Damage { get; set; } = 5;

        public bool Kill = false;

        public float RepulsiveForce { get; set; } = 5;

        public Npc(World world, AABB aabb, NpcType type) : base(world, aabb)
        {
            NpcType = type;
            particleSystem = new ParticleSystem();
        }

        public override void Update(float deltaTime)
        {
            particleSystem.Update(deltaTime);
        }

        public override void OnCollided(Entity other, Vector2f normal, float depth)
        {
            base.OnCollided(other, normal, depth);

            DebugRender.AddVector(Position + Size / 2, normal, Color.Red, 50);

            if (other is IDamageable damageable && other is Npc npc)
            {
                if (npc.NpcType == NpcType)
                    return;

                if(NpcType == NpcType.Enemy)
                    Attack(damageable, MathHelper.Normalize(npc.Position - Position));
            }
        }

        public virtual void Damege(float damage, Vector2f normalAttack)
        {
            float d = damage - Armor;

            if (d <= 0)
                d = 1;

            particleSystem.AddParticle(new Particle(d.ToString(), TextureManager.GetFont("Arial"), Color.Red, lifeTime: 0.1f, timeStep: 0.2f));
            particleSystem.Position = Position - new Vector2f(0, Size.Y);

            Health -= d;

            AudioManager.PlaySuond($"{hitSoundName}{hitSound}");

            if (Health <= 0)
                OnKill();
        }

        public virtual void PushAway(Vector2f dir)
        {
            Position += new Vector2f(dir.X, -dir.Y) * RepulsiveForce;
        }

        public virtual void OnKill()
        {
            if (NpcType != NpcType.Player)
            {
                world.RemoveEntity(this);
                world.DropItem(DropItem, Position);
            }

            AudioManager.PlaySuond($"{killSoundName}{killSound}");
        }

        public virtual void Attack(IDamageable damageable, Vector2f normalAttack)
        {
            damageable.Damege(Damage, normalAttack);

            DebugRender.AddVector(Position + Size / 2, normalAttack, Color.Black, 50);

            damageable.PushAway(normalAttack);
        }

        public override void Draw(RenderTarget target, RenderStates states)
        {
            base.Draw(target, states);

            particleSystem.Draw(target, states);
        }
    }
}
