using SFML.System;
using VoxelGame.Meths;
using VoxelGame.Physics.Collision;
using VoxelGame.Physics.Collision.Colliders;

namespace VoxelGame.Physics;

public class PhysicsWorld
{
    public static float MinDensity = 0.1f;
    public static float MaxDensity = 22.0f;

    public static int MinIterations = 1;
    public static int MaxIterations = 128;

    private List<RigidBody> _bodies = [];
    private List<(int, int)> _contactPairs = [];
    private Vector2f _gravity = new Vector2f();

    private Vector2f[] _contactList = new Vector2f[2];
    private Vector2f[] _impulseList = new Vector2f[2];
    private Vector2f[] _frictionImpulse = new Vector2f[2];
    private Vector2f[] _raList = new Vector2f[2];
    private Vector2f[] _rbList = new Vector2f[2];
    private float[] _jList = new float[2];

    public PhysicsWorld()
    {
        _bodies = new List<RigidBody>();
        _contactPairs = new List<(int, int)>();
        _gravity = new Vector2f(0, 9.81f);
    }

    public RigidBody CreateBody(Polygon polygon, float density, float restitution, BodyType type = BodyType.Dynamic)
    {
        var body = new RigidBody(polygon, density, restitution, type);
        _bodies.Add(body);

        return body;
    }

    public RigidBody CreateBody(Circle circle, float density, float restitution, BodyType type = BodyType.Dynamic)
    {
        var body = new RigidBody(circle, density, restitution, type);
        _bodies.Add(body);

        return body;
    }

    public bool AddBody(RigidBody body)
    {
        if (body == null) throw new ArgumentNullException("Body is null.");

        if (!_bodies.Where(b => b.Id == body.Id).Any())
        {
            _bodies.Add(body);
            return true;
        }

        return false;
    }

    public void AddBodyRenge(List<RigidBody> rigidBodies)
    {
        _bodies.AddRange(rigidBodies);
    }

    public RigidBody? GetRigidBody(Guid id)
    {
        return _bodies.FirstOrDefault(b => b.Id == id);
    }

    public List<RigidBody> GetBodies() => _bodies;

    public void Step(float time, int iterations)
    {
        iterations = (int)MathHelper.Clamp(iterations, PhysicsWorld.MinIterations, PhysicsWorld.MaxIterations);

        for (int it = 0; it < iterations; it++)
        {
            _contactPairs.Clear();
            BodiesStep(time, iterations, _bodies);
            BroadPhase(_bodies);
            NarrowPhase(_bodies);
        }
    }

    public void Step(float time, int iterations, List<RigidBody> bodies)
    {
        iterations = (int)MathHelper.Clamp(iterations, PhysicsWorld.MinIterations, PhysicsWorld.MaxIterations);

        for (int it = 0; it < iterations; it++)
        {
            _contactPairs.Clear();
            BodiesStep(time, iterations, bodies);
            BroadPhase(bodies);
            NarrowPhase(bodies);
        }
    }

    private void BroadPhase(List<RigidBody> _bodies)
    {
        for (int i = 0; i < _bodies.Count - 1; i++)
        {
            RigidBody bodyA = _bodies[i];
            AABB aabbA = bodyA.GetAABB();

            for (int j = i + 1; j < _bodies.Count; j++)
            {
                RigidBody bodyB = _bodies[j];
                AABB aabbB = bodyB.GetAABB();

                if (bodyA.Type is BodyType.Static && bodyB.Type is BodyType.Static)
                    continue;

                if (!CollisionDetected.AABBsIntersect(aabbA, aabbB))
                    continue;

                _contactPairs.Add((i, j));
            }
        }
    }

    private void NarrowPhase(List<RigidBody> _bodies)
    {
        for (int i = 0; i < _contactPairs.Count; i++)
        {
            (int, int) pair = _contactPairs[i];
            RigidBody bodyA = _bodies[pair.Item1];
            RigidBody bodyB = _bodies[pair.Item2];

            if (CollisionDetected.OnCollisionDetected(bodyA, bodyB, out Vector2f normal, out float depth))
            {
                bodyA.OnCollidedBody(new BodyCollidedEvent(bodyB, normal, depth));
                bodyB.OnCollidedBody(new BodyCollidedEvent(bodyA, normal, depth));

                if(!CanCollide(bodyA, bodyB))
                    continue;

                SeparateBodies(bodyA, bodyB, normal, depth);

                CollisionDetected.OnContactsDetected(bodyA, bodyB, out Vector2f contact1, out Vector2f contact2, out int contactCount);

                Manifold contact = new Manifold(bodyA, bodyB, normal, depth, contact1, contact2, contactCount);

                ResolveCollisionBase(contact);
                ApplayFriction(contact);
            }

        }
    }

    bool CanCollide(RigidBody a, RigidBody b)
    {
        return (a.CollidesWith & b.Layer) != 0 && (b.CollidesWith & a.Layer) != 0;
    }

    private void BodiesStep(float time, int iterations, List<RigidBody> _bodies)
    {
        for (int i = 0; i < _bodies.Count; i++)
        {
            if (_bodies[i].Type is BodyType.Static)
                continue;

            _bodies[i].Step(time, _gravity, iterations);
        }
    }

    private void SeparateBodies(RigidBody bodyA, RigidBody bodyB, Vector2f normal, float depth)
    {
        var mtv = normal * depth;

        if (bodyA.Type is BodyType.Static)
        {
            bodyB.Move(mtv);
        }
        else if (bodyB.Type is BodyType.Static)
        {
            bodyA.Move(-mtv);
        }
        else
        {
            bodyA.Move(-mtv / 2f);
            bodyB.Move(mtv / 2f);
        }
    }

    private void ResolveCollisionBase(Manifold contact)
    {
        RigidBody bodyA = contact.BodyA;
        RigidBody bodyB = contact.BodyB;
        Vector2f normal = contact.Normal;

        Vector2f relativeVelocity = bodyB.LinearVelocity - bodyA.LinearVelocity;

        float contacVelosityMag = MathHelper.Dot(relativeVelocity, normal);

        if (contacVelosityMag > 0f)
        {
            return;
        }

        float e = MathF.Min(bodyA.Material.Restitution, bodyB.Material.Restitution);

        Vector2f impulse = GeBodyMoment(normal, relativeVelocity, e, bodyA.MassData.InvMass, bodyB.MassData.InvMass);

        bodyA.AddLinearVelosity(-impulse * bodyA.MassData.InvMass);
        bodyB.AddLinearVelosity(impulse * bodyB.MassData.InvMass);
    }

    private void ApplayFriction(Manifold contact)
    {
        RigidBody bodyA = contact.BodyA;
        RigidBody bodyB = contact.BodyB;
        Vector2f normal = contact.Normal;

        Vector2f relativeVelocity = bodyB.LinearVelocity - bodyA.LinearVelocity;

        float contacVelosityMag = MathHelper.Dot(relativeVelocity, normal);

        if (contacVelosityMag > 0f)
        {
            return;
        }

        var tangent = relativeVelocity - contacVelosityMag * normal;
        tangent = MathHelper.Normalize(tangent);

        float e = MathF.Min(bodyA.Material.Restitution, bodyB.Material.Restitution);

        Vector2f impulse = GeBodyMoment(tangent, relativeVelocity, e, bodyA.MassData.InvMass, bodyB.MassData.InvMass);

        bodyA.AddForce(-impulse * bodyA.MassData.InvMass);
        bodyB.AddForce(impulse * bodyB.MassData.InvMass);
    }

    private Vector2f GeBodyMoment(Vector2f normal, Vector2f relativeVeloity, float e, float invMassA, float invMassB)
    {
        float j = -(1f + e) * MathHelper.Dot(relativeVeloity, normal);
        j /= invMassA + invMassB;

        return j * normal;
    }

    private void ResolveCollisionWhithRotation(Manifold contact)
    {
        RigidBody bodyA = contact.BodyA;
        RigidBody bodyB = contact.BodyB;
        Vector2f normal = contact.Normal;
        Vector2f contact1 = contact.Contact1;
        Vector2f contact2 = contact.Contact2;
        int contactCount = contact.ContactCount;

        float e = MathF.Min(bodyA.Material.Restitution, bodyB.Material.Restitution);

        this._contactList[0] = contact1;
        this._contactList[1] = contact2;

        Vector2f centerA = bodyA.ColliderType == ColliderType.Poligon ? bodyA.GetPolygon().Center : bodyA.GetCircle().Center;
        Vector2f centerB = bodyB.ColliderType == ColliderType.Poligon ? bodyB.GetPolygon().Center : bodyB.GetCircle().Center;

        for (int i = 0; i < contactCount; i++)
        {
            this._impulseList[i] = new Vector2f();
            this._raList[i] = new Vector2f();
            this._rbList[i] = new Vector2f();
        }

        for (int i = 0; i < contactCount; i++)
        {
            Vector2f ra = _contactList[i] - centerA;
            Vector2f rb = _contactList[i] - centerB;

            _raList[i] = ra;
            _rbList[i] = rb;

            Vector2f raPerp = new Vector2f(-ra.Y, ra.X);
            Vector2f rbPerp = new Vector2f(-rb.Y, rb.X);

            Vector2f angularLinearVelocityA = raPerp * bodyA.AngularVelocity;
            Vector2f angularLinearVelocityB = rbPerp * bodyB.AngularVelocity;

            Vector2f relativeVelocity =
                (bodyB.LinearVelocity + angularLinearVelocityB) -
                (bodyA.LinearVelocity + angularLinearVelocityA);

            float contactVelocityMag = MathHelper.Dot(relativeVelocity, normal);

            if (contactVelocityMag > 0f)
            {
                continue;
            }

            float raPerpDotN = MathHelper.Dot(raPerp, normal);
            float rbPerpDotN = MathHelper.Dot(rbPerp, normal);

            float denom = bodyA.MassData.InvMass + bodyB.MassData.InvMass +
                (raPerpDotN * raPerpDotN) * bodyA.MassData.InvInertia +
                (rbPerpDotN * rbPerpDotN) * bodyB.MassData.InvInertia;

            float j = -(1f + e) * contactVelocityMag;
            j /= denom;
            j /= (float)contactCount;

            Vector2f impulse = j * normal;
            _impulseList[i] = impulse;
        }

        for (int i = 0; i < contactCount; i++)
        {
            Vector2f impulse = _impulseList[i];
            Vector2f ra = _raList[i];
            Vector2f rb = _rbList[i];

            bodyA.AddLinearVelosity(-impulse * bodyA.MassData.InvMass);
            bodyB.AddLinearVelosity(impulse * bodyB.MassData.InvMass);

            bodyA.AddAngularVelosity(-MathHelper.Cross(ra, impulse) * bodyA.MassData.InvInertia);
            bodyB.AddAngularVelosity(MathHelper.Cross(rb, impulse) * bodyB.MassData.InvInertia);
        }
    }

    private void ResolveCollisionWhithRotationAndFriction(Manifold contact)
    {
        RigidBody bodyA = contact.BodyA;
        RigidBody bodyB = contact.BodyB;
        Vector2f normal = contact.Normal;
        Vector2f contact1 = contact.Contact1;
        Vector2f contact2 = contact.Contact2;
        int contactCount = contact.ContactCount;

        float e = MathF.Min(bodyA.Material.Restitution, bodyB.Material.Restitution);

        float sf = (bodyA.Material.StaticFriction + bodyB.Material.StaticFriction) * 0.5f;
        float df = (bodyA.Material.DynamicFriction + bodyB.Material.DynamicFriction) * 0.5f;

        _contactList[0] = contact1;
        _contactList[1] = contact2;

        Vector2f centerA = bodyA.ColliderType == ColliderType.Poligon ? bodyA.GetPolygon().Center : bodyA.GetCircle().Center;
        Vector2f centerB = bodyB.ColliderType == ColliderType.Poligon ? bodyB.GetPolygon().Center : bodyB.GetCircle().Center;

        for (int i = 0; i < contactCount; i++)
        {
            _impulseList[i] = new Vector2f();
            _raList[i] = new Vector2f();
            _rbList[i] = new Vector2f();
        }

        for (int i = 0; i < contactCount; i++)
        {
            Vector2f ra = _contactList[i] - centerA;
            Vector2f rb = _contactList[i] - centerB;

            _raList[i] = ra;
            _rbList[i] = rb;

            Vector2f raPerp = new Vector2f(-ra.Y, ra.X);
            Vector2f rbPerp = new Vector2f(-rb.Y, rb.X);

            Vector2f angularLinearVelocityA = raPerp * bodyA.AngularVelocity;
            Vector2f angularLinearVelocityB = rbPerp * bodyB.AngularVelocity;

            Vector2f relativeVelocity =
                (bodyB.LinearVelocity + angularLinearVelocityB) -
                (bodyA.LinearVelocity + angularLinearVelocityA);

            float contactVelocityMag = MathHelper.Dot(relativeVelocity, normal);

            if (contactVelocityMag > 0f)
            {
                continue;
            }

            float raPerpDotN = MathHelper.Dot(raPerp, normal);
            float rbPerpDotN = MathHelper.Dot(rbPerp, normal);

            float denom = bodyA.MassData.InvMass + bodyB.MassData.InvMass +
                (raPerpDotN * raPerpDotN) * bodyA.MassData.InvInertia +
                (rbPerpDotN * rbPerpDotN) * bodyB.MassData.InvInertia;

            float j = -(1f + e) * contactVelocityMag;
            j /= denom;
            j /= (float)contactCount;

            _jList[i] = j;

            Vector2f impulse = j * normal;
            _impulseList[i] = impulse;
        }

        for (int i = 0; i < contactCount; i++)
        {
            Vector2f impulse = _impulseList[i];
            Vector2f ra = _raList[i];
            Vector2f rb = _rbList[i];

            bodyA.AddLinearVelosity(-impulse * bodyA.MassData.InvMass);
            bodyB.AddLinearVelosity(impulse * bodyB.MassData.InvMass);

            bodyA.AddAngularVelosity(-MathHelper.Cross(ra, impulse) * bodyA.MassData.InvInertia);
            bodyB.AddAngularVelosity(MathHelper.Cross(rb, impulse) * bodyB.MassData.InvInertia);
        }

        for (int i = 0; i < contactCount; i++)
        {
            Vector2f ra = _contactList[i] - bodyA.Position;
            Vector2f rb = _contactList[i] - bodyB.Position;

            _raList[i] = ra;
            _rbList[i] = rb;

            Vector2f raPerp = new Vector2f(-ra.Y, ra.X);
            Vector2f rbPerp = new Vector2f(-rb.Y, rb.X);

            Vector2f angularLinearVelocityA = raPerp * bodyA.AngularVelocity;
            Vector2f angularLinearVelocityB = rbPerp * bodyB.AngularVelocity;

            Vector2f relativeVelocity =
                (bodyB.LinearVelocity + angularLinearVelocityB) -
                (bodyA.LinearVelocity + angularLinearVelocityA);

            Vector2f tangent = relativeVelocity - MathHelper.Dot(relativeVelocity, normal) * normal;

            if (MathHelper.NearlyEqual(tangent, new Vector2f()))
            {
                continue;
            }
            else
            {
                tangent = MathHelper.Normalize(tangent);
            }

            float raPerpDotT = MathHelper.Dot(raPerp, tangent);
            float rbPerpDotT = MathHelper.Dot(rbPerp, tangent);

            float denom = bodyA.MassData.InvMass + bodyB.MassData.InvMass +
                (raPerpDotT * raPerpDotT) * bodyA.MassData.InvInertia +
                (rbPerpDotT * rbPerpDotT) * bodyB.MassData.InvInertia;

            float jt = -MathHelper.Dot(relativeVelocity, tangent);
            jt /= denom;
            jt /= (float)contactCount;

            Vector2f frictionImpulse;
            float j = _jList[i];

            if (MathF.Abs(jt) <= j * sf)
            {
                frictionImpulse = jt * tangent;
            }
            else
            {
                frictionImpulse = -j * tangent * df;
            }

            _frictionImpulse[i] = frictionImpulse;
        }

        for (int i = 0; i < contactCount; i++)
        {
            Vector2f frictionImpulse = _frictionImpulse[i];
            Vector2f ra = _raList[i];
            Vector2f rb = _rbList[i];

            bodyA.AddForce(-frictionImpulse * bodyA.MassData.InvMass);
            bodyA.AddAngularVelosity(-MathHelper.Cross(ra, frictionImpulse) * bodyA.MassData.InvInertia);
            bodyB.AddForce(frictionImpulse * bodyB.MassData.InvMass);
            bodyB.AddAngularVelosity(MathHelper.Cross(rb, frictionImpulse) * bodyB.MassData.InvInertia);
        }
    }
}
