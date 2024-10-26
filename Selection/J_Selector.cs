using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.Selection
{
    /// <summary>
    /// selects one item
    /// </summary>
    /// <typeparam name="T">type of the selectable item</typeparam>
    public abstract class J_Selector<T> : MonoBehaviour, iResettable
        where T : iSelectable
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        private event Action<T> OnSelect;
        private event Action<T> OnDeselect;

        [FoldoutGroup("State", false, 5), ShowInInspector] private List<T> _selected = new List<T>(8);

        [FoldoutGroup("State", false, 5), ShowInInspector] public T Current => GetMainSelected();
        [FoldoutGroup("State", false, 5), ShowInInspector] public bool NothingSelected => TotalSelected == 0;
        [FoldoutGroup("State", false, 5), ShowInInspector] public int TotalSelected => _selected.Count;

        // --------------- COMMANDS --------------- //
        /// <summary>
        /// selects multiple items
        /// </summary>
        /// <param name="items">the items to be selected</param>
        /// <param name="resetPreviousSelection">if we want to remove all previously selected items</param>
        public void SelectMultiple(IEnumerable<T> items, bool resetPreviousSelection = false)
        {
            if (resetPreviousSelection) { ResetThis(); }

            using IEnumerator<T> enumerator = items.GetEnumerator();
            while (enumerator.MoveNext()) { Select(enumerator.Current); }
        }

        /// <summary>
        /// deselects multiple items
        /// </summary>
        public void DeselectMultiple(IEnumerable<T> items)
        {
            using IEnumerator<T> enumerator = items.GetEnumerator();
            while (enumerator.MoveNext()) { Deselect(enumerator.Current); }
        }

        /// <summary>
        /// Toggles the selection of an item.
        /// If the item is selected, it will be deselected.
        /// If the item is not selected, it will be selected.
        /// </summary>
        /// <param name="item">The item to toggle the selection for.</param>
        /// <param name="resetPreviousSelection"></param>
        public void ToggleSelection(T item, bool resetPreviousSelection = false)
        {
            if (!IsSelected(item)) { Select(item, resetPreviousSelection); }
            else { Deselect(item); }
        }

        /// <summary>
        /// selects an item
        /// </summary>
        /// <param name="item">the item selected</param>
        /// <param name="resetPreviousSelection">if we want to remove all previously selected items</param>
        public void Select(T item, bool resetPreviousSelection = true)
        {
            if (resetPreviousSelection) { ResetThis(); }

            if (!CanSelect(item)) { return; }

            SelectImpl(item);
            _selected.Add(item);
        }

        private void SelectImpl(T item)
        {
            ActOnSelection(item);
            OnSelect?.Invoke(item);
        }

        /// <summary>
        /// deselects an item
        /// </summary>
        public void Deselect(T item)
        {
            if (!CanDeselect(item)) { return; }

            DeselectImpl(item);
            _selected.Remove(item);
        }

        private void DeselectImpl(T item)
        {
            ActOnDeselection(item);
            OnDeselect?.Invoke(item);
        }

        /// <summary>
        /// Resets the current selection by deselecting all selected items.
        /// </summary>
        public virtual void ResetThis()
        {
            for (int i = _selected.Count - 1; i >= 0; i--) { DeselectImpl(_selected[i]); }
            _selected.Clear();
        }

        // --------------- QUERIES --------------- //
        /// <summary>
        /// checks if the item is selected
        /// </summary>
        public bool IsSelected(T item) => _selected.Contains(item);

        public virtual T GetMainSelected() => _selected.Count == 0 ? default(T) : _selected[0];

        // --------------- VIRTUAL IMPLEMENTATION --------------- //
        //logic to stop the selection
        protected virtual bool CanSelect(T item) => !_selected.Contains(item);

        /// any logic to be applied on the selected item
        protected virtual void ActOnSelection(T item) {}

        //logic to stop the deselection
        protected virtual bool CanDeselect(T item) => _selected.Contains(item);

        //any logic to apply on the deselected item
        protected virtual void ActOnDeselection(T item) {}

        // --------------- SUBSCRIBERS --------------- //
        public void SubscribeOnSelect(Action<T>     actionToAdd)    => OnSelect += actionToAdd;
        public void UnSubscribeOnSelect(Action<T>   actionToRemove) => OnSelect -= actionToRemove;
        public void SubscribeOnDeselect(Action<T>   actionToAdd)    => OnDeselect += actionToAdd;
        public void UnSubscribeOnDeselect(Action<T> actionToRemove) => OnDeselect -= actionToRemove;

#if UNITY_EDITOR
        [BoxGroup("Debug", true, true, 100), SerializeField] private T _selectTest;

        [BoxGroup("Debug", true, true, 100), Button("Select", ButtonSizes.Medium)] private void DebugSelect() => Select(_selectTest);

        [BoxGroup("Debug", true, true, 100), Button("DeSelect", ButtonSizes.Medium)]
        private void DebugDeSelect() => ResetThis();
#endif
    }
}
