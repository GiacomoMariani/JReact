using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.Collections
{
    /// <summary>
    /// an array that acts as a reactive collection, with the option to add at a given index, or add in the first empty place
    /// </summary>
    public abstract class J_ReactiveArray<T> : ScriptableObject,
                                               jObservable<(int index, T previous, T current)>,
                                               iResettable,
                                               iReactiveIndexCollection<T>
    {
        // --------------- EVENTS --------------- //
        private event Action<(int index, T previous, T current)> OnChange;
        public event Action<T> OnAdd;
        public event Action<T> OnRemove;

        // --------------- SETUP --------------- //
        [InfoBox("NULL => generated at default"), BoxGroup("Setup", true, true), SerializeField]
        protected T[] _thisArray;

        // --------------- STATE --------------- //
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public int Length => _thisArray?.Length ?? 0;

        // --------------- ARRAY --------------- //
        public T this[int index] { get => _thisArray[index]; set => AddAt(index, value); }

        public static J_ReactiveArray<T> Create(int length)
        {
            var reactiveArray = CreateInstance<J_ReactiveArray<T>>();
            reactiveArray.SetupWith(length);
            return reactiveArray;
        }

        // --------------- COMMANDS - INDEX --------------- //
        /// <summary>
        /// adds an item at the given index, replacing the previous if needed 
        /// </summary>
        public void AddAt(int index, T item)
        {
            T previous = _thisArray[index];
            _thisArray[index] = item;

            if (previous != null) HappensOnRemove(previous);
            HappensOnAdd(item);

            OnChange?.Invoke((index, previous, item));
        }

        /// <summary>
        /// removes the item at a given index
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt(int index)
        {
            T item = _thisArray[index];
            _thisArray[index] = default;

            HappensOnRemove(item);

            OnChange?.Invoke((index, item, default));
        }

        /// <summary>
        /// gets the item at the given index
        /// </summary>
        public T Get(int index) => _thisArray[index];

        // --------------- COMMANDS - PLAIN --------------- //
        /// <summary>
        /// adds the item in the first empty spot and returns the index
        /// </summary>
        /// <returns>the index where this is added, or -1 if the array is full</returns>
        public int Add(T item)
        {
            //find the first empty place
            int index = IndexOf(default);
            // --------------- ADD CONFIRM --------------- //
            if (index != -1)
            {
                AddAt(index, item);
                return index;
            }

            // --------------- ADD ERROR --------------- //
            JLog.Warning($"{name} array is full. Item {item}");
            return -1;
        }

        /// <summary>
        /// removes the first item from the array, returning its index
        /// </summary>
        /// <returns>returns the index of the item, or -1 if nothing is found</returns>
        public int Remove(T item)
        {
            if (item == null) JLog.Warning($"{name} is removing a null element. Is it intended?");
            //find the item
            int index = IndexOf(item);
            // --------------- REMOVE CONFIRM --------------- //
            if (index != -1)
            {
                RemoveAt(index);
                return index;
            }

            // --------------- REMOVE ERROR --------------- //
            JLog.Warning($"{name} there is not such item in the array. Item {item}");
            return -1;
        }

        /// <summary>
        /// returns the first index with the given item
        /// </summary>
        /// <param name="item">the item we search</param>
        /// <returns></returns>
        public int IndexOf(T item) => Array.IndexOf(_thisArray, item);

        // --------------- VIRTUAL FURTHER IMPLEMENTATION --------------- //
        protected virtual void HappensOnRemove(T item) => OnAdd?.Invoke(item);
        protected virtual void HappensOnAdd(T    item) => OnRemove?.Invoke(item);

        // --------------- RESETS --------------- //
        [FoldoutGroup("Commands", false, 100), Button(ButtonSizes.Medium)]
        public void ResetThis()
        {
            for (int i = 0; i < Length; i++) _thisArray[i] = default;
        }

        [FoldoutGroup("Commands", false, 100), Button(ButtonSizes.Medium)]
        public void SetupWith(int length) => _thisArray = new T[length];

        public void Clear() => ResetThis();

        // --------------- HELPERS --------------- //
        /// <summary>
        /// process all the non null elements with an action
        /// </summary>
        public void ProcessWith(Action<T> actionToCall)
        {
            for (int i = 0; i < Length; i++)
                if (_thisArray[i] != null)
                    actionToCall(_thisArray[i]);
        }

        public void CopyTo(T[] array, int arrayIndex) => _thisArray.CopyTo(array, arrayIndex);

        public bool Contains(T item) => IndexOf(item) != -1;

        // --------------- SUBSCRIBERS--------------- //
        public void Subscribe(Action<(int index, T previous, T current)>   action) => OnChange += action;
        public void UnSubscribe(Action<(int index, T previous, T current)> action) => OnChange -= action;

        public void SubscribeToAdd(Action<T>   actionToRegister) => OnAdd += actionToRegister;
        public void UnSubscribeToAdd(Action<T> actionToRegister) => OnAdd -= actionToRegister;

        public void SubscribeToRemove(Action<T>   actionToRegister) => OnRemove += actionToRegister;
        public void UnSubscribeToRemove(Action<T> actionToRegister) => OnRemove -= actionToRegister;
    }
}
