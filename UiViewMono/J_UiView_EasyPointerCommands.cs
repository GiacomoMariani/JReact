using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

namespace JReact.UiViewMono
{
    public class J_UiView_EasyPointerCommands : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private JUnityEvent _onClick_Events;
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private JUnityEvent _onDown_Events;
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private JUnityEvent _onEnter_Events;
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private JUnityEvent _onExit_Events;
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private JUnityEvent _onUp_Events;
        
        public void OnPointerClick(PointerEventData eventData) =>
            _onClick_Events?.Invoke();

        public void OnPointerDown(PointerEventData eventData) => _onDown_Events?.Invoke();
        public void OnPointerUp(PointerEventData eventData) => _onUp_Events?.Invoke();

        public void OnPointerEnter(PointerEventData eventData) => _onEnter_Events?.Invoke();
        public void OnPointerExit(PointerEventData eventData) => _onExit_Events?.Invoke();
    }
}
