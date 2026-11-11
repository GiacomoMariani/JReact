using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace JReact.UiViewMono
{
    public static class J_VertexHelper_Extensions
    {
        private const string UnevenBand = nameof(AddBand) + ": both outlines need the same corner segments.";

        public static VertexHelper AddFan(this VertexHelper vh, List<Vector2> outline, Color tint)
        {
            int count = outline.Count;
            if (count < 3) { return vh; }

            Vector2 centre = Vector2.zero;
            for (int i = 0; i < count; i++) { centre += outline[i]; }

            int first = vh.currentVertCount;
            vh.AddVert(centre / count, tint, Vector2.zero);
            for (int i = 0; i < count; i++) { vh.AddVert(outline[i], tint, Vector2.zero); }

            for (int i = 0; i < count; i++)
            {
                vh.AddTriangle(first, first + 1 + i, first + 1 + (i + 1) % count);
            }

            return vh;
        }

        public static VertexHelper AddBand(this VertexHelper vh, List<Vector2> outer, List<Vector2> inner, Color outerTint,
                                           Color innerTint)
        {
            int count = outer.Count;
            if (count < 3) { return vh; }
            if (inner.Count == 0) { return vh.AddFan(outer, outerTint); }

            Assert.AreEqual(count, inner.Count, UnevenBand);
            int first = vh.currentVertCount;
            for (int i = 0; i < count; i++)
            {
                vh.AddVert(outer[i], outerTint, Vector2.zero);
                vh.AddVert(inner[i], innerTint, Vector2.zero);
            }

            for (int i = 0; i < count; i++)
            {
                int a = first + i * 2;
                int b = first + (i + 1) % count * 2;
                vh.AddTriangle(a, b, b + 1);
                vh.AddTriangle(a, b + 1, a + 1);
            }

            return vh;
        }
    }
}
