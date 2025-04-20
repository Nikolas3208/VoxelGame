using VoxelGame.Npc;
using VoxelGame.Physics;
using VoxelGame.Worlds;

namespace VoxelGame.Entitys
{
    public enum NpcType
    { 
        None,
        Enemy
    }
    public abstract class Npc : Entity
    {
        public NpcType Type { get; set; }

        public Npc(RigidBody rigidBody, World world, NpcType type) : base(rigidBody, world)
        {
            Type = type;
        }

        public override void Update(float deltaTime)
        {

        }
    }
}
