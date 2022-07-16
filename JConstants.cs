using Unity.Mathematics;
using UnityEngine;

namespace JReact
{
    public class JConstants
    {
        public static string EmptyString = "";
        public const string LineBreak = "\n";

        public const float VeryHighFloatTolerance = .5f;
        public const float HighFloatTolerance = .1f;
        public const float GeneralFloatTolerance = .01f;
        public const float LowFloatTolerance = .001f;
        public const float VeryLowFloatTolerance = .0001f;

        public const float HoursInDay = 24f;
        public const float MinutesInHour = 60f;
        public const float SecondsInMinute = 60f;

        public static readonly Vector2 Vector2Zero = new Vector2(0f, 0f);
        public static readonly Vector2 Vector2One = new Vector2(1f,  1f);

        public static readonly float2 Float2Zero = new float2(0f, 0f);
        public static readonly float2 Float2One = new float2(1f,  1f);

        public static readonly float4 Float4Zero = new float4(0, 0, 0, 0);

        public static readonly Vector2Int Vector2IntZero = new Vector2Int(0, 0);
        public static readonly Vector2Int Vector2IntOne = new Vector2Int(1,  1);
        public static readonly Quaternion quarterionIdentity = new Quaternion();
    }
}
