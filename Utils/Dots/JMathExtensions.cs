using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;

namespace JReact
{
    public static class JMathExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int TimeToFrame(this float time, float framePerSecond)
        {
            int result = (int) math.ceil(time / framePerSecond);
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int ToVector3Int(this int2 value) => new Vector3Int(value.x, value.y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int2 ToInt2(this Vector3Int vector3Int) => new int2(vector3Int.x, vector3Int.y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2 ToFloat2(this Vector3 vector3) => new float2(vector3.x, vector3.y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsZero(this float2 value) => math.all(value == 0);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsAboveZero(this float2 value, float allowed) => math.abs(value.x) > allowed || math.abs(value.y) > allowed;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float EaseOutQuartic(this float t) => 1f - math.pow(1f - t, 4f);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ReverseEaseOutQuartic(this float normalizedValue)
        {
            float clampedValue = math.saturate(normalizedValue);
            return 1f - math.pow(1f - clampedValue, 0.25f);
        }

        // Cubic Hermite arrival at rest; requires remainingTime > 0 and 0 <= deltaTime <= remainingTime.
        public static float2 HermiteToTarget(this float2 position, float2 target, ref float2 velocity,
            float remainingTime, float deltaTime)
        {
            float fraction = deltaTime / remainingTime;
            float rest = 1 - fraction;
            float2 distance = target - position;
            position += distance * (fraction * fraction * (3 - 2 * fraction))
                + velocity * (remainingTime * fraction * rest * rest);
            velocity = distance * (6 * fraction * rest / remainingTime)
                + velocity * (rest * (1 - 3 * fraction));
            return position;
        }

        public static float2 SafeNormalize(this float2 v)
        {
            float l2 = math.lengthsq(v);
            if (l2 < 1e-20f) return new float2(1f, 0f);
            return v * math.rsqrt(l2);
        }

        public static float AngleInRadians(this  float2 vector) => math.atan2(vector.y, vector.x);
        public static float AngleInDegrees(this  float2 vector) => math.degrees(AngleInRadians(vector));
        public static float AngleNormalized(this float2 vector) => AngleInRadians(vector) / math.PI;
        
        public static bool IsWithin(this int value, int min, int max) => value >= min && value <= max;
    }
}
