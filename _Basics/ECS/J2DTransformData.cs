#if UNITY_DOTS
using System.Runtime.CompilerServices;
using Unity.Assertions;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace JReact
{
    public struct J2DTransformData : IComponentData
    {
        public J2DPlacement placement2D;

        public readonly float2 Position => placement2D.Position;
        public readonly float Rotation => placement2D.Rotation;
        public float2 PositionWithOffset(float2 offset) => placement2D.PositionWithOffset(offset);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public J2DTransformData SetPosition(float2 position)
        {
            placement2D.Position = position;
            return new J2DTransformData { placement2D = placement2D  };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public J2DTransformData SetRotation(float rotation)
        {
            placement2D.Rotation = rotation;
            return new J2DTransformData { placement2D = placement2D };
        }
        
       /// <summary>
        /// Rotates this transform so its Up (+Y) turns toward the given world-space direction,
        /// limited by maxRadiansPerSecond * fixedDeltaTime. Returns the updated J2DTransformData.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public J2DTransformData RotateUpTowards(float2 directionNormalized, float maxRadiansPerSecond,
                                                       float fixedDeltaTime)
        {
            J2DPlacement rotated = placement2D.RotateUpTowards(directionNormalized, maxRadiansPerSecond, fixedDeltaTime);
            return new J2DTransformData { placement2D = rotated };
        }

        /// <summary>UnityEngine Quaternion for this rotation (keep it out of the component to avoid Burst misuse).</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Quaternion Rotation3D() => Quaternion.Euler(0f, 0f, math.degrees(placement2D.Rotation));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static J2DTransformData FromLocalTransform(in LocalTransform lt)
        {
            J2DPlacement data = J2DPlacement.FromLocalTransform(lt);
            return new J2DTransformData { placement2D = data };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static J2DTransformData FromTransform(Transform t)
        {
            J2DPlacement placement2D = J2DPlacement.FromTransform(t);
            return new J2DTransformData { placement2D = placement2D };
        }
    }
}
#endif
