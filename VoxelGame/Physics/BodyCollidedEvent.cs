using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoxelGame.Physics
{
    public class BodyCollidedEvent
    {
        public RigidBody Body { get; }

        public Vector2f Normal { get; }

        public float Depth { get; }

        public BodyCollidedEvent(RigidBody body, Vector2f normal, float depth)
        {
            Body = body;
            Normal = normal;
            Depth = depth;
        }
    }
}
