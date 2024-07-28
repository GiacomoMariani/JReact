using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

namespace JReact.Selection
{
    [RequireComponent(typeof(Collider2D))]
    public abstract class J_Mono_MouseClick_Selection<T> : J_Mono_ActorElement<T>, IPointerClickHandler
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField] private bool _deselectOnClick;
        protected abstract J_Selector<T> _Selector { get; }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!_Selector.IsSelected(_actor)) _Selector.Select(_actor);
            else if (_deselectOnClick) _Selector.Deselect();
        }
    }
}
