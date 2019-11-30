using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.JScreen
{
    [CreateAssetMenu(menuName = "Reactive/Screen/Camera", fileName = "ReactiveCamera", order = 0)]
    public sealed class J_ReactiveCamera : J_ReactiveItem<Camera>
    {
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private float _zPosition = -10f;

        public void SendCameraTo(Vector2 place) => Current.transform.position = new Vector3(place.x, place.y, _zPosition);

        public Vector2 RectToScreenPosition(RectTransform    rect)          => rect.ToScreenPosition(Current);
        public Vector2 WorldPositionToScreenPosition(Vector3 worldPosition) => Current.WorldToScreenPoint(worldPosition);
    }
}
