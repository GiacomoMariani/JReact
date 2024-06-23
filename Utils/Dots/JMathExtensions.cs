using Unity.Mathematics;
using UnityEngine;

namespace JReact
{
    public static class JMathExtensions
    {
        public static Vector3Int ToVector3Int(this int2 value) => new Vector3Int(value.x, value.y);

        public static int2   ConvertToInt2(this   Vector3Int vector3Int) => new int2(vector3Int.x, vector3Int.y);
        public static float2 ConvertToFloat2(this Vector3    vector3)    => new float2(vector3.x, vector3.y);
    }
}
