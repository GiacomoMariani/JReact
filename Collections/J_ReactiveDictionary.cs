using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.Collections
{
    public abstract class J_ReactiveDictionary<TKey, TValue> : ScriptableObject,
                                                               iResettable,
                                                               jObservable<(TKey key, TValue value)>
    {
        // --------------- EVENTS --------------- //
        private event Action<(TKey key, TValue value)> OnChange;
        private event Action<(TKey key, TValue value)> OnRemove;

        // --------------- FIELDS --------------- //
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] protected Dictionary<TKey, TValue> _Dictionary { get; } =
            new Dictionary<TKey, TValue>(50);

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public int Count => _Dictionary.Count;

        // --------------- DICTIONARY FIELDS --------------- //
        public bool IsReadOnly => false;
        public ICollection<TKey> Keys => _Dictionary.Keys;
        public ICollection<TValue> Values => _Dictionary.Values;

        // --------------- MAIN ACCESSOR --------------- //
        public TValue this[TKey key] { get => _Dictionary[key]; set => Add(key, value); }

        // --------------- ADD --------------- //
        public void Add(TKey key, TValue value)
        {
            Remove(key);
            Internal_UpdateItem(key, value);
            OnChange?.Invoke((key, value));
        }

        public void Add(KeyValuePair<TKey, TValue> item) => Add(item.Key, item.Value);

        // the specific action to be sent when the dictionary is changed, as default it adds the value
        protected virtual void Internal_UpdateItem(TKey key, TValue value) { _Dictionary[key] = value; }

        // --------------- GETTERS --------------- //
        public bool TryGetValue(TKey key, out TValue value) => _Dictionary.TryGetValue(key, out value);

        // --------------- CHECKS --------------- //
        public bool Contains(KeyValuePair<TKey, TValue> item)  => _Dictionary.Contains(item);
        public bool ContainsKey(TKey                    key)   => _Dictionary.ContainsKey(key);
        public bool ContainsValue(TValue                value) => _Dictionary.ContainsValue(value);

        // --------------- RESET --------------- //
        public void Clear() => _Dictionary.Clear();

        public virtual void ResetThis() => Clear();

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _Dictionary.GetEnumerator();
        // IEnumerator IEnumerable.                       GetEnumerator() => GetEnumerator();

        // --------------- REMOVE --------------- //
        public bool Remove(TKey key)
        {
            if (!_Dictionary.TryGetValue(key, out TValue value)) return false;
            OnRemove?.Invoke((key, value));
            Internal_RemoveItem(key, value);
            return true;
        }

        public bool Remove(KeyValuePair<TKey, TValue> item) => Remove(item.Key);

        // the specific action to be sent when the dictionary is changed, as default it adds the key
        protected virtual void Internal_RemoveItem(TKey key, TValue value) { _Dictionary.Remove(key); }

        // --------------- DICTIONARY IMPLEMENTATION --------------- //
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
            => ((ICollection) _Dictionary).CopyTo(array, arrayIndex);

        #region SUBSCRIBERS
        // --------------- SUBSCRIBERS --------------- //
        public void Subscribe(Action<(TKey key, TValue value)>           action) { OnChange += action; }
        public void UnSubscribe(Action<(TKey key, TValue value)>         action) { OnChange -= action; }
        public void SubscribeToRemove(Action<(TKey key, TValue value)>   action) { OnRemove += action; }
        public void UnSubscribeToRemove(Action<(TKey key, TValue value)> action) { OnRemove -= action; }
        #endregion
    }
}
