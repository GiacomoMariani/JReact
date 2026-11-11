using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace JReact.UiViewMono
{
    [RequireComponent(typeof(CanvasRenderer))]
    public sealed class J_UiView_RoundedRing : MaskableGraphic
    {
        private const int MaxSegments = 24;

        [BoxGroup("Setup", true, true, 0), SerializeField, Min(0f)] private float _radius = 8f;
        [BoxGroup("Setup", true, true, 0), SerializeField, Min(0f)] private float _thickness = 4f;
        [BoxGroup("Setup", true, true, 0), SerializeField] private float _inset;
        [BoxGroup("Setup", true, true, 0), SerializeField, Min(0f)] private float _feather = 1f;
        [BoxGroup("Setup", true, true, 0), SerializeField, Range(1, MaxSegments)] private int _cornerSegments = 6;

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector]
        private readonly List<Vector2> _outerFade = new List<Vector2>(JRoundedOutline.PointCount(MaxSegments));
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector]
        private readonly List<Vector2> _outerEdge = new List<Vector2>(JRoundedOutline.PointCount(MaxSegments));
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector]
        private readonly List<Vector2> _innerEdge = new List<Vector2>(JRoundedOutline.PointCount(MaxSegments));
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector]
        private readonly List<Vector2> _innerFade = new List<Vector2>(JRoundedOutline.PointCount(MaxSegments));

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            if (_thickness <= 0f) { return; }

            Rect rect = rectTransform.rect;
            float half = Mathf.Min(_feather, _thickness) * 0.5f;
            float inner = _inset + _thickness;
            Color clear = color;
            clear.a = 0f;
            _outerFade.SetRoundedRect(rect, _radius, _inset - half, _cornerSegments);
            _outerEdge.SetRoundedRect(rect, _radius, _inset + half, _cornerSegments);
            _innerEdge.SetRoundedRect(rect, _radius, inner - half, _cornerSegments);
            _innerFade.SetRoundedRect(rect, _radius, inner + half, _cornerSegments);
            vh.AddBand(_outerFade, _outerEdge, clear, color);
            vh.AddBand(_outerEdge, _innerEdge, color, color);
            vh.AddBand(_innerEdge, _innerFade, color, clear);
        }
    }
}
