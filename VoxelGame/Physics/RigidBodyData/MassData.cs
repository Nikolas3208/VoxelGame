namespace VoxelGame.Physics.RigidBodyData
{
    public struct MassData
    {
        public readonly float Mass;
        public readonly float InvMass;
        public readonly float Inertia;
        public readonly float InvInertia;

        public MassData(float mass, float inertia)
        {
            Mass = mass;
            InvMass = mass > 0 ? 1f / mass : 0f;
            Inertia = inertia;
            InvInertia = inertia > 0 ? 1f / inertia : 0f;
        }
    }
}
