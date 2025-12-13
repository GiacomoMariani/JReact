using Unity.Mathematics;

namespace JMath2D
{
    public static class J_2dMathAnglesExtensions
    {
        const float TAU = 2f * math.PI;

        // ---------- Core angle from vector ----------
        // Radians in (-π, π], 0 at +Y, increasing clockwise
        public static float Radians(this float2 v) => math.atan2(v.x, v.y);

        // Degrees in (-180, 180], 0 at +Y, increasing clockwise
        public static float Degrees(this float2 v) => math.degrees(Radians(v));

        // ---------- Wrapped to [0, 2π) / [0, 360) ----------
        public static float Radians0To2Pi(this float2 v)
        {
            float a = Radians(v);
            return a < 0f ? a + TAU : a;
        }

        public static float Degrees0To360(this float2 v) => math.degrees(Radians0To2Pi(v));

        public static float ToRadians(this float degrees) => math.radians(degrees);
        public static float ToDegrees(this float radians) => math.degrees(radians);
        
        // --------------- SAFE OPTION --------------- //
        public static float RadiansSafe(this float2 v)
        {
            if (math.lengthsq(v) == 0f) { return -1f; }

            return Radians(v);
        }

        public static float DegreesSafe(this float2 v)
        {
            var angle = RadiansSafe(v);
            if (angle == -1f) { return -1f; }

            return math.degrees(angle);
        }

        // ---------- Differences (shortest turn) ----------
        // Returns signed smallest difference b - a in radians, in (-π, π]
        // Positive value => rotate clockwise from a to reach b (since our angles increase CW).
        public static float DeltaRadians(this float a, float b)
        {
            float d = math.fmod(b - a, TAU);
            switch (d)
            {
                case <= -math.PI: d += TAU; break;
                case > math.PI:   d -= TAU; break;
            }

            return d;
        }

        // Degrees version in (-180, 180]
        public static float DeltaDegrees(this float aDeg, float bDeg)
        {
            float d = math.fmod(bDeg - aDeg, 360f);
            switch (d)
            {
                case <= -180f: d += 360f; break;
                case > 180f:   d -= 360f; break;
            }

            return d;
        }
        
        // Signed smallest angle from a -> b in (-π, π], 0 at +Y, positive = clockwise
        public static float SignedDeltaRadians(this float2 a, float2 b)
        {
            // atan2(cross, dot) is CCW+, so negate to make CW+
            float cross = a.x * b.y - a.y * b.x;          // z-component of 2D cross
            float dot   = a.x * b.x + a.y * b.y;
            float ccw   = math.atan2(cross, dot); // CCW-positive
            float cw    = -ccw;                   // CW-positive
            // Normalize to (-π, π]
            if (cw      <= -math.PI) cw += TAU;
            else if (cw > math.PI) cw   -= TAU;
            return cw;
        }

        // Degrees version in (-180, 180]
        public static float SignedDeltaDegrees(this float2 a, float2 b)
            => math.degrees(SignedDeltaRadians(a, b));
        
        public static quaternion RotationAlignUpToDir(this float2 dir)
        {
            if (math.lengthsq(dir) < 1e-12f) { return quaternion.identity; } 
            float angleCW = math.atan2(dir.x, dir.y);                        // 0 at +Y, CW+
            return quaternion.RotateZ(-angleCW);                             // Unity quats are CCW+, so negate
        }
    }
}
