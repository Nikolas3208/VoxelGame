using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoxelGame.Entitys.Physics
{
    public class RigidBody
    {
        public Shape Shape;

        public RigidBody() { }
        public RigidBody(Shape shape)
        {
            Shape = shape;
        }

        public void SetShape(Shape shape) => Shape = shape;
        public Shape GetShape() => Shape;
    }
}
