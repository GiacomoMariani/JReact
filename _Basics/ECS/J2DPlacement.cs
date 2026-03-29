#if UNITY_DOTS
using System.Runtime.CompilerServices;
using JMath2D;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace JReact
{
    public struct J2DPlacement
    {
        public float2 Position;

        private float _rotation;
        public float Rotation { readonly get => _rotation; set => _rotation = value.WrapRadians(); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public readonly float3 Position3D(float z = 0f) => new float3(Position, z);

        // If you need multiple direction ops, reuse this instead of calling Rotate() repeatedly.
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public readonly float2x2 RotationMatrix() => float2x2.Rotate(_rotation);

        // Unity-like convention: local +X is "Right", local +Y is "Up"
        public readonly float2 Right => math.mul(RotationMatrix(), new float2(1f, 0f));
        public readonly float2 Left => -Right;

        public readonly float2 Up => math.mul(RotationMatrix(), new float2(0f, 1f));
        public readonly float2 Down => -Up;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly LocalTransform ToLocalTransform(float z = 0f, float scale = 1f)
            => LocalTransform.FromPositionRotationScale(new float3(Position.x, Position.y, z),
                                                        quaternion.RotateZ(_rotation),
                                                        scale);

        /// <summary>Unity.Mathematics quaternion around +Z (radians).</summary>
        public readonly quaternion QRotation => quaternion.RotateZ(_rotation);

        /// <summary>
        /// Non-uniform XY scale as a PostTransformMatrix (use with LocalTransform).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly PostTransformMatrix ToPostTransformMatrix(float2 scale2D)
            => new PostTransformMatrix { Value = float4x4.Scale(new float3(scale2D.x, scale2D.y, 1f)) };

        /// <summary>Unity-standard: local offset under this transform => Position + (Rotation * localOffset)</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly float2 PositionWithOffset(float2 localOffset)
        {
            float2x2 r = float2x2.Rotate(_rotation); // sin/cos once
            return Position + math.mul(r, localOffset);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static J2DPlacement FromLocalTransform(in LocalTransform lt)
        {
            quaternion q    = lt.Rotation;
            float      sinZ = 2f * (q.value.w * q.value.z + q.value.x * q.value.y);
            float      cosZ = 1f - 2f * (q.value.y * q.value.y + q.value.z * q.value.z);
            float      rotZ = math.atan2(sinZ, cosZ);

            return new J2DPlacement { Position = new float2(lt.Position.x, lt.Position.y), Rotation = rotZ };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static J2DPlacement FromTransform(Transform t) => new()
        {
            Position = new float2(t.position.x, t.position.y), Rotation = math.radians(t.eulerAngles.z)
        };
    }
    
    public static class J2DTransformExtensions
    {
        /// <summary>
        /// Sets Transform position (XY) and Z rotation from J2DTransform. Scale is unchanged.
        /// </summary>
        public static void SetFromJ2D(this Transform t, in J2DPlacement data, float z = 0f)
        {
            t.position = new Vector3(data.Position.x, data.Position.y, z);
            t.rotation = Quaternion.Euler(0f, 0f, math.degrees(data.Rotation));
        }

        /// <summary>
        /// Same as SetFromJ2D but preserves the Transform's current Z position.
        /// </summary>
        public static void SetFromJ2DPreserveZ(this Transform t, in J2DPlacement data)
        {
            float z = t.position.z;
            t.position = new Vector3(data.Position.x, data.Position.y, z);
            t.rotation = Quaternion.Euler(0f, 0f, math.degrees(data.Rotation));
        }

        /// <summary>
        /// Rotates this transform so its Up (+Y) turns toward the given world-space direction,
        /// limited by maxRadiansPerSecond * fixedDeltaTime. Returns the applied step (radians).
        /// </summary>
        public static J2DPlacement RotateUpTowards(this J2DPlacement t, float2 directionNormalized, float maxRadiansPerSecond,
                                                       float                 fixedDeltaTime)
        {
            float2 up = t.Up;

            float cross = up.x * directionNormalized.y - up.y * directionNormalized.x;
            float dot   = up.x * directionNormalized.x + up.y * directionNormalized.y;
            float delta = math.atan2(cross, dot);

            float maxStep = maxRadiansPerSecond * fixedDeltaTime;
            float step    = math.clamp(delta, -maxStep, maxStep);

            t.Rotation += step;
            return t;
        }

        /// <summary>UnityEngine Quaternion for this rotation (keep it out of the component to avoid Burst misuse).</summary>
        public static Quaternion Rotation3D(this in J2DPlacement data) => Quaternion.Euler(0f, 0f, math.degrees(data.Rotation));
    }
}
#endif
