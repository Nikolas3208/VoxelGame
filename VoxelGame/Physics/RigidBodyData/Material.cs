namespace VoxelGame.Physics.RigidBodyData
{
    public struct Material
    {
        public readonly float Density;
        public readonly float Restitution;
        public readonly float StaticFriction;
        public readonly float DynamicFriction;

        public Material(float density, float restitution, float staticFriction, float dynamicFriction)
        {
            Density = density;
            Restitution = restitution;
            StaticFriction = staticFriction;
            DynamicFriction = dynamicFriction;
        }

        public Material(float density, float restitution)
        {
            Density = density;
            Restitution = restitution;

            StaticFriction = 10.0f;
            DynamicFriction = 10.0f;
        }

        public static Material Default => new Material(1f, 0.2f, 0.8f, 0.2f);
    }
}
