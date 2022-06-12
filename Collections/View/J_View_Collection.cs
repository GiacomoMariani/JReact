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
        private const int ExpectedAmount = 12;
        private const string PoolParentName = "DisabledPool";
        // --------------- ABSTRACT PROPERTIES --------------- //
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector]
        protected virtual iReactiveIndexCollection<T> _Collection { get; private set; }
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] protected abstract J_Mono_Actor<T> _PrefabActor { get; }

        // --------------- FIELDS AND PROPERTIES --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField] private bool _openFull = true;
        [BoxGroup("Setup", true, true, 0), SerializeField, ChildGameObjectsOnly, Required]
        private Transform _viewParent;
        [BoxGroup("Setup", true, true, 0), SerializeField, ChildGameObjectsOnly, Required]
        private Transform _poolParent;

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private Dictionary<T, J_Mono_Actor<T>> _trackedElements;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private J_Pool<J_Mono_Actor<T>> _pool;

        // --------------- INITIALIZATION --------------- //
        private void Awake()
        {
            SanityChecks();
            InitThis();
            _poolParent = new GameObject(PoolParentName, typeof(RectTransform)).transform;
            _poolParent.SetParent(this.transform, false);
            _poolParent.gameObject.SetActive(false);

            var amount = ExpectedAmount;
            if (_Collection != null) { amount = _Collection.Length; }

            _pool            = J_Pool<J_Mono_Actor<T>>.GetPool(_PrefabActor, amount, amount, _poolParent);
            _trackedElements = new Dictionary<T, J_Mono_Actor<T>>(amount);
        }

        protected virtual void SanityChecks()
        {
            Assert.IsNotNull(_PrefabActor, $"{gameObject.name} requires a {nameof(_PrefabActor)}");
            Assert.IsTrue(!_openFull || _Collection != null, $"{gameObject.name} requires a {nameof(_Collection)}");
        }

        protected virtual void InitThis()
        {
            if (_viewParent == null) { _viewParent = this.transform; }
        }

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
            if (!_trackedElements.ContainsKey(itemRemoved)) { return; }

            RemovedView(itemRemoved, _trackedElements[itemRemoved]);
            _pool.DeSpawn(_trackedElements[itemRemoved].gameObject);
            _trackedElements.Remove(itemRemoved);
        }

        // --------------- DISPLAY COMMANDS --------------- //
        protected virtual void OpenFull()
        {
            Assert.IsNotNull(_Collection, $"{gameObject.name} requires a {nameof(_Collection)}");
            ShowFrom(0, _Collection.Length);
        }

        protected virtual void Close() {}

        public void SetCollection(iReactiveIndexCollection<T> newCollection)
        {
            ClearViews();
            if (gameObject.activeSelf &&
                _Collection != null) { UnTrackCollection(_Collection); }

            _Collection = newCollection;
            if (gameObject.activeSelf &&
                _Collection != null) { TrackCollection(_Collection); }
        }

        /// <summary>
        /// shows a collection only within 2 indexes
        /// </summary>
        /// <param name="start">the first indexes</param>
        /// <param name="amount">the amount to display</param>
        public void ShowFrom(int start = 0, int amount = 1)
        {
            if (amount <= 0) { return; }

            var finalItemIndex = start + amount;
            Assert.IsTrue(start < _Collection.Length, $"{name} - {start} out of bounds {_Collection.Length}");
            Assert.IsTrue(finalItemIndex <= _Collection.Length,
                          $"{name} - {finalItemIndex} out of bounds {_Collection.Length}");

            ClearViews();
            for (int i = start; i < finalItemIndex; i++) { UpdateView(_Collection[i]); }
        }

        public void ClearViews()
        {
            if (_Collection            == null ||
                _trackedElements       == null ||
                _trackedElements.Count == 0) { return; }

            for (int i = 0; i < _Collection.Length; i++) { Remove(_Collection[i]); }
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
            if (_openFull) { OpenFull(); }

            if (_Collection != null) { TrackCollection(_Collection); }
        }

        private void TrackCollection(iReactiveIndexCollection<T> collection)
        {
            collection.UnSubscribeToAdd(UpdateView);
            collection.SubscribeToAdd(UpdateView);
            collection.UnSubscribeToRemove(Remove);
            collection.SubscribeToRemove(Remove);
        }

        private void OnDisable()
        {
            if (_Collection != null) { UnTrackCollection(_Collection); }

            Close();
        }

        private void UnTrackCollection(iReactiveIndexCollection<T> collection)
        {
            collection.UnSubscribeToAdd(UpdateView);
            collection.UnSubscribeToRemove(Remove);
        }
    }
}
