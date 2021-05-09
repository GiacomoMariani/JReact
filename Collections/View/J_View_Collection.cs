using System.Collections.Generic;
using JReact.Pool;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.Collections.View
{
    /// <summary>
    /// shows a collection of elements
    /// </summary>
    public abstract class J_View_Collection<T> : MonoBehaviour
    {
        // --------------- ABSTRACT PROPERTIES --------------- //
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector]
        protected abstract iReactiveIndexCollection<T> _Collection { get; }
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] protected abstract J_Mono_Actor<T> _PrefabActor { get; }

        // --------------- FIELDS AND PROPERTIES --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField] private bool _openFull = true;
        [BoxGroup("Setup", true, true, 0), SerializeField, ChildGameObjectsOnly, Required]
        private Transform _viewParent;

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private iReactiveIndexCollection<T> _currentlyDisplayed;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private Dictionary<T, J_Mono_Actor<T>> _trackedElements;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private J_Pool<J_Mono_Actor<T>> _pool;

        // --------------- INITIALIZATION --------------- //
        private void Awake()
        {
            SanityChecks();
            InitThis();
            Transform poolTransform = null;
#if UNITY_EDITOR
            poolTransform = new GameObject($"{name}_disabled", typeof(RectTransform)).transform;
            poolTransform.SetParent(this.transform);
#endif
            var amount = _currentlyDisplayed.Length;
            _pool            = J_Pool<J_Mono_Actor<T>>.GetPool(_PrefabActor, amount, amount, poolTransform);
            _trackedElements = new Dictionary<T, J_Mono_Actor<T>>(amount);
        }

        protected virtual void SanityChecks()
        {
            Assert.IsNotNull(_PrefabActor, $"{gameObject.name} requires a {nameof(_PrefabActor)}");
            Assert.IsNotNull(_Collection,  $"{gameObject.name} requires a {nameof(_Collection)}");
        }

        protected virtual void InitThis()
        {
            _currentlyDisplayed = _Collection;
            if (_viewParent == null) { _viewParent = this.transform; }
        }

        // --------------- VIEW UPDATER --------------- //
        protected virtual void Open()
        {
            Assert.IsNotNull(_currentlyDisplayed, $"{gameObject.name} requires a {nameof(_currentlyDisplayed)}");
            if (_openFull) { ShowFrom(0, _currentlyDisplayed.Length); }
        }

        protected virtual void Close() {}

        // --------------- SINGLE VIEW COMMANDS --------------- //
        private void UpdateView(T item)
        {
            if (!WantToShowElement(item)) { return; }

            // --------------- VIEW CREATION --------------- //
            J_Mono_Actor<T> view = _trackedElements.ContainsKey(item)
                                       ? _trackedElements[item]
                                       : _pool.Spawn(_viewParent);

            view.ActorUpdate(item);
            if (_trackedElements.ContainsKey(item)) { return; }

            _trackedElements[item] = view;
            AddedView(item, view);
        }

        private void Remove(T itemRemoved)
        {
            RemovedView(itemRemoved, _trackedElements[itemRemoved]);
            _pool.DeSpawn(_trackedElements[itemRemoved].gameObject);
            _trackedElements.Remove(itemRemoved);
        }

        // --------------- DISPLAY COMMANDS --------------- //
        public void ReplaceCollection(iReactiveIndexCollection<T> newCollection)
        {
            ClearViews();
            _currentlyDisplayed = newCollection;
        }

        /// <summary>
        /// shows a collection only within 2 indexes
        /// </summary>
        /// <param name="start">the first indexes</param>
        /// <param name="amount">the amount to display</param>
        public void ShowFrom(int start = 0, int amount = 1)
        {
            var finalItemIndex = start + amount;
            Assert.IsTrue(start < _currentlyDisplayed.Length, $"{name} - {start} out of bounds {_currentlyDisplayed.Length}");
            Assert.IsTrue(finalItemIndex <= _currentlyDisplayed.Length,
                          $"{name} - {finalItemIndex} out of bounds {_currentlyDisplayed.Length}");

            ClearViews();
            for (int i = start; i < finalItemIndex; i++) { UpdateView(_currentlyDisplayed[i]); }
        }

        private void ClearViews()
        {
            for (int i = 0; i < _currentlyDisplayed.Length; i++) { Remove(_currentlyDisplayed[i]); }
        }

        // --------------- FURTHER IMPLEMENTATION --------------- //
        /// <summary>
        /// a virtual method that may be used to decide if show or not show
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected virtual bool WantToShowElement(T item) => true;

        /// <summary>
        /// logic that might be applied when an item is added
        /// </summary>
        protected virtual void AddedView(T item, J_Mono_Actor<T> view) {}

        /// <summary>
        /// a virtual method to add further logic to apply when an item is removed
        /// </summary>
        protected virtual void RemovedView(T item, J_Mono_Actor<T> view) {}

        // --------------- UNITY EVENTS --------------- //
        private void OnEnable()
        {
            Open();
            _currentlyDisplayed.SubscribeToAdd(UpdateView);
            _currentlyDisplayed.SubscribeToRemove(Remove);
        }

        private void OnDisable()
        {
            _currentlyDisplayed.UnSubscribeToAdd(UpdateView);
            _currentlyDisplayed.UnSubscribeToRemove(Remove);
            Close();
        }
    }
}
