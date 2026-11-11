using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;

namespace JReact.ScreenMessage
{
    /// <summary>
    /// a fixed number of message slots, allocated once and reused: when full, the oldest slot takes the next message.
    /// Index 0 is the oldest message.
    /// </summary>
    public sealed class JMessageRing : IReadOnlyList<JMessage>
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        private readonly JMessage[] _slots;
        private int _head; // the oldest message's slot

        public int Count { get; private set; }
        public int Capacity => _slots.Length;
        public bool IsFull => Count == _slots.Length;

        public JMessage this[int index]
        {
            get
            {
                Assert.IsTrue(index >= 0 && index < Count, $"{nameof(JMessageRing)}: index {index} outside {Count} messages");
                return _slots[(_head + index) % _slots.Length];
            }
        }

        public JMessageRing(int capacity)
        {
            Assert.IsTrue(capacity > 0, $"{nameof(JMessageRing)}: capacity must be above 0, it is {capacity}");
            _slots = new JMessage[capacity];
        }

        // --------------- COMMANDS --------------- //
        public void Add(JMessage message)
        {
            Assert.IsFalse(IsFull, $"{nameof(JMessageRing)}: full, remove the oldest first");
            _slots[(_head + Count) % _slots.Length] = message;
            Count++;
        }

        public JMessage RemoveOldest()
        {
            Assert.IsTrue(Count > 0, $"{nameof(JMessageRing)}: empty");
            JMessage removed = _slots[_head];
            _slots[_head] = default; // drops its string references
            _head         = (_head + 1) % _slots.Length;
            Count--;
            return removed;
        }

        public JMessage RemoveNewest()
        {
            Assert.IsTrue(Count > 0, $"{nameof(JMessageRing)}: empty");
            int slot = (_head + Count - 1) % _slots.Length;
            JMessage removed = _slots[slot];
            _slots[slot] = default; // drops its string references
            Count--;
            return removed;
        }

        // --------------- ENUMERATION --------------- //
        // foreach allocates an enumerator: read with Count and the indexer instead.
        public IEnumerator<JMessage> GetEnumerator()
        {
            for (int i = 0; i < Count; i++) { yield return this[i]; }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
