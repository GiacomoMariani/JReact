using System;
using System.Collections.Generic;

namespace JReact
{
    public sealed class JTwoWayMap<TKey, TValue>
    {
        public static readonly JTwoWayMap<TKey, TValue> Default = new();
        
        private readonly Dictionary<TKey, TValue> _keyToValue;
        private readonly Dictionary<TValue, TKey> _valueToKey;

        public JTwoWayMap(IEqualityComparer<TKey>? keyComparer = null, IEqualityComparer<TValue>? valueComparer = null)
        {
            _keyToValue = new Dictionary<TKey, TValue>(keyComparer);
            _valueToKey = new Dictionary<TValue, TKey>(valueComparer);
        }

        public int Count => _keyToValue.Count;

        public IEnumerable<TKey> Keys => _keyToValue.Keys;
        public IEnumerable<TValue> Values => _valueToKey.Keys;

        public bool ContainsKey(TKey     key)   => _keyToValue.ContainsKey(key);
        public bool ContainsValue(TValue value) => _valueToKey.ContainsKey(value);

        public bool TryGetValue(TKey key, out TValue value) => _keyToValue.TryGetValue(key, out value!);

        public bool TryGetKey(TValue value, out TKey key) => _valueToKey.TryGetValue(value, out key!);

        public TValue GetValue(TKey key)   => _keyToValue[key];
        public TKey   GetKey(TValue value) => _valueToKey[value];

        public void Add(TKey key, TValue value)
        {
            if (_keyToValue.ContainsKey(key)) throw new ArgumentException("Key is already mapped.", nameof(key));

            if (_valueToKey.ContainsKey(value)) throw new ArgumentException("Value is already mapped.", nameof(value));

            _keyToValue.Add(key, value);
            _valueToKey.Add(value, key);
        }

        public bool RemoveByKey(TKey key)
        {
            if (!_keyToValue.TryGetValue(key, out var value)) return false;

            _keyToValue.Remove(key);
            _valueToKey.Remove(value);
            return true;
        }

        public bool RemoveByValue(TValue value)
        {
            if (!_valueToKey.TryGetValue(value, out var key)) return false;

            _valueToKey.Remove(value);
            _keyToValue.Remove(key);
            return true;
        }

        public void Clear()
        {
            _keyToValue.Clear();
            _valueToKey.Clear();
        }
    }
}
