using System.Text;
using UnityEngine;

namespace JMath2D
{
    public static class J_MathEffects
    {
        private const string Material = "Unlit/Color";
        private const string LineFormat = "{0}_{1}_Line";
        private const string PointFormat = "{0}_Point";
        private static readonly StringBuilder _stringBuilder = new StringBuilder(150);
        private static StringBuilder SBuilder
        {
            get
            {
                _stringBuilder.Clear();
                return _stringBuilder;
            }
        }

        public static GameObject DrawLineTo(this Vector2 start, Vector2 end, Color color, float startWidth = 1f, float endWidth = 1f,
                                            string       material = Material)
        {
            var lineGO   = new GameObject(SBuilder.AppendFormat(LineFormat, start, end).ToString());
            var renderer = lineGO.AddComponent<LineRenderer>();
            renderer.material      = new Material(Shader.Find(material)) { color = color };
            renderer.positionCount = 2;
            renderer.SetPosition(0, new Vector2(start.x, start.y));
            renderer.SetPosition(1, new Vector2(end.x,   end.y));
            renderer.startWidth = startWidth;
            renderer.endWidth   = endWidth;
            return lineGO;
        }

        public static GameObject DrawPoint(this Vector2 point, Color color, float length = 4f, float width = 1f,
                                           string       material = Material)
        {
            var pointGO      = new GameObject(SBuilder.AppendFormat(PointFormat, point).ToString());
            var lineRenderer = pointGO.AddComponent<LineRenderer>();
            lineRenderer.material      = new Material(Shader.Find(material)) { color = color };
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, new Vector3(point.x - width / length, point.y - width / length));
            lineRenderer.SetPosition(1, new Vector3(point.x + width / length, point.y + width / length));
            lineRenderer.startWidth = width;
            lineRenderer.endWidth   = width;
            return pointGO;
        }
    }
}
