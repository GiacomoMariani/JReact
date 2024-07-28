using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

namespace JReact.Selection
{
    [RequireComponent(typeof(Collider2D))]
    public abstract class J_Mono_MouseOverSelection<T> : J_Mono_ActorElement<T>, IPointerEnterHandler, IPointerExitHandler
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] protected abstract J_Selector<T> _Selector { get; }
        public void OnPointerEnter(PointerEventData eventData) => _Selector.Select(_actor);
        public void OnPointerExit(PointerEventData  eventData) => _Selector.Deselect();
    }
}
