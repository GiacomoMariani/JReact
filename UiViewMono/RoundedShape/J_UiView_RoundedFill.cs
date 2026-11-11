using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace JReact.UiViewMono
{
    [RequireComponent(typeof(CanvasRenderer))]
    public sealed class J_UiView_RoundedFill : MaskableGraphic
    {
        private const int MaxSegments = 24;

        [BoxGroup("Setup", true, true, 0), SerializeField, Min(0f)] private float _radius = 8f;
        [BoxGroup("Setup", true, true, 0), SerializeField, Min(0f)] private float _feather = 1f;
        [BoxGroup("Setup", true, true, 0), SerializeField, Range(1, MaxSegments)] private int _cornerSegments = 6;

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector]
        private readonly List<Vector2> _body = new List<Vector2>(JRoundedOutline.PointCount(MaxSegments));
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector]
        private readonly List<Vector2> _fade = new List<Vector2>(JRoundedOutline.PointCount(MaxSegments));

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            Rect rect = rectTransform.rect;
            float half = _feather * 0.5f;
            Color clear = color;
            clear.a = 0f;
            vh.AddFan(_body.SetRoundedRect(rect, _radius, half, _cornerSegments), color)
              .AddBand(_fade.SetRoundedRect(rect, _radius, -half, _cornerSegments), _body, clear, color);
        }
    }
}
