using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace JReact.UiViewMono
{
    public class J_UiView_SimpleOverlayOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField, ChildGameObjectsOnly, Required]
        private Image _overlay;

        public void OnPointerEnter(PointerEventData eventData) { _overlay.enabled = true; }

        public void OnPointerExit(PointerEventData eventData) { _overlay.enabled = false; }

        // --------------- LISTENER SETUP --------------- //
        private void OnEnable()  { _overlay.enabled = false; }
        private void OnDisable() { _overlay.enabled = false; }
    }
}
