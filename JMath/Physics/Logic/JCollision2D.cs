#if UNITY_DOTS
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine.Assertions;

namespace JMath2D.JPhysics
{
    public readonly struct JCollision2D
    {
        // Squared distance threshold below which we treat the two centers as coincident.
        private readonly float _kEpsilonDistanceSquared;
        public JCollision2D(float kEpsilonDistanceSquared = 1e-12f) { _kEpsilonDistanceSquared = kEpsilonDistanceSquared; }

        /// <summary>
        /// Circle–circle overlap. Returns a unit normal pointing from circleA to circleB,
        /// and a positive penetration depth.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Overlaps(in  JCircle2D circleA, in JCircle2D circleB, out float2 normalFromAToB, out float penetrationDepth,
                             out float2    contactPoint)
        {
            contactPoint     = normalFromAToB = default;
            penetrationDepth = 0f;
            // Reject degenerate circles early
            if (circleA.Radius <= 0f ||
                circleB.Radius <= 0f) { return false; }

            float2 centerDelta           = circleB.Center - circleA.Center;
            float  centerDistanceSquared = math.lengthsq(centerDelta);
            float  combinedRadius        = circleA.Radius + circleB.Radius;
            float  combinedRadiusSquared = combinedRadius * combinedRadius;

            // Separated?
            if (centerDistanceSquared > combinedRadiusSquared) { return false; }

            if (centerDistanceSquared > _kEpsilonDistanceSquared)
            {
                float inverseDistance = math.rsqrt(centerDistanceSquared);
                float centerDistance  = 1f / inverseDistance;
                normalFromAToB   = centerDelta * inverseDistance;
                penetrationDepth = math.max(0f, combinedRadius - centerDistance);
            }
            else
            {
                normalFromAToB   = new float2(1f, 0f);
                penetrationDepth = combinedRadius;
            }

            contactPoint = circleA.Center + normalFromAToB * circleA.Radius;
            return true;
        }

        public bool Overlaps(in  JCircle2D circle, in JObbBox2D box,
                             out float2    normalFromCircleToBox,
                             out float     penetrationDepth,
                             out float2    contactPoint)
        {
            normalFromCircleToBox = default;
            penetrationDepth      = 0f;
            contactPoint          = default;

            Assert.IsTrue(circle.Radius > 0f);
            // circle center relative to box
            float2 relativeCenter = circle.Center - box.Center;

            // circle center in box local space
            float2 local;
            local.x = math.dot(relativeCenter, box.X);
            local.y = math.dot(relativeCenter, box.Y);

            // closest point in local space
            float2 closestLocalSpacePoint = math.clamp(local, -box.Half, box.Half);

            bool inside =
                math.abs(local.x - closestLocalSpacePoint.x) < float.Epsilon &&
                math.abs(local.y - closestLocalSpacePoint.y) < float.Epsilon;

            float2 closestWorld = box.Center + box.X * closestLocalSpacePoint.x + box.Y * closestLocalSpacePoint.y;

            float2 diff   = closestWorld - circle.Center;
            float  distSq = math.lengthsq(diff);
            float  r      = circle.Radius;
            float  rSq    = r * r;

            if (!inside)
            {
                if (distSq > rSq) { return false; }

                if (distSq > 1e-12f)
                {
                    float invDist = math.rsqrt(distSq);
                    normalFromCircleToBox = diff * invDist;
                    penetrationDepth      = r - (1f / invDist);
                }
                else
                {
                    // almost same point
                    normalFromCircleToBox = new float2(1f, 0f);
                    penetrationDepth      = r;
                }

                contactPoint = circle.Center + normalFromCircleToBox * r;
                return true;
            }
            else
            {
                // circle center is INSIDE the box: choose nearest face in LOCAL space
                float distToPosX = box.Half.x - local.x;       // to +X face  ( +half.x )
                float distToNegX = local.x    - (-box.Half.x); // to -X face  ( -half.x )
                float distToPosY = box.Half.y - local.y;
                float distToNegY = box.Half.y + local.y;

                // find smallest
                float  minDist = distToPosX;
                float2 normal  = box.X; // +X

                if (distToNegX < minDist)
                {
                    minDist = distToNegX;
                    normal  = -box.X;
                }

                if (distToPosY < minDist)
                {
                    minDist = distToPosY;
                    normal  = box.Y;
                }

                if (distToNegY < minDist)
                {
                    minDist = distToNegY;
                    normal  = -box.Y;
                }

                normalFromCircleToBox = normal;
                // need to clear both the remaining space to the face and the circle radius
                penetrationDepth = r             + minDist;
                contactPoint     = circle.Center + normalFromCircleToBox * r;
                return true;
            }
        }

        /// <summary>
        /// Circle–circle overlap with contact points on both circles along the collision normal.
        /// normalFromAToB points from circleA to circleB. penetrationDepth is positive.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Overlaps(in  JCircle2D circleA,
                             in  JCircle2D circleB,
                             out float2    normalFromAToB,
                             out float     penetrationDepth,
                             out float2    contactPointOnA,
                             out float2    contactPointOnB)
        {
            if (!Overlaps(in circleA, in circleB, out normalFromAToB, out penetrationDepth, out _))
            {
                contactPointOnA = default;
                contactPointOnB = default;
                return false;
            }

            contactPointOnA = circleA.Center + normalFromAToB * circleA.Radius; // on A toward B
            contactPointOnB = circleB.Center - normalFromAToB * circleB.Radius; // on B toward A
            return true;
        }

        /// <summary>
        /// Boolean-only intersection test (no normals/depth).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Intersects(in JCircle2D circleA, in JCircle2D circleB)
        {
            if (circleA.Radius <= 0f ||
                circleB.Radius <= 0f) return false;

            float2 centerDelta           = circleB.Center - circleA.Center;
            float  centerDistanceSquared = math.lengthsq(centerDelta);
            float  combinedRadius        = circleA.Radius + circleB.Radius;

            return centerDistanceSquared <= combinedRadius * combinedRadius;
        }
    }
}
#endif
