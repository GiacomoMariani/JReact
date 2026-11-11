using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.UiViewMono
{
    public static class JRoundedOutline
    {
        private static readonly float[] CornerStart = { 180f, 90f, 0f, -90f };

        private const string NoSegments = nameof(JRoundedOutline) + ": a corner needs at least one segment.";

        public static int PointCount(int cornerSegments) => (cornerSegments + 1) * CornerStart.Length;

        public static List<Vector2> SetRoundedRect(this List<Vector2> outline, Rect rect, float radius, float inset,
                                                   int cornerSegments)
        {
            Assert.IsTrue(cornerSegments > 0, NoSegments);
            outline.Clear();
            var inner = new Rect(rect.xMin + inset, rect.yMin + inset, rect.width - inset * 2f, rect.height - inset * 2f);
            if (inner.width <= 0f || inner.height <= 0f) { return outline; }

            float corner = Mathf.Clamp(radius - inset, 0f, Mathf.Min(inner.width, inner.height) * 0.5f);
            for (int i = 0; i < CornerStart.Length; i++)
            {
                Vector2 centre = Centre(inner, corner, i);
                for (int step = 0; step <= cornerSegments; step++)
                {
                    float radians = (CornerStart[i] - 90f * step / cornerSegments) * Mathf.Deg2Rad;
                    outline.Add(centre + new Vector2(Mathf.Cos(radians), Mathf.Sin(radians)) * corner);
                }
            }

            return outline;
        }

        private static Vector2 Centre(Rect rect, float radius, int corner)
        {
            float x = corner == 0 || corner == 3 ? rect.xMin + radius : rect.xMax - radius;
            float y = corner < 2 ? rect.yMax - radius : rect.yMin + radius;
            return new Vector2(x, y);
        }
    }
}
