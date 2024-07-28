using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine.Assertions;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;

namespace JReact.UnmanagedCollection
{
    /// <summary>
    /// Represents a span-based list of values in unmanaged memory.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the list.</typeparam>
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct JSpanList<T> : IEnumerable<T> where T : unmanaged
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        public readonly T* listPtr;
        public readonly T* First => listPtr;
        public readonly T* Last => listPtr + _length;
        public readonly bool IsCreated => listPtr != default;

        public readonly int Capacity;
        private int _length;
        public readonly int Length
        {
            get
            {
                Assert.IsTrue(_length >= 0);
                Assert.IsTrue(_length <= Capacity);
                return _length;
            }
        }
        public readonly bool IsEmpty => _length == 0;
        public readonly bool IsFull => _length  == Capacity;
        public readonly bool CanAdd => _length  < Capacity;
        public readonly int ByteSizeUsed => Length       * sizeof(T);
        public readonly int ByteSizeCapacity => Capacity * sizeof(T);

        // --------------- ACCESSORS --------------- //
        public ref T this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] get
            {
                Assert.IsTrue(IsCreated);
                Assert.IsTrue(index >= 0);
                Assert.IsTrue(index < _length);

                return ref listPtr[index];
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public ref T ElementAt(int index) => ref this[index];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly ref readonly T ReadonlyElementAt(int index) => ref listPtr[index];

        // --------------- CONSTRUCTOR --------------- //
        /// <summary>
        /// Represents a span-based list of unmanaged items.
        /// </summary>
        /// <typeparam name="T">The type of the items in the list.</typeparam>
        /// <remarks>
        /// This list provides a span-based implementation of a list for unmanaged items.
        /// </remarks>
        /// <seealso cref="System.Collections.Generic.IEnumerable{T}" />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public JSpanList(System.Span<T> span)
        {
            Capacity = span.Length;
            _length  = 0;
            fixed (T* spanDataPtr = span) { listPtr = spanDataPtr; }
        }

        /// <summary>
        /// Represents a span-based list of unmanaged items.
        /// </summary>
        /// <typeparam name="T">The type of the items in the list.</typeparam>
        /// <remarks>
        /// This list provides a span-based implementation of a list for unmanaged items.
        /// </remarks>
        /// <seealso cref="System.Collections.Generic.IEnumerable{T}" />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public JSpanList(int capacity)
        {
            Assert.IsTrue(capacity >= 0);
            Capacity = capacity;
            _length  = 0;
            var spanData = new System.Span<T>(new T[capacity]);
            fixed (T* spanDataPtr = spanData) { listPtr = spanDataPtr; }
        }

        // --------------- ADD COMMANDS --------------- //
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(in T value)
        {
            Assert.IsTrue(IsCreated);
            Assert.IsTrue(CanAdd);
            this[_length] = value;
            _length++;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ParallelAdd(in T value)
        {
            Assert.IsTrue(IsCreated);
            Assert.IsTrue(CanAdd);
            int index = Interlocked.Increment(ref _length) - 1;
            this[index] = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryAdd(in T value)
        {
            Assert.IsTrue(IsCreated);
            if (_length < Capacity)
            {
                int index = _length++;
                this[index] = value;
                return true;
            }
            else { return false; }
        }

        /// <summary>Unsafe method, Length could become greater or equal to capacity.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryAddParallel(in T value)
        {
            Assert.IsTrue(IsCreated);
            Assert.IsTrue(CanAdd);
            int index = Interlocked.Increment(ref _length) - 1;
            if (index < Capacity)
            {
                this[index] = value;
                return true;
            }
            else
            {
                Interlocked.CompareExchange(ref _length, index, index + 1);
                return false;
            }
        }

        /// <summary>
        /// Adds the elements of a NativeArray to the JSpanList.
        /// </summary>
        /// <typeparam name="T">The type of the items in the list.</typeparam>
        /// <param name="array">The NativeArray to add to the list.</param>
        /// <remarks>
        /// This method adds the elements of the specified NativeArray to the JSpanList.
        /// The JSpanList must be created and have sufficient capacity to accommodate the new elements.
        /// If the length of the array is less than 0, nothing is added to the JSpanList.
        /// </remarks>
        /// <seealso cref="JReact.UnmanagedCollection.JSpanList{T}" />
        /// <seealso cref="Unity.Collections.NativeArray{T}" />
        /// <seealso cref="Unity.Collections.LowLevel.Unsafe.UnsafeUtility.MemCpy(void*,void*,int)" />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddArray(NativeArray<T> array)
        {
            Assert.IsTrue(IsCreated);
            int arrayLength = array.SafeLength();
            if (arrayLength < 0) { return; }

            Assert.IsTrue(_length + arrayLength <= Capacity);
            UnsafeUtility.MemCpy(First + _length, array.GetReadonlyPtrTyped(), arrayLength * UnsafeUtility.SizeOf<T>());
            _length += arrayLength;
        }

        /// <summary>
        /// Fills a range of elements in the JSpanList with a specified value.
        /// </summary>
        /// <typeparam name="T">The type of the items in the JSpanList.</typeparam>
        /// <param name="value">The value to fill the range with.</param>
        /// <param name="startIndex">The index at which to start filling the range. The default value is 0.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Fill(in T value, int startIndex = 0) => FillRange(in value, startIndex, _length);

        /// <summary>
        /// Fills a range of the JSpanList with the specified value.
        /// </summary>
        /// <typeparam name="T">The type of items in the JSpanList.</typeparam>
        /// <param name="value">The value to fill the range with.</param>
        /// <param name="startIndex">The starting index of the range.</param>
        /// <param name="amount">The number of items in the range to fill with the specified value.</param>
        /// <seealso cref="JSpanList{T}" />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void FillRange(in T value, int startIndex, int amount)
        {
            Assert.IsTrue(IsCreated);
            Assert.IsTrue(amount     <= _length);
            Assert.IsTrue(startIndex >= 0);
            int endIndex = startIndex + amount;
            for (int i = startIndex; i < endIndex; ++i) { this[i] = value; }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Resize(int length, in T value = default)
        {
            Assert.IsTrue(IsCreated);
            Assert.IsTrue(length <= Capacity);
            for (int i = _length; i < length; ++i) { this[_length++] = value; }

            _length = length;
        }

        // --------------- REMOVE COMMANDS --------------- //
        /// <summary>
        /// Removes the last element from the JSpanList.
        /// </summary>
        /// <param name="/*START_USER_CODE*/">Any additional parameters needed to remove the last element.</param>
        /// <remarks>
        /// This method removes the last element from the JSpanList. It throws an exception if the list is not created or if the length is zero.
        /// </remarks>
        /// <exception cref="JReact.UnmanagedCollection.JSpanList{T}">Thrown when the JSpanList is not created or the length is zero.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveLastFast()
        {
            Assert.IsTrue(IsCreated);
            Assert.IsTrue(_length > 0);
            _length--;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveAt(int index)
        {
            Assert.IsTrue(IsCreated);
            Assert.IsTrue(index < _length);
            RemoveRange(index, index + 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveRange(int startIndex, int rangeAmount)
        {
            Assert.IsTrue(IsCreated);
            Assert.IsTrue(rangeAmount > 0);
            int copyFrom = math.min(startIndex + rangeAmount, _length);
            var dst      = listPtr + startIndex;
            var src      = listPtr + copyFrom;
            var sizeOf   = UnsafeUtility.SizeOf<T>();
            UnsafeUtility.MemCpy(dst, src, (_length - copyFrom) * sizeOf);
            _length -= rangeAmount;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            Assert.IsTrue(IsCreated);
            _length = 0;
        }

        // --------------- QUERIES --------------- //
        [BurstDiscard, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(JSpanList<T> a, JSpanList<T> b) => EqualsImpl(a, b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(JSpanList<T> a, JSpanList<T> b) => !EqualsImpl(a, b);

        [BurstDiscard, MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool EqualsImpl(JSpanList<T> a, JSpanList<T> b)
        {
            if (a._length != b._length) { return false; }

            for (int i = 0; i < a._length; ++i)
            {
                if (!a[i].Equals(b.ReadonlyElementAt(i))) { return false; }
            }

            return true;
        }

        public bool MemoryEquals(in JSpanList<T> other)
        {
            if (Length != other.Length) { return false; }

            var memCmpResult = UnsafeUtility.MemCmp(First, other.First, ByteSizeUsed);
            return memCmpResult == 0;
        }

        [BurstDiscard]
        public override bool Equals(object obj)
        {
            if (obj is T other) { return other.Equals(this); }
            else { return false; }
        }

        public override readonly int GetHashCode() => (int)math.hash(First, UnsafeUtility.SizeOf<JSpanList<T>>());

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void* GetPtr() => First;

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public T* GetPtrTyped() => First;

        // --------------- CONVERSIONS --------------- //
        [BurstDiscard]
        public readonly T[] ToArray()
        {
            var values = new T[_length];
            for (int i = 0; i < _length; ++i) { values[i] = ReadonlyElementAt(i); }

            return values;
        }

        public readonly NativeArray<T> ToNativeArray(Allocator allocator)
        {
            var values = new NativeArray<T>(Length, allocator);
            for (int i = 0; i < _length; ++i) { values[i] = ReadonlyElementAt(i); }

            return values;
        }

        // --------------- ENUMERATORS --------------- //
        public IEnumerator<T> GetEnumerator()
        {
            Assert.IsTrue(IsCreated);
            return new JMemoryEnumerator<T>(First, Length);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
