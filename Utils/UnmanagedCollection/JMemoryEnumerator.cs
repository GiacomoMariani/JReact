using System.Collections;
using System.Collections.Generic;

namespace JReact.UnmanagedCollection
{
    public unsafe struct JMemoryEnumerator<T> : IEnumerator<T>
        where T : unmanaged
    {
        private int _index;
        private readonly int Length;
        private readonly T* _ptr;

        public T Current => _ptr[_index];

        object IEnumerator.Current => Current;

        public JMemoryEnumerator(T* ptr, int length)
        {
            _ptr   = ptr;
            Length = length;
            _index = -1;
        }

        public bool MoveNext()
        {
            _index++;
            return Length > _index;
        }

        public void Reset()   { _index = -1; }
        public void Dispose() {}
    }
}
