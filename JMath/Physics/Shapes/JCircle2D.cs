using System.Runtime.CompilerServices;
using Unity.Mathematics;

namespace JMath2D.JPhysics
{
    public readonly struct JCircle2D
    {
        public readonly float2 Center;
        public readonly float Radius;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public JCircle2D(float2 center, float radius)
        {
            Center = center;
            Radius = math.abs(radius);
        }

        public float2 Min => Center - new float2(Radius);
        public float2 Max => Center + new float2(Radius);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(float2 p) => math.lengthsq(p - Center) <= Radius * Radius;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float2 ClosestPoint(float2 p)
        {
            float2 vectorToCenter = p - Center;
            float  distanceSqr    = math.lengthsq(vectorToCenter);
            if (distanceSqr <= 1e-12f) { return Center + new float2(Radius, 0f); }

            float inv = math.rsqrt(distanceSqr);
            return Center + vectorToCenter * inv * Radius;
        }
    }
}
