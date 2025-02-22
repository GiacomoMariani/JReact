using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

namespace JReact.Selection
{
    [RequireComponent(typeof(Collider2D))]
    public abstract class J_ClickSelection<T> : J_Mono_ActorElement<T>, IPointerClickHandler
        where T : iSelectable
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField] private bool _deselectOnClick;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] protected abstract J_Selector<T> _Selector { get; }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!_Selector.IsSelected(_actor)) { _Selector.Select(_actor); }
            else if (_deselectOnClick) { _Selector.Deselect(_actor); }
        }
    }
}
