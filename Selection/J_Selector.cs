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
    public abstract class J_Selector<T> : ScriptableObject, jObservableValue<T>, iResettable
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        private event Action<T> OnSelect;

        [FoldoutGroup("State", false, 5), ShowInInspector] private T _current;
        public T Current
        {
            get => _current;
            private set
            {
                //deselects if required
                if (_current != null) { ActOnDeselection(_current); }

                //set the value
                _current = value;
                if (_current != null) { ActOnSelection(_current); }

                //send event
                OnSelect?.Invoke(value);
            }
        }

        // --------------- COMMANDS --------------- //
        /// <summary>
        /// selects an item
        /// </summary>
        /// <param name="item">the item selected</param>
        public void Select(T item)
        {
            if (!CanSelect(item)) { return; }

            Current = item;
        }

        /// <summary>
        /// deselects the selected item
        /// </summary>
        public void Deselect()
        {
            if (!CanDeselect(Current)) { return; }

            Current = default;
        }

        // --------------- QUERIES --------------- //
        /// <summary>
        /// checks if the item is selected
        /// </summary>
        public bool IsSelected(T item)
        {
            if (EqualityComparer<T>.Default.Equals(Current, default(T))) { return false; }
            else { return EqualityComparer<T>.Default.Equals(Current, item); }
        }

        // --------------- VIRTUAL IMPLEMENTATION --------------- //
        //logic to stop the selection
        protected virtual bool CanSelect(T item) => true;

        /// any logic to be applied on the selected item
        protected virtual void ActOnSelection(T item) {}

        //logic to stop the deselection
        protected virtual bool CanDeselect(T selected) => true;

        //any logic to apply on the deselected item
        protected virtual void ActOnDeselection(T item) {}

        // --------------- DISABLE AND RESET --------------- //
        private        void OnDisable() => ResetThis();
        public virtual void ResetThis() => Deselect();

        // --------------- SUBSCRIBERS --------------- //
        public void Subscribe(Action<T>   actionToAdd)    => OnSelect += actionToAdd;
        public void UnSubscribe(Action<T> actionToRemove) => OnSelect -= actionToRemove;

#if UNITY_EDITOR
        [BoxGroup("Debug", true, true, 100), SerializeField] private T _selectTest;

        [BoxGroup("Debug", true, true, 100), Button("Select", ButtonSizes.Medium)] private void DebugSelect() => Current = _selectTest;

        [BoxGroup("Debug", true, true, 100), Button("DeSelect", ButtonSizes.Medium)]
        private void DebugDeSelect() => Current = default;
#endif
    }
}
