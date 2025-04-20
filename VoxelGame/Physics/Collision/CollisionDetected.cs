using SFML.System;
using System.Numerics;
using VoxelGame.Meths;
using VoxelGame.Physics.Collision.Colliders;

namespace VoxelGame.Physics.Collision;

public static class CollisionDetected
{
    public static void OnContactsDetected(RigidBody rbA, RigidBody rbB, out Vector2f cp1, out Vector2f cp2, out int contactCount)
    {
        cp1 = new Vector2f();
        cp2 = new Vector2f();
        contactCount = 0;

        if (rbA.ColliderType == ColliderType.Poligon)
        {
            if (rbB.ColliderType == ColliderType.Poligon)
            {
                OnPoligonContactDetected(
                    rbA.GetPolygon(),
                    rbB.GetPolygon(),
                    out cp1, out cp2, out contactCount);
            }
            else if (rbB.ColliderType == ColliderType.Circle)
            {
                OnCirclePoligonContactDetected(
                    rbB.GetCircle(),
                    rbA.GetPolygon(),
                    out cp1);
                contactCount = 1;
            }
        }
        else if (rbA.ColliderType == ColliderType.Circle)
        {
            if (rbB.ColliderType == ColliderType.Poligon)
            {
                OnCirclePoligonContactDetected(
                    rbA.GetCircle(),
                    rbB.GetPolygon(),
                    out cp1);
                contactCount = 1;
            }
            if (rbB.ColliderType == ColliderType.Circle)
            {
                OnCircleContactDetected(
                    rbA.GetCircle(),
                    rbB.GetCircle(),
                    out cp1);
                contactCount = 1;
            }
        }
    }

    private static void OnPoligonContactDetected(Polygon pA, Polygon pB, out Vector2f cp1, out Vector2f cp2, out int contactCount)
    {
        cp1 = new Vector2f();
        cp2 = new Vector2f();
        contactCount = 0;

        var minDistSq = float.MaxValue;

        for (int i = 0; i < pA.GetVertexCount(); i++)
        {
            var p = pA.GetVertices()[i];

            for (int j = 0; j < pB.GetVertexCount(); j++)
            {
                Vector2f va = pB.GetVertex(j);
                Vector2f vb = pB.GetVertex((j + 1) % pB.GetVertexCount());

                CollisionDetected.PointSegmentDistance(p, va, vb, out float distanceSquared, out Vector2f contact);

                if (MathHelper.NearlyEqual(distanceSquared, minDistSq))
                {
                    if (!MathHelper.NearlyEqual(contact, cp1))
                    {
                        cp2 = contact;
                        contactCount = 2;
                    }
                }
                else if (distanceSquared < minDistSq)
                {
                    minDistSq = distanceSquared;
                    cp1 = contact;
                    contactCount = 1;
                }
            }
        }

        for (int i = 0; i < pB.GetVertexCount(); i++)
        {
            var p = pB.GetVertex(i);

            for (int j = 0; j < pA.GetVertexCount(); j++)
            {
                Vector2f va = pA.GetVertex(j); ;
                Vector2f vb = pA.GetVertex((j + 1) % pA.GetVertexCount());

                CollisionDetected.PointSegmentDistance(p, va, vb, out float distanceSquared, out Vector2f contact);

                if (MathHelper.NearlyEqual(distanceSquared, minDistSq))
                {
                    if (!MathHelper.NearlyEqual(contact, cp1))
                    {
                        cp2 = contact;
                        contactCount = 2;
                    }
                }
                else if (distanceSquared < minDistSq)
                {
                    minDistSq = distanceSquared;
                    cp1 = contact;
                    contactCount = 1;
                }
            }
        }
    }

    private static void OnCirclePoligonContactDetected(Circle circle, Polygon poligon, out Vector2f cp)
    {
        cp = new Vector2f();

        var minDistSq = float.MaxValue;

        for (int i = 0; i < poligon.GetVertexCount(); i++)
        {
            Vector2f va = poligon.GetVertices()[i];
            Vector2f vb = poligon.GetVertices()[(i + 1) % poligon.GetVertexCount()];

            CollisionDetected.PointSegmentDistance(circle.Center, va, vb, out float distanceSquared, out Vector2f contact);

            if (distanceSquared < minDistSq)
            {
                minDistSq = distanceSquared;
                cp = contact;
            }
        }
    }

    private static void PointSegmentDistance(Vector2f p, Vector2f a, Vector2f b, out float distanceSquared, out Vector2f contact)
    {
        distanceSquared = 0;
        contact = new Vector2f();

        var ab = b - a;
        var ap = p - a;

        var proj = MathHelper.Dot(ap, ab);
        var abLenSq = MathHelper.LengthSquared(ab);
        var d = proj / abLenSq;

        if (d <= 0)
        {
            contact = a;
        }
        else if (d >= 1)
        {
            contact = b;
        }
        else
        {
            contact = a + ab * d;
        }

        distanceSquared = MathHelper.DistanceSquared(p, contact);
    }

    private static void OnCircleContactDetected(Circle circleA, Circle circleB, out Vector2f cp)
    {
        cp = new Vector2f();

        var ab = circleB.Center - circleA.Center;
        var dir = MathHelper.Normalize(ab);

        cp = dir * circleA.Radius + circleA.Center;

    }
    public static bool OnCollisionDetected(RigidBody rbA, RigidBody rbB, out Vector2f normal, out float depth)
    {
        normal = new Vector2f();
        depth = 0;

        ProjectCircle(new Vector2f(7, 9), 2, new Vector2f(0, -1), out float minB, out float maxB);

        var vertices = new List<Vector2f>();
        vertices.Add(new Vector2f(4, 10));
        vertices.Add(new Vector2f(4, 12));

        ProjectVertices(vertices, new Vector2f(0, -1), out float minA, out float maxA);

        var axisDepth = MathF.Min(maxB - minA, maxA - minB);

        var dir = new Vector2f(10 - 7, 9 - 11);
        var dot = MathHelper.Dot(dir, new Vector2f(0, -1));

        if (rbA.ColliderType == ColliderType.Poligon)
        {
            if (rbB.ColliderType == ColliderType.Poligon)
            {
                return PoligonIntersect(
                    rbA.GetPolygon(),
                    rbB.GetPolygon(),
                    out normal, out depth);
            }
            else if (rbB.ColliderType == ColliderType.Circle)
            {
                return PoligonCircleIntersect(
                    rbA.GetPolygon(),
                    rbB.GetCircle(),
                    out normal, out depth);
            }
        }
        else if (rbA.ColliderType == ColliderType.Circle)
        {
            if (rbB.ColliderType == ColliderType.Poligon)
            {
                bool result = PoligonCircleIntersect(
                    rbB.GetPolygon(),
                    rbA.GetCircle(),
                    out normal, out depth);

                normal = -normal;
                return result;
            }
            if (rbB.ColliderType == ColliderType.Circle)
            {
                return CircleIntersect(
                    rbA.GetCircle(),
                    rbB.GetCircle(),
                    out normal, out depth);
            }
        }

        return false;
    }

    public static bool PoligonCircleIntersect(Polygon poligon, Circle circle, out Vector2f normal, out float depth)
    {
        normal = new Vector2f();
        depth = float.MaxValue;

        Vector2f axis = new Vector2f();
        float axisDepth = 0f;
        float minA, maxA, minB, maxB;

        var vertices = poligon.GetVertices();

        for (int i = 0; i < vertices.Count; i++)
        {
            Vector2f va = vertices[i];
            Vector2f vb = vertices[(i + 1) % vertices.Count];

            Vector2f edge = vb - va;
            axis = new Vector2f(-edge.Y, edge.X);
            axis = MathHelper.Normalize(axis);

            CollisionDetected.ProjectVertices(vertices, axis, out minA, out maxA);
            CollisionDetected.ProjectCircle(circle.Center, circle.Radius, axis, out minB, out maxB);

            if (minA >= maxB || minB >= maxA)
            {
                return false;
            }

            axisDepth = MathF.Min(maxB - minA, maxA - minB);

            if (axisDepth < depth)
            {
                depth = axisDepth;
                normal = axis;
            }
        }

        int cpIndex = CollisionDetected.FindClosestPointOnPolygon(circle.Center, vertices.ToArray());
        if (cpIndex > 0)
        {
            Vector2f cp = vertices[cpIndex];

            axis = circle.Center - cp;
            axis = MathHelper.Normalize(axis);

            CollisionDetected.ProjectVertices(vertices, axis, out minA, out maxA);
            CollisionDetected.ProjectCircle(circle.Center, circle.Radius, axis, out minB, out maxB);

            if (minA >= maxB || minB >= maxA)
            {
                return false;
            }

            axisDepth = MathF.Min(maxB - minA, maxA - minB);

            if (axisDepth < depth)
            {
                depth = axisDepth;
            }
        }

        depth /= MathHelper.Length(normal);
        normal = MathHelper.Normalize(normal);

        Vector2f polygonCenter = CollisionDetected.FindArithmeticMean(vertices);

        Vector2f direction = circle.Center - polygonCenter;

        if (MathHelper.Dot(direction, normal) < 0)
        {
            normal = -normal;
        }

        return true;
    }

    private static Vector2f FindDirection(Vector2f normal, Vector2f circleCenter, Vector2f polygonCenter, List<Vector2f> vertices)
    {
        var direction = new Vector2f();

        if (normal.Y != 0)
        {
            for (int i = 0; i < vertices.Count; i++)
            {
                if (vertices[i].Y < circleCenter.Y)
                {
                    direction = new Vector2f(0, circleCenter.Y - polygonCenter.Y);
                    break;
                }
                else
                {
                    direction = new Vector2f(0, polygonCenter.Y - circleCenter.Y);
                }
            }
        }
        if (normal.X != 0)
        {
            for (int i = 0; i < vertices.Count; i++)
            {
                if (vertices[i].X < circleCenter.X)
                    direction = new Vector2f(circleCenter.X - polygonCenter.X, direction.Y);
                else
                {
                    direction = new Vector2f(polygonCenter.X - circleCenter.X, direction.Y);
                }
            }
        }

        return direction;
    }


    private static int FindClosestPointOnPolygon(Vector2f circleCenter, Vector2f[] vertices)
    {
        int result = -1;
        float minDistance = float.MaxValue;

        for (int i = 0; i < vertices.Length; i++)
        {
            Vector2f v = vertices[i];
            float distance = MathHelper.Distance(v, circleCenter);

            if (distance < minDistance)
            {
                minDistance = distance;
                result = i;
            }
        }

        return result;
    }

    private static void ProjectCircle(Vector2f center, float radius, Vector2f axis, out float min, out float max)
    {
        Vector2f direction = MathHelper.Normalize(axis);
        Vector2f directionAndRadius = direction * radius;

        Vector2f p1 = center + directionAndRadius;
        Vector2f p2 = center - directionAndRadius;

        min = MathHelper.Dot(p1, axis);
        max = MathHelper.Dot(p2, axis);

        if (min > max)
        {
            // swap the min and max values.
            float t = min;
            min = max;
            max = t;
        }
    }

    public static bool PoligonIntersect(Polygon pA, Polygon pB, out Vector2f normal, out float depth)
    {
        normal = new Vector2f();
        depth = float.MaxValue;

        for (int i = 0; i < pA.GetVertexCount(); i++)
        {
            Vector2f va = pA.GetVertices()[i];
            Vector2f vb = pA.GetVertices()[(i + 1) % pA.GetVertexCount()];

            Vector2f edge = vb - va;
            Vector2f axis = new Vector2f(-edge.Y, edge.X);

            CollisionDetected.ProjectVertices(pA.GetVertices(), axis, out float minA, out float maxA);
            CollisionDetected.ProjectVertices(pB.GetVertices(), axis, out float minB, out float maxB);

            if (minA >= maxB || minB >= maxA)
                return false;

            float axisDepth = MathF.Min(maxB - minA, maxA - minB);
            if (axisDepth < depth)
            {
                depth = axisDepth;
                normal = axis;
            }
        }


        for (int i = 0; i < pB.GetVertexCount(); i++)
        {
            Vector2f va = pB.GetVertices()[i];
            Vector2f vb = pB.GetVertices()[(i + 1) % pB.GetVertexCount()];

            Vector2f edge = vb - va;
            Vector2f axis = new Vector2f(-edge.Y, edge.X);

            CollisionDetected.ProjectVertices(pA.GetVertices(), axis, out float minA, out float maxA);
            CollisionDetected.ProjectVertices(pB.GetVertices(), axis, out float minB, out float maxB);

            if (minA >= maxB || minB >= maxA)
                return false;

            float axisDepth = MathF.Min(maxB - minA, maxA - minB);
            if (axisDepth < depth)
            {
                depth = axisDepth;
                normal = axis;
            }
        }

        depth /= MathHelper.Length(normal);
        normal = MathHelper.Normalize(normal);

        Vector2f direction = pB.Center - pA.Center;

        if (MathHelper.Dot(direction, normal) < 0)
        {
            normal = -normal;
        }

        return true;
    }

    private static void ProjectVertices(List<Vector2f> points, Vector2f axis, out float min, out float max)
    {
        min = float.MaxValue;
        max = float.MinValue;

        for (int i = 0; i < points.Count; i++)
        {
            Vector2f v = points[i];
            float proj = MathHelper.Dot(v, axis);

            if (proj < min) min = proj;
            if (proj > max) max = proj;
        }
    }

    public static Vector2f FindArithmeticMean(List<Vector2f> vertices)
    {
        float sumX = 0f;
        float sumY = 0f;

        for (int i = 0; i < vertices.Count; i++)
        {
            Vector2f v = vertices[i];
            sumX += v.X;
            sumY += v.Y;
        }

        return new Vector2f(sumX, sumY) / vertices.Count;
    }

    public static bool CircleIntersect(Circle cA, Circle cB, out Vector2f normal, out float depth)
    {
        normal = new Vector2f();
        depth = 0;

        float distance = MathHelper.Distance(cA.Center, cB.Center);
        float radii = cA.Radius + cB.Radius;

        if (distance >= radii)
            return false;

        normal = MathHelper.Normalize(cB.Center - cA.Center);
        depth = radii - distance;

        return true;
    }

    public static bool AABBsIntersect(AABB a, AABB b)
    {
        if (a.Max.X <= b.Min.X || b.Max.X <= a.Min.X
        || a.Max.Y <= b.Min.Y || b.Max.Y <= a.Min.Y)
            return false;

        return true;
    }
}
