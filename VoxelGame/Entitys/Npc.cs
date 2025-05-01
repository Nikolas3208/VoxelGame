using VoxelGame.Physics;
using VoxelGame.Worlds;

namespace VoxelGame.Entitys
{
    public enum NpcType
    { 
        None,
        Player,
        Enemy
    }
    public abstract class Npc : Entity
    {
        public NpcType Type { get; set; }

        public Npc(World world, AABB aabb, NpcType type) : base(world, aabb)
        {
            Type = type;
        }

        public override void Update(float deltaTime)
        {

        }
    }
}
