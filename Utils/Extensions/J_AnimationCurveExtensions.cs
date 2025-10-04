using Unity.Mathematics;
using UnityEngine;

namespace JReact
{
    public static class J_AnimationCurveExtensions
    {
        private const float kTolerance = 1e-5f;

        public static float FindNormalizedTimeMonotonic(this AnimationCurve monotonicCurve, float value, int maxIter = 30,
                                                        float               tolerance = kTolerance)
        {
            Keyframe[] keys = monotonicCurve.keys;
            if (keys        == null ||
                keys.Length == 0) { return 0f; }

            float tMin = keys[0].time;
            float tMax = keys[^1].time;

            float yMin = monotonicCurve.Evaluate(tMin);
            float yMax = monotonicCurve.Evaluate(tMax);

            if (value <= math.min(yMin, yMax)) { return (yMin <= yMax) ? 0f : 1f; }

            if (value >= math.max(yMin, yMax)) { return (yMin <= yMax) ? 1f : 0f; }

            bool  ascending = yMax > yMin;
            float lo        = tMin, hi = tMax;

            for (int i = 0; i < maxIter; i++)
            {
                float mid        = 0.5f * (lo + hi);
                float y          = monotonicCurve.Evaluate(mid);
                float difference = y - value;

                if (math.abs(difference) <= tolerance) { return math.unlerp(tMin, tMax, mid); }

                if (ascending)
                {
                    if (y < value) lo = mid;
                    else hi           = mid;
                }
                else
                {
                    if (y > value) lo = mid;
                    else hi           = mid;
                }
            }

            float t = 0.5f * (lo + hi);
            return math.unlerp(tMin, tMax, t);
        }
    }
}
