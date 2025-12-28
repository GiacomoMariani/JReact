#if UNITY_DOTS
using System.Runtime.CompilerServices;
using Unity.Mathematics;

namespace JMath2D.JPhysics
{
    public readonly struct JAabbBox2D
    {
        public readonly float xMin;
        public readonly float xMax;
        public readonly float yMin;
        public readonly float yMax;

        // ---- Derived properties (computed from min/max) ----
        public float2 Min { [MethodImpl( MethodImplOptions.AggressiveInlining)] get => new float2(xMin,        yMin); }
        public float2 Max { [MethodImpl( MethodImplOptions.AggressiveInlining)] get => new float2(xMax,        yMax); }
        public float2 Size { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => new float2(xMax - xMin, yMax - yMin); }
        public float2 Half { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => Size * 0.5f; }
        public float2 Center
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] get => new float2((xMin + xMax) * 0.5f, (yMin + yMax) * 0.5f);
        }
        public float Area { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => math.max(0f, Size.x) * math.max(0f, Size.y); }

        public float Width => xMax  - xMin;
        public float Height => yMax - yMin;
        public bool IsValid => (xMin <= xMax) & (yMin <= yMax);

        // --------------- CONSTRUCTORS --------------- //
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public JAabbBox2D(float xMin, float yMin, float xMax, float yMax)
        {
            float2 mn = math.min(new float2(xMin, yMin), new float2(xMax, yMax));
            float2 mx = math.max(new float2(xMin, yMin), new float2(xMax, yMax));
            this.xMin = mn.x;
            this.yMin = mn.y;
            this.xMax = mx.x;
            this.yMax = mx.y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static JAabbBox2D FromMinMax(float2 min, float2 max) => new JAabbBox2D(min.x, min.y, max.x, max.y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static JAabbBox2D FromCenterHalf(float2 center, float2 half)
        {
            float2 h  = math.abs(half);
            float2 mn = center - h;
            float2 mx = center + h;
            return new JAabbBox2D(mn.x, mn.y, mx.x, mx.y);
        }

        // Tile-aligned (optional origin)
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static JAabbBox2D FromTile(int2 tile, float2 cellSize, float2 origin = default)
        {
            float2 min = origin + (float2)tile * cellSize;
            float2 max = min    + cellSize;
            return FromMinMax(min, max);
        }

        // --------------- PHYSICS QUERIES --------------- //
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(float2 p) => (p.x >= xMin) & (p.x <= xMax) & (p.y >= yMin) & (p.y <= yMax);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float2 ClosestPoint(float2 p)
        {
            float x = math.clamp(p.x, xMin, xMax);
            float y = math.clamp(p.y, yMin, yMax);
            return new float2(x, y);
        }

        // Fast boolean overlap (AABB vs AABB)
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Overlaps(in JAabbBox2D other) => !(xMax       < other.xMin ||
                                                       other.xMax < xMin       ||
                                                       yMax       < other.yMin ||
                                                       other.yMax < yMin);

        // MTV (normal from THIS -> other, depth scalar)
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Overlaps(in JAabbBox2D other, out float2 normal, out float depth)
        {
            float2 cA = Center, cB = other.Center;
            float2 hA = Half,   hB = other.Half;

            float2 d   = cB - cA;
            float2 ad  = math.abs(d);
            float2 sum = hA + hB;

            float2 overlap = sum - ad;
            if (overlap.x <= 0f ||
                overlap.y <= 0f)
            {
                normal = default;
                depth  = 0f;
                return false;
            }

            if (overlap.x < overlap.y)
            {
                depth  = overlap.x;
                normal = new float2(math.sign(d.x != 0 ? d.x : 1f), 0f);
            }
            else
            {
                depth  = overlap.y;
                normal = new float2(0f, math.sign(d.y != 0 ? d.y : 1f));
            }

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2 PenetrationVector(float2 normal, float depth) => normal * depth;

        public bool Raycast(float2 origin, float2 dir, float tMax, out float tEnter, out float2 hitNormal)
        {
            float2 inv = 1f             / math.max(math.abs(dir), new float2(1e-30f));
            float2 t0  = (Min - origin) * inv * math.sign(dir + 1e-30f);
            float2 t1  = (Max - origin) * inv * math.sign(dir + 1e-30f);

            float2 tmin = math.min(t0, t1);
            float2 tmax = math.max(t0, t1);

            float enter = math.max(tmin.x, tmin.y);
            float exit  = math.min(tmax.x, tmax.y);

            if (exit  < 0f   ||
                enter > exit ||
                enter > tMax)
            {
                tEnter    = 0f;
                hitNormal = default;
                return false;
            }

            if (tmin.x      > tmin.y) hitNormal = new float2(-math.sign(dir.x), 0f);
            else if (tmin.y > tmin.x) hitNormal = new float2(0f,                -math.sign(dir.y));
            else
                hitNormal = math.abs(dir.x) > math.abs(dir.y)
                                ? new float2(-math.sign(dir.x), 0f)
                                : new float2(0f,                -math.sign(dir.y));

            tEnter = math.clamp(enter, 0f, tMax);
            return true;
        }

        // --------------- UTILITIES --------------- //
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public JAabbBox2D Translated(float2 delta) => FromMinMax(Min + delta, Max + delta);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public JAabbBox2D Inflated(float2 amount)
        {
            float2 a = math.max(amount, 0f);
            return new JAabbBox2D(xMin - a.x, yMin - a.y, xMax + a.x, yMax + a.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static JAabbBox2D Union(in JAabbBox2D a, in JAabbBox2D b) => FromMinMax(math.min(a.Min, b.Min), math.max(a.Max, b.Max));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static JAabbBox2D Intersection(in JAabbBox2D a, in JAabbBox2D b)
            => FromMinMax(math.max(a.Min, b.Min), math.min(a.Max, b.Max));

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public JAabbBox2D WithCenter(float2 c) => FromCenterHalf(c, Half);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public JAabbBox2D WithHalf(float2 half) => FromCenterHalf(Center, half);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public JAabbBox2D ExpandedToInclude(float2 p) => FromMinMax(math.min(Min, p), math.max(Max, p));

        public override string ToString() => $"Min:{Min}, Max:{Max}";
    }
}
#endif
