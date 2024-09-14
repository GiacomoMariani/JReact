using TMPro;
using Unity.Mathematics;
using UnityEngine;

namespace JReact.DebugUtils
{
    public static class JDebug
    {
        private static GameObject _DebugParent;
        private static GameObject DebugParent
        {
            get
            {
                if (_DebugParent == null) { _DebugParent = new GameObject("DebugParent"); }

                return _DebugParent;
            }
        }

        private static Sprite _circle;
        public static Sprite Circle
        {
            get
            {
                if (_circle != null) { return _circle; }

                Texture2D texture = new Texture2D(128, 128);
                for (int y = 0; y < texture.height; y++)
                {
                    for (int x = 0; x < texture.width; x++)
                    {
                        float xCenter = x - texture.width  / 2.0f;
                        float yCenter = y - texture.height / 2.0f;
                        texture.SetPixel(x, y, xCenter * xCenter + yCenter * yCenter <=
                                               (texture.width / 2.0f) * (texture.height / 2.0f)
                                                   ? Color.white
                                                   : Color.clear);
                    }
                }

                texture.Apply();
                _circle = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

                return _circle;
            }
        }

        private static Material _defaultMaterial;
        public static Material DefaultMaterial
        {
            get
            {
                if (_defaultMaterial == null) { _defaultMaterial = new Material(Shader.Find("Sprites/Default")); }

                return _defaultMaterial;
            }
        }

        public static void AddText(Transform parent, string textToAdd)
        {
            GameObject textGO = new GameObject("Text");
            textGO.transform.SetParent(parent);
            TextMeshPro textComponent = textGO.AddComponent<TextMeshPro>();
            textComponent.renderer.sortingOrder       = 100;
            textComponent.alignment                   = TextAlignmentOptions.Center;
            textComponent.rectTransform.localPosition = new Vector3(0, 0.2f, 0); // Position it slightly above the parent
            textComponent.fontSize                    = 0.5f;
            textComponent.color                       = Color.black;
            textComponent.text                        = textToAdd;
        }

        public static SpriteRenderer DrawCircle(Vector2 position, Color color, GameObject parentGo = default, float localScale = 0.2f,
                                                int     sortingOrder = 100, string sortingLayer = "Default")
        {
            parentGo ??= DebugParent;
            SpriteRenderer renderer = new GameObject("CircleDebug").AddComponent<SpriteRenderer>();
            renderer.transform.SetParent(parentGo.transform);
            renderer.transform.position   = position;
            renderer.transform.localScale = Vector3.one * localScale;
            renderer.color                = color;
            renderer.sortingOrder         = sortingOrder;
            renderer.sortingLayerName     = sortingLayer;
            renderer.sprite               = Circle;
            return renderer;
        }

        public static void DrawLine(GameObject parentGo,         Vector2 origin, Vector2 destination, Color color, float startWidth = 0.02f,
                                    float      endWidth = 0.02f, int  sortingLayerID = 0)
        {

            GameObject   line         = new GameObject("Line");
            LineRenderer lineRenderer = line.AddComponent<LineRenderer>();
            lineRenderer.sortingLayerID = sortingLayerID;
            lineRenderer.transform.SetParent(parentGo.transform);
            lineRenderer.startColor    = color;
            lineRenderer.endColor      = color;
            lineRenderer.startWidth    = startWidth;
            lineRenderer.endWidth      = endWidth;
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, origin);
            lineRenderer.SetPosition(1, destination);
            lineRenderer.material = DefaultMaterial;
        }
    }
}
