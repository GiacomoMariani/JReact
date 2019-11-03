using UnityEngine;

namespace JReact.JScreen
{
    [CreateAssetMenu(menuName = "Reactive/Screen/Camera", fileName = "ReactiveCamera", order = 0)]
    public class J_ReactiveCamera : J_ReactiveItem<Camera>
    {
        public void    SendCameraTo(Vector2                  place)         {}
        public Vector2 RectToScreenPosition(RectTransform    rect)          => rect.ToScreenPosition(Current);
        public Vector2 WorldPositionToScreenPosition(Vector3 worldPosition) => Current.WorldToScreenPoint(worldPosition);
    }
}
