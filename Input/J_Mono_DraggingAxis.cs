using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

namespace JReact.J_Input
{
    [RequireComponent(typeof(Collider2D))]
    public class J_Mono_DraggingAxis : MonoBehaviour, IDragHandler
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private J_ReactiveVector2 _Axis;
        [BoxGroup("Setup", true, true, 0), SerializeField] private float _touchSensitivity_x = 10f;
        [BoxGroup("Setup", true, true, 0), SerializeField] private float _touchSensitivity_y = 10f;

        public void OnDrag(PointerEventData eventData)
            => _Axis.Current = new Vector2(eventData.delta.x * _touchSensitivity_x, eventData.delta.y * _touchSensitivity_y);
    }
}
