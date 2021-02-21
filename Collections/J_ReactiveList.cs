using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.Collections
{
    public abstract class J_ReactiveList<T> : ScriptableObject, IList<T>, jObservable<T>, iReactiveIndexCollection<T>
    {
        // --------------- EVENTS --------------- //
        private event Action<T> OnAdd;
        private event Action<T> OnRemove;

        // --------------- STATE --------------- //
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] protected List<T> _ThisList { get; } = new List<T>(50);
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public int Length => _ThisList.Count;
        public int Count => _ThisList.Count;

        // --------------- ACCESSOR --------------- //
        public T this[int index] { get => _ThisList[index]; set => Replace(index, value); }

        // --------------- MAIN COMMANDS --------------- //
        public void Add(T item)
        {
            Assert.IsNotNull(_ThisList, $"{name} List not initialized");
            Assert.IsTrue(item != null, $"{name} Null elements are not valid");
            _ThisList.Add(item);
            //a virtual method if we want to add further actions
            WhatHappensOnAdd(item);
            OnAdd?.Invoke(item);
        }
        
        public T Get(int index) => _ThisList[index];

        public bool Remove(T item)
        {
            if (!_ThisList.Remove(item))
            {
                JLog.Warning($"The element {item} is not in the list {name}", JLogTags.Collection, this);
                return false;
            }

            //a virtual method if we want to add further actions
            WhatHappensOnRemove(item);
            OnRemove?.Invoke(item);
            return true;
        }

        public int RemoveAll(Predicate<T> predicate) => _ThisList.RemoveAll(predicate);

        public virtual void Clear()
        {
            //send the remove events for all the items
            for (int i = 0; i < Count; i++)
            {
                var item = _ThisList[i];
                WhatHappensOnRemove(item);
                OnRemove?.Invoke(item);
            }

            _ThisList.Clear();
        }

        /// <summary>
        /// process all the elements in this list with a given action
        /// </summary>
        /// <param name="actionToCall">the action used on all the items</param>
        public void ProcessWith(Action<T> actionToCall)
        {
            for (int i = 0; i < Count; i++) actionToCall(_ThisList[i]);
        }

        // --------------- FURTHER IMPLEMENTATIONS AND HELPERS --------------- //
        //virtual methods to be applied if required
        protected virtual void WhatHappensOnRemove(T elementToRemove) {}
        protected virtual void WhatHappensOnAdd(T    elementToAdd)    {}

        public virtual bool Contains(T elementToCheck) => _ThisList.Contains(elementToCheck);

        #region SUBSCRIBERS
        public void Subscribe(Action<T> actionToRegister)
        {
            OnAdd    += actionToRegister;
            OnRemove += actionToRegister;
        }

        public void UnSubscribe(Action<T> actionToRegister)
        {
            OnAdd    -= actionToRegister;
            OnRemove -= actionToRegister;
        }

        public void SubscribeToAdd(Action<T>   actionToRegister) { OnAdd += actionToRegister; }
        public void UnSubscribeToAdd(Action<T> actionToRegister) { OnAdd -= actionToRegister; }

        public void SubscribeToRemove(Action<T>   actionToRegister) { OnRemove += actionToRegister; }
        public void UnSubscribeToRemove(Action<T> actionToRegister) { OnRemove -= actionToRegister; }
        #endregion

        #region IENUMERABLE AND LIST IMPLEMENTATION
        public void CopyTo(T[] array, int arrayIndex) { _ThisList.CopyTo(array, arrayIndex); }

        public bool IsReadOnly => false;

        public IEnumerator<T> GetEnumerator() => _ThisList.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public int IndexOf(T item) => _ThisList.IndexOf(item);

        public void Insert(int index, T item)
        {
            for (int i = index; i < Count; i++)
            {
                T next = _ThisList[i];
                Replace(i, item);
                item = next;
            }
        }

        public void RemoveAt(int index)
        {
            Assert.IsTrue(index < Count, $"{name} has not the given index {index}. List length = {Count}");
            T item = _ThisList[index];
            Remove(item);
        }

        protected void Replace(int index, T item)
        {
            Assert.IsNotNull(_ThisList, $"{name} Collection not initialized");
            Assert.IsTrue(item != null, $"{name} Null elements are not valid");
            RemoveAt(index);
            _ThisList.Insert(index, item);
            WhatHappensOnAdd(item);
            OnAdd?.Invoke(item);
        }
        #endregion
    }
}
