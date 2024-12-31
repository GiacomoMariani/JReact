using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;

namespace JReact
{
    public static class JMathExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int ToVector3Int(this int2 value) => new Vector3Int(value.x, value.y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int2   ToInt2(this   Vector3Int vector3Int) => new int2(vector3Int.x, vector3Int.y);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2 ToFloat2(this Vector3    vector3)    => new float2(vector3.x, vector3.y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsZero(this      float2 value)                => math.all(value == 0);
        
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
    }
}
