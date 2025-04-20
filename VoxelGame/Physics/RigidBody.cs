using SFML.System;
using VoxelGame.Meths;
using VoxelGame.Physics.Collision;
using VoxelGame.Physics.Collision.Colliders;
using VoxelGame.Physics.RigidBodyData;

namespace VoxelGame.Physics;

[Flags]
public enum CollisionLayer
{
    None = 0,
    Ground = 1 << 0,
    Entity = 1 << 1,
    Item = 1 << 2,
    Enemy = 1 << 3,
    All = Ground | Entity | Item | Enemy
}

public enum BodyType
{
    Static,
    Dynamic,
    Kinematic
}
public class RigidBody
{
    /// <summary>
    /// ������� ������������
    /// </summary>
    /// <param name="e"></param>
    public delegate void OnCollided(BodyCollidedEvent e);
    public event OnCollided Collided;

    /// <summary>
    /// ������� ����
    /// </summary>
    private Vector2f _position;

    /// <summary>
    /// ���� ����������� �� ����
    /// </summary>
    private Vector2f _force;

    /// <summary>
    /// ������� ���� � ��������
    /// </summary>
    private float _rotation;

    /// <summary>
    /// ������� ���� � ��������
    /// </summary>
    private float _digrisRotation;

    /// <summary>
    /// �������� �������� ����
    /// </summary>
    private float _angularVelocity;

    /// <summary>
    /// ���� �������� �����
    /// </summary>
    private float _sin;

    /// <summary>
    /// ���� �������� �������
    /// </summary>
    private float _cos;

    /// <summary>
    /// ��������� ����
    /// </summary>
    private Collider _collider;

    /// <summary>
    /// AABB ����
    /// </summary>
    private AABB _aabb;

    /// <summary>
    /// ������������� ����
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    /// ��������� ����
    /// </summary>
    public Vector2f Position
    {
        get => _position;
        set => _position = value;
    }

    /// <summary>
    /// �������� �������� ����
    /// </summary>
    public Vector2f LinearVelocity { get; set; }

    /// <summary>
    /// ������� ���� � ��������
    /// </summary>
    public float Rotation
    {
        get => _rotation;
        set
        {
            _rotation = value;
            _digrisRotation = value * (MathF.PI / 180);
            _sin = MathF.Sin(_digrisRotation);
            _cos = MathF.Cos(_digrisRotation);
        }
    }

    /// <summary>
    /// ������� ���� � ��������
    /// </summary>
    public float DigrisRotation { get => _digrisRotation; }

    /// <summary>
    /// ������� �������� ����
    /// </summary>
    public float AngularVelocity
    {
        get => _angularVelocity;
        set
        {
            if (!FreezeRotation)
                _angularVelocity = value;
        }
    }

    /// <summary>
    /// ��� ����������
    /// </summary>
    public ColliderType ColliderType { get => _collider.Type; }

    /// <summary>
    /// ��� ����
    /// </summary>
    public BodyType Type { get; set; } = BodyType.Dynamic;

    /// <summary>
    /// ���� ��������
    /// </summary>
    public CollisionLayer Layer { get; set; } = CollisionLayer.None;

    /// <summary>
    /// ���� � ������� ����� ����������� ��������
    /// </summary>
    public CollisionLayer CollidesWith { get; set; } = CollisionLayer.None;

    /// <summary>
    /// ���������� � ����� ����
    /// </summary>
    public MassData MassData { get; private set; }

    /// <summary>
    /// �������� ����
    /// </summary>
    public Material Material { get; private set; }

    /// <summary>
    /// ��������� �������� ����
    /// </summary>
    public bool FreezeRotation { get; set; } = false;

    private RigidBody(ColliderType colliderType, BodyType type, float density, float restitution)
    {
        Type = type;
        ColliderType = colliderType;

        density = MathHelper.Clamp(density, PhysicsWorld.MinDensity, PhysicsWorld.MaxDensity);
        restitution = MathHelper.Clamp(restitution, 0, 1);

        Material = new Material(density, restitution);

        FreezeRotation = type == BodyType.Static ? true : false;

        Id = Guid.NewGuid();
    }

    public RigidBody(Polygon polygon, float density, float restitution, BodyType type = BodyType.Dynamic) : this(ColliderType.Poligon, type, density, restitution)
    {
        _sin = MathF.Sin(0);
        _cos = MathF.Cos(0);

        _polygon = polygon;

        float area = MathHelper.GetAreaRegularPolygon(polygon) / 100;
        float mass = area * Material.Density;

        float inertia = 1f / 12 * mass * (polygon.Size.X * polygon.Size.X + polygon.Size.Y * polygon.Size.Y);

        MassData = new MassData(mass, inertia);
    }

    public RigidBody(Circle circle, float density, float restitution, BodyType type = BodyType.Dynamic) : this(ColliderType.Circle, type, density, restitution)
    {
        _circle = circle;

        float area = circle.Radius * circle.Radius * MathF.PI / 1000;
        float mass = area * Material.Density;

        float inertia = 1f / 2 * mass * circle.Radius * circle.Radius;

        MassData = new MassData(mass, inertia);
    }

    /// <summary>
    /// ��� ������
    /// </summary>
    /// <param name="deltaTime"> ����� ����� </param>
    /// <param name="gravity"> ���� ��������� </param>
    /// <param name="iterations"> ����������� �������� </param>
    public void Step(float deltaTime, Vector2f gravity, int iterations)
    {
        if (Type is BodyType.Static)
            return;

        float time = deltaTime / iterations;

        LinearVelocity += (_force + (gravity * MassData.Mass)) * time;
        Position += LinearVelocity * time;

        Rotation += AngularVelocity * MassData.Mass * time;

        _force = new Vector2f();
    }

    /// <summary>
    /// ���������� ��� ������������ � ������ �����
    /// </summary>
    /// <param name="e"> ������� ������������ </param>
    public void OnCollidedBody(BodyCollidedEvent e)
    {
        Collided?.Invoke(e);
    }

    /// <summary>
    /// �������� ���� �� �������� ������
    /// </summary>
    /// <param name="offset"> ������ ����������� </param>
    public void Move(Vector2f offset)
    {
        if (Type is BodyType.Static) return;

        Position += offset;
    }

    /// <summary>
    /// �������� ���� � �������� �������
    /// </summary>
    /// <param name="position"> ������ </param>
    public void MoveTo(Vector2f position)
    {
        if (Type is BodyType.Static) return;

        Position = position;
    }

    /// <summary>
    /// ��������� ���� �� �������� ����
    /// </summary>
    /// <param name="angleOffset"> ���� �������� </param>
    public void Rotate(float angleOffset)
    {
        if (Type is BodyType.Static) return;

        Rotation += angleOffsets;
    }

    /// <summary>
    /// ��������� ���� � �������� ����
    /// </summary>
    /// <param name="angle"> ����� ���� ���� </param>
    public void RotateTo(float angle)
    {
        if (Type is BodyType.Static) return;

        Rotation = angle;
    }

    /// <summary>
    /// ��������� ���� � ����
    /// </summary>
    /// <param name="force"> ������ ���������� � ���� </param>
    public void AddForce(Vector2f force)
    {
        if (Type is BodyType.Static) return;

        _force += force;
    }

    /// <summary>
    /// �������� �������� ��������
    /// </summary>
    /// <param name="velocity"> �������� </param>
    public void AddLinearVelosity(Vector2f velocity)
    {
        if (Type is BodyType.Static) return;

        LinearVelocity += velocity;
    }

    /// <summary>
    /// �������� ������� ��������
    /// </summary>
    /// <param name="angularVelocity"> ������� �������� </param>
    public void AddAngularVelosity(float angularVelocity)
    {
        if (Type is BodyType.Static) return;

        AngularVelocity += angularVelocity;
    }

    /// <summary>
    /// �������� AABB ����
    /// </summary>
    /// <returns> AABB </returns>
    public AABB GetAABB()
    {
        return _aabb;
    }

    /// <summary>
    /// �������� ��������� ����
    /// </summary>
    /// <returns> Collider </returns>
    public Collider GetCollider()
    {
        return _collider.Transform(_position);
    }
}
