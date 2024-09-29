using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

namespace JReact.Selection
{
    [RequireComponent(typeof(Collider2D))]
    public abstract class J_HoverSelection<T> : J_Mono_ActorElement<T>, IPointerEnterHandler, IPointerExitHandler
    where T :  iSelectable<T>
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] protected abstract J_Selector<T> _Selector { get; }

        public void OnPointerEnter(PointerEventData eventData) => _Selector.Select(_actor);
        public void OnPointerExit(PointerEventData  eventData) => _Selector.Deselect(_actor);
    }
}
