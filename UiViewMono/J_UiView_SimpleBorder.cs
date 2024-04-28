using JReact.Pool;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace JReact.UiViewMono
{
    [RequireComponent(typeof(CanvasRenderer))]
    public class J_UiView_SimpleBorder : MaskableGraphic
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        //used for serialization
        [BoxGroup("Setup", true, true, 0), SerializeField, ChildGameObjectsOnly, Required]
        private RectTransform _rectTransform;

        //0 would mean no border
        [BoxGroup("Setup", true, true, 0), SerializeField, Min(1f)] private float _size = 1f;

        //this can be less than 0, it will make the border larger than the rect transform
        [BoxGroup("Setup", true, true, 0), SerializeField] private float _edgeSize = 0f;

        [BoxGroup("Setup", true, true, 0), SerializeField] private Color _color = Color.white;

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            base.OnPopulateMesh(vh);
            if (_rectTransform.pivot != JConstants.Vector2Zero)
            {
                JLog.Warning($"{name} has pivot {_rectTransform.pivot}. Changing to {JConstants.Vector2Zero}");
                _rectTransform.pivot = JConstants.Vector2Zero;
            }

            vh.Clear();
            vh.DrawBordersOnRect(_rectTransform.rect, _size, _edgeSize, _color);
        }
    }
}
