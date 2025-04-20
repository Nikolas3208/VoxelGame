using SFML.System;
using System.Numerics;

namespace VoxelGame.Physics.Collision;

public readonly struct Manifold
{
    public readonly RigidBody BodyA;
    public readonly RigidBody BodyB;
    public readonly Vector2f Normal;
    public readonly float Depth;

    public readonly Vector2f Contact1;
    public readonly Vector2f Contact2;
    public readonly int ContactCount;

    public Manifold(RigidBody bodyA, RigidBody bodyB, Vector2f normal, float depth, Vector2f contact1, Vector2f contact2, int contactCount)
    {
        BodyA = bodyA;
        BodyB = bodyB;
        Normal = normal;
        Depth = depth;
        Contact1 = contact1;
        Contact2 = contact2;
        ContactCount = contactCount;
    }
}
