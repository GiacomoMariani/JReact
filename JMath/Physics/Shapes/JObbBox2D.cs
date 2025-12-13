using System.Runtime.CompilerServices;
using JReact;
using Unity.Assertions;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace JMath2D.JPhysics
{
    public readonly struct JObbBox2D
    {
        private const float kEpsLen2 = 1e-18f;
        private const float kEpsAxis = 1e-12f; 
        private const float kEpsSegLen = 1e-6f;

        public readonly float2 Center; // c
        public readonly float2 X;      // local +x axis (unit)
        public readonly float2 Y;      // local +y axis (unit, orthogonal to X)
        public readonly float2 Half;   // half extents (ex, ey)

        /// <summary> Returns the rotation angle in radians (optional, for debugging/serialization). </summary>
        public float AngleRadians => math.atan2(X.y, X.x);

        public JObbBox2D(float2 center, float2 axisX, float2 axisY, float2 halfExtents)
        {
            Center = center;
            Half   = new float2(math.abs(halfExtents.x), math.abs(halfExtents.y));
            X      = axisX.SafeNormalize();

            float2 yProj = axisY - math.dot(axisY, X) * X;
            float2 perpX = new float2(-X.y, X.x);

            float s = math.lengthsq(yProj) > kEpsLen2 ? math.sign(math.dot(yProj, perpX)) : 1f;
            if (s == 0f) { s = 1f; }

            Y = perpX * s;
        }

        /// <summary>
        /// Construct from a line segment (p0->p1) and half-width (expands sideways).
        /// Useful for thin bars/doors.
        /// </summary>
        public static JObbBox2D FromSegment(float2 p0, float2 p1, float halfWidth)
        {
            float2 d   = p1 - p0;
            float  len = math.length(d);
            if (len <= kEpsSegLen)
            {
                float w = math.abs(halfWidth);
                return new JObbBox2D(p0, new float2(1, 0), new float2(0, 1), new float2(w, w));
            }

            float2 x      = d / len;
            float2 y      = new float2(-x.y, x.x);
            float2 center = (p0 + p1) * 0.5f;
            float2 half   = new float2(len * 0.5f, math.abs(halfWidth));
            return new JObbBox2D(center, x, y, half);
        }

        public static JObbBox2D FromAngle(float2 center, float2 halfExtents, float radians)
        {
            float  cs = math.cos(radians);
            float  sn = math.sin(radians);
            float2 x  = new float2(cs,  sn);
            float2 y  = new float2(-sn, cs);
            return new JObbBox2D(center, x, y, math.abs(halfExtents));
        }

        public static JObbBox2D FromAabb(float2 center, float2 half)
            => new JObbBox2D(center, new float2(1, 0), new float2(0, 1), math.abs(half));

        /// <summary> Four corners in CCW order: (-,-), (+,-), (+,+), (-,+) in local space mapped to world. </summary>
        public void GetCorners(ref NativeArray<float2> result)
        {
            Assert.IsTrue(result.Length >= 4, $"{result.Length} is not enough for {nameof(GetCorners)}");
            float2 ex = X * Half.x;
            float2 ey = Y * Half.y;
            result[0] = Center      - ex - ey;
            result[1] = Center + ex - ey;
            result[2] = Center      + ex + ey;
            result[3] = Center - ex + ey;
        }

        /// <summary> Closest point on the OBB to a world-space point p. </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float2 ClosestPoint(float2 p)
        {
            float2 d = p - Center;
            // coordinates in box local space (no matrix needed)
            float lx = math.dot(d, X);
            float ly = math.dot(d, Y);
            lx = Clamp(lx, -Half.x, Half.x);
            ly = Clamp(ly, -Half.y, Half.y);
            return Center + X * lx + Y * ly;
        }

        /// <summary> Projected radius of this OBB on a world-space axis (unit or not). </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float ProjectedRadius(float2 axis)
        {
            // normalize axis to get true radius
            float len = math.length(axis);
            if (len < kEpsAxis) { return 0f; }

            float2 n = axis / len;
            return Half.x * math.abs(math.dot(X, n)) +
                   Half.y * math.abs(math.dot(Y, n));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float Clamp(float v, float a, float b) => math.max(a, math.min(b, v));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float2 ToLocal(float2 p) => new float2(math.dot(p - Center, X), math.dot(p - Center, Y));

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public float2 ToWorld(float2 lp) => Center + X * lp.x + Y * lp.y;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(float2 p)
        {
            float2 lp = ToLocal(p);
            return math.abs(lp.x) <= Half.x && math.abs(lp.y) <= Half.y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float2 Support(float2 dir)
        {
            float  dx = math.dot(dir, X);
            float  dy = math.dot(dir, Y);
            float2 ex = X * (dx >= 0 ? Half.x : -Half.x);
            float2 ey = Y * (dy >= 0 ? Half.y : -Half.y);
            return Center + ex + ey;
        }

        public float2x2 Rotation => new float2x2(X, Y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float ProjectedRadiusUnitAxis(float2 nUnit)
            => Half.x * math.abs(math.dot(X, nUnit)) + Half.y * math.abs(math.dot(Y, nUnit));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public JObbBox2D Translated(float2 delta) => new JObbBox2D(Center + delta, X, Y, Half);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public JObbBox2D WithCenter(float2 c) => new JObbBox2D(c, X, Y, Half);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public JObbBox2D Rotated(float radians)
        {
            float  cs = math.cos(radians), sn = math.sin(radians);
            float2 x2 = new float2(X.x * cs - X.y * sn, X.x * sn + X.y * cs);
            float2 y2 = new float2(-x2.y,               x2.x);
            return new JObbBox2D(Center, x2, y2, Half);
        }

        public void CopyToTransform(Transform transform)
        {
            float3     worldPosition = new float3(Center.x, Center.y, 0f);
            quaternion worldRotation = quaternion.AxisAngle(new float3(0f, 0f, 1f), AngleRadians);
            float3     worldScale    = new float3(Half.x * 2f, Half.y * 2f, 1f);
            transform.SetPositionAndRotation(worldPosition,
                                             new Quaternion(worldRotation.value.x, worldRotation.value.y, worldRotation.value.z,
                                                            worldRotation.value.w));

            transform.localScale = new Vector3(worldScale.x, worldScale.y, worldScale.z);
        }
    }
}
