using UnityEngine;

namespace JMath2D
{
    public static class J_Math2D
    {
        public const float Tolerance = 0.00001f;
        
        public static Vector2 GetPerpendicular(this Vector2 v) => new Vector2(-v.y, v.x);

        public static Vector2 Lerp(Vector2 v1, Vector2 v2, float t)
        {
            t = Mathf.Clamp(t, 0, 1);
            var   v  = new Vector2(v1.x - v2.x, v1.y - v2.y);
            float xt = v1.x + v.x * t;
            float yt = v2.y + v.y * t;

            return new Vector2(xt, yt);
        }
        
        
        static public float Dot(this Vector2 v, Vector2 v2) => (v.x * v2.x + v.y * v2.y);

        public static float Angle(this Vector2 vector1, Vector2 vector2, bool wantRadians = true)
        {
            float dotDivide = Vector2.Dot(vector1, vector2) / (vector1.magnitude * vector2.magnitude);
            float radians   = Mathf.Acos(dotDivide);
            if (wantRadians) return radians;
            return radians * 180 / Mathf.PI;
        }

        public static Vector2 LookAt2D(this Vector2 currentPosition, Vector2 forward, Vector2 target)
        {
            Vector2 direction = new Vector2(target.x - currentPosition.x, target.y - currentPosition.y);
            float   angle     = Angle(forward, direction);
            bool    clockwise = IsOnRight(forward, direction) < 0;

            Vector2 newDir = Rotate(forward, angle, clockwise);
            return newDir;
        }

        public static float IsOnRight(this Vector2 v1, Vector2 v2) => v1.x * v2.y - v1.y * v2.x;

        public static Vector2 Rotate(this Vector2 v, float radians, bool clockwise)
        {
            if (clockwise) { radians = 2 * Mathf.PI - radians; }

            float xVal = v.x * Mathf.Cos(radians) - v.y * Mathf.Sin(radians);
            float yVal = v.x * Mathf.Sin(radians) + v.y * Mathf.Cos(radians);
            return new Vector2(xVal, yVal);
        }
    }
}
