using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.Collections
{
    public class J_ReactiveQueue<T> : ICollection, IReadOnlyCollection<T>, jObservable<T>
    {
        private event Action<T> OnDequeue;
        private event Action<T> OnEnqueue;

        // --------------- FIELDS AND PROPERTIES --------------- //
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private static int QueueIDCounter = 0;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private readonly int QueueID;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public int MaxLength;

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private T[] _arrayQueue;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private int _first;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private int _last;

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public int Count => _last - _first;

        // --------------- CONSTRUCTOR --------------- //
        public J_ReactiveQueue(int maxLength)
        {
            MaxLength   = maxLength;
            _arrayQueue = new T[MaxLength];
            _first      = 0;
            _last       = 0;
            QueueID     = QueueIDCounter;
            QueueIDCounter++;
        }

        // --------------- MAIN COMMANDS --------------- //
        public void Enqueue(T item)
        {
            int nextIndex = _last.SumRound(1, MaxLength);
            if (nextIndex == _first)
            {
                JLog.Error($"{nameof(J_ReactiveQueue<T>)}_{QueueID} full with {_last + 1} elements. Cancel enqueue.",
                           JLogTags.Collection);

                return;
            }

            _arrayQueue[_last] = item;
            _last              = nextIndex;
            OnEnqueue?.Invoke(item);
        }

        public T Dequeue()
        {
            if (_first == _last)
            {
                JLog.Error($"{nameof(J_ReactiveQueue<T>)}_{QueueID} is empty. Cancel dequeue.", JLogTags.Collection);
                return default;
            }

            T item = Peek();
            _arrayQueue[_first] = default;
            _first              = _first.SumRound(1, MaxLength);
            OnDequeue?.Invoke(item);
            return item;
        }

        public T Peek() => _arrayQueue[_first];

        public void Clear()
        {
            for (int i = _first; i < _last; i++) _arrayQueue[i] = default;

            _first = 0;
            _last  = 0;
        }

        // --------------- QUEUE IMPLEMENTATION --------------- //
        public bool IsSynchronized => false;
        public object SyncRoot => false;

        public void CopyTo(Array array, int index)
        {
            var newArray                                        = (T[])array;
            for (int i = 0; i < Count; i++) newArray[index + i] = _arrayQueue[i];
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = _first; i < _last; i++) yield return _arrayQueue[i];
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        // --------------- SUBSCRIBERS --------------- //
        public void Subscribe(Action<T>   action) { OnEnqueue += action; }
        public void UnSubscribe(Action<T> action) { OnEnqueue -= action; }

        public void SubscribeToEnqueue(Action<T>   action) { Subscribe(action); }
        public void UnSubscribeToEnqueue(Action<T> action) { UnSubscribe(action); }

        public void SubscribeToDequeue(Action<T>   action) { OnDequeue += action; }
        public void UnSubscribeToDequeue(Action<T> action) { OnDequeue -= action; }
    }
}
