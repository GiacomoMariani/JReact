using System;
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
    /// Represents a list implementation that allows working with unmanaged data in an unsafe manner.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the list.</typeparam>
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct JUnsafeList<T> : IDisposable,
                                          IEnumerable<T>
        where T : unmanaged
    {
        [NativeDisableUnsafePtrRestriction] private UnsafeList<T>* _ListPtr;
        public UnsafeList<T>* ListPtr => _ListPtr;
        public T* First => GetPtrTyped();
        public T* Last => GetPtrTyped() + Length;
        public readonly bool IsCreated => _ListPtr != null && _ListPtr->IsCreated;

        public readonly int Capacity
        {
            get
            {
                Assert.IsTrue(IsCreated);
                return _ListPtr->Capacity;
            }
        }
        public readonly int Length
        {
            get
            {
                Assert.IsTrue(IsCreated);
                return _ListPtr->Length;
            }
        }

        public readonly int SafeLength() => IsCreated ? _ListPtr->Length : 0;

        public readonly bool IsEmpty => !IsCreated || _ListPtr->IsEmpty;

        public readonly int ByteSizeUsed => _ListPtr->Length       * UnsafeUtility.SizeOf<T>();
        public readonly int ByteSizeCapacity => _ListPtr->Capacity * UnsafeUtility.SizeOf<T>();

        // --------------- ACCESSORS --------------- //
        public ref T this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] get => ref UnsafeUtility.ArrayElementAsRef<T>(_ListPtr->Ptr, index);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public ref T ElementAt(int index) => ref this[index];

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public readonly T PeekLast() => ReadonlyElementAt(Length - 1);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public ref T GetLastRef() => ref ElementAt(Length - 1);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly ref readonly T ReadonlyElementAt(int index)
        {
            Assert.IsTrue(IsCreated);
            Assert.IsTrue(index < Length);
            Assert.IsTrue(index >= 0);
            return ref UnsafeUtility.ArrayElementAsRef<T>(_ListPtr->Ptr, index);
        }

        // --------------- POINTERS --------------- //
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void* GetUnsafePtr() => _ListPtr->Ptr;

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public T* GetPtrTyped() => (T*)GetUnsafePtr();

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public readonly void* GetUnsafePtrReadonly() => _ListPtr->Ptr;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly T* GetUnsafePtrTypedReadonly() => (T*)GetUnsafePtrReadonly();

        // --------------- CONSTRUCTORS --------------- //
        /// <summary>
        /// Represents a list that allows unsafe access to its elements in unmanaged memory.
        /// </summary>
        /// <typeparam name="T">The type of elements in the list.</typeparam>
        /// <remarks>
        /// This struct is used to efficiently store and manipulate large collections of unmanaged elements in memory.
        /// </remarks>
        public JUnsafeList(int                initialCapacity, Allocator allocator,
                           NativeArrayOptions options = NativeArrayOptions.UninitializedMemory)
        {
            Assert.IsTrue(allocator       != Allocator.None);
            Assert.IsTrue(initialCapacity > 0);

            _ListPtr = UnsafeList<T>.Create(initialCapacity, allocator, options);
        }

        /// <summary>
        /// Represents a list that allows unsafe access to its elements in unmanaged memory.
        /// </summary>
        /// <typeparam name="T">The type of elements in the list.</typeparam>
        /// <remarks>
        /// This struct is used to efficiently store and manipulate large collections of unmanaged elements in memory.
        /// </remarks>
        public JUnsafeList(Allocator          allocator, NativeArrayOptions options = NativeArrayOptions.UninitializedMemory)
            : this(1, allocator, options)
        {
        }

        /// <summary>
        /// Represents a list that allows unsafe access to its elements in unmanaged memory.
        /// </summary>
        /// <typeparam name="T">The type of elements in the list.</typeparam>
        /// <remarks>
        /// This class is used to efficiently store and manipulate large collections of unmanaged elements in memory.
        /// </remarks>
        public JUnsafeList(void* data, int lengthInElements, Allocator allocator = Allocator.Persistent)
        {
            _ListPtr = AllocatorManager.Allocate<UnsafeList<T>>(allocator);
            UnsafeUtility.MemClear(_ListPtr, UnsafeUtility.SizeOf<UnsafeList<T>>());

            *_ListPtr = new UnsafeList<T>((T*)data, lengthInElements);
        }

        // --------------- CAPACITY --------------- //
        public void SetCapacity(int capacity)
        {
            Assert.IsTrue(IsCreated);
            Assert.IsTrue(capacity > 0);

            _ListPtr->SetCapacity(capacity);
        }

        public void EnsureCapacity(int capacity)
        {
            if (Capacity < capacity) { SetCapacity(capacity); }
        }

        // --------------- ADD --------------- //
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(in T value)
        {
            Assert.IsTrue(IsCreated);
            _ListPtr->Add(in value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryAddNoResize(in T value)
        {
            Assert.IsTrue(IsCreated);
            int nextSlot = Length + 1;
            if (nextSlot <= Capacity)
            {
                _ListPtr->AddNoResize(value);
                return true;
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ParallelAdd(in T value)
        {
            int nextIndex = Interlocked.Increment(ref _ListPtr->m_length) - 1;
            this[nextIndex] = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ParallelTryAddNoResize(in T value)
        {
            Assert.IsTrue(IsCreated);
            int nextIndex = Interlocked.Increment(ref _ListPtr->m_length) - 1;
            if (nextIndex < _ListPtr->Capacity)
            {
                this[nextIndex] = value;
                return true;
            }
            else
            {
                Interlocked.CompareExchange(ref _ListPtr->m_length, nextIndex, nextIndex + 1);
                return false;
            }
        }

        public void Insert(in T value, int index)
        {
            UnityEngine.Assertions.Assert.IsTrue(IsCreated);
            Assert.IsTrue(index < Length);
            Resize(Length                                  + 1);
            byte* beginPointer      = (byte*)_ListPtr->Ptr + (UnsafeUtility.SizeOf<T>() * index);
            byte* newPositionPoiner = beginPointer         + (UnsafeUtility.SizeOf<T>());
            int   sizeToMove        = (Length - index) * UnsafeUtility.SizeOf<T>();
            UnsafeUtility.MemMove(newPositionPoiner, beginPointer, sizeToMove);
            this[index] = value;
        }

        // --------------- ADD OTHER STRUCTURES --------------- //

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddUnsafeList(JUnsafeList<T> unsafeList)
        {
            Assert.IsTrue(IsCreated);
            if (unsafeList.IsCreated) { _ListPtr->AddRange(*unsafeList._ListPtr); }
        }

        [BurstDiscard, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddArray(T[] array)
        {
            Assert.IsTrue(IsCreated);
            if (array == default) { return; }

            int valuesLength = array.Length;
            for (int i = 0; i < valuesLength; ++i) { _ListPtr->Add(in array[i]); }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddNativeArray(NativeArray<T>.ReadOnly nativeArray)
        {
            Assert.IsTrue(IsCreated);
            _ListPtr->AddRange(nativeArray.GetUnsafeReadOnlyPtr(), nativeArray.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ParallelTryAddNoResize(NativeArray<T>.ReadOnly nativeArray)
            => ParallelTryAddNoResize(nativeArray, 0, nativeArray.Length);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ParallelTryAddNoResize(NativeArray<T>.ReadOnly array, int start, int amount)
        {
            Assert.IsTrue(IsCreated);
            Assert.IsTrue(array.Length >= amount);
            int startListIndex = _ListPtr->m_length;
            int finalListIndex = Interlocked.Add(ref _ListPtr->m_length, amount);

            if (start >= _ListPtr->Capacity)
            {
                Interlocked.CompareExchange(ref _ListPtr->m_length, startListIndex, finalListIndex);
                return false;
            }
            else
            {
                int   sizeofT     = UnsafeUtility.SizeOf<T>();
                void* destination = (byte*)_ListPtr->Ptr + startListIndex * sizeofT;
                UnsafeUtility.MemCpy(destination, array.GetUntypedPtrWithOffset(start), amount * sizeofT);
                return true;
            }
        }

        // --------------- RESIZE AND FILL --------------- //
        public void Resize(int newSize, NativeArrayOptions options = NativeArrayOptions.UninitializedMemory)
        {
            Assert.IsTrue(IsCreated);
            Assert.IsTrue(newSize >= 0);
            _ListPtr->Resize(newSize, options);
        }

        public void ResizeFill(int length, in T defaultItem, NativeArrayOptions options = NativeArrayOptions.UninitializedMemory)
        {
            var oldLength = Length;
            Resize(length, options);

            var fillSize = length - oldLength;
            if (fillSize > 0) { Fill(in defaultItem, oldLength, fillSize); }
        }

        public void Fill(in T fillValue) { Fill(in fillValue, 0, Length); }

        public void Fill(in T fillValue, int start, int count)
        {
            var   data    = fillValue;
            var   sizeOfT = UnsafeUtility.SizeOf<T>();
            byte* ptr     = (byte*)GetUnsafePtr() + (sizeOfT * start);
            UnsafeUtility.MemCpyReplicate(ptr, UnsafeUtility.AddressOf(ref data), UnsafeUtility.SizeOf<T>(), count);
        }

        public void MemCpy(in JUnsafeList<T> otherList)
        {
            Assert.IsTrue(IsCreated);
            Assert.IsTrue(otherList.IsCreated);
            Assert.IsTrue(Capacity >= otherList.Length);

            UnsafeUtility.MemCpy(GetUnsafePtr(), otherList.GetUnsafePtr(), ByteSizeUsed);
        }

        // --------------- REMOVAL --------------- //
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void RemoveAt(int index) { _ListPtr->RemoveAt(index); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveRange(int index, int count) { _ListPtr->RemoveRange(index, count); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void RemoveLastFast() { --_ListPtr->Length; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly T PopLast()
        {
            T last = ReadonlyElementAt(Length - 1);
            --_ListPtr->Length;
            return last;
        }

        // --------------- CLEAR AND DISPOSE --------------- //
        public void Dispose()
        {
            if (_ListPtr != null) { UnsafeList<T>.Destroy(_ListPtr); }

            _ListPtr = null;
        }

        public void Clear() { _ListPtr->Clear(); }

        public void SafeClear()
        {
            if (IsCreated)
            {
                _ListPtr->Clear();
            }
        }

        public void ZeroFillUsedMemory()
        {
            Assert.IsTrue(IsCreated);
            UnsafeUtility.MemClear(_ListPtr->Ptr, ByteSizeUsed);
        }

        // --------------- COMPARISON --------------- //
        /// <summary>
        /// Compares the memory contents of two JUnsafeList<T> objects.
        /// </summary>
        /// <param name="other">The JUnsafeList<T> object to compare to.</param>
        /// <returns>True if the memory contents of both JUnsafeList<T> objects are equal, false otherwise.</returns>
        public bool MemEquals(in JUnsafeList<T> other)
        {
            if (Length != other.Length) { return false; }

            var memCmpResult = UnsafeUtility.MemCmp(First, other.First, ByteSizeUsed);
            return memCmpResult == 0;
        }

        public bool MemEqualsSafe(in JUnsafeList<T> other)
        {
            if (IsCreated != other.IsCreated) { return false; }

            if (!IsCreated) { return true; }

            return MemEquals(in other);
        }

        // --------------- CONVERTERS FROM --------------- //
        public static JUnsafeList<T> FromArray(T[] data, Allocator allocator)
        {
            var list = new JUnsafeList<T>(data.Length, allocator);
            if (data.Length > 0) { list.AddArray(data); }

            return list;
        }

        public static JUnsafeList<T> FromUnsafeList(JUnsafeList<T> other, Allocator allocator)
        {
            Assert.IsTrue(other.IsCreated);
            var list = new JUnsafeList<T>(other.Capacity, allocator);
            list._ListPtr = UnsafeList<T>.Create(math.max(other.Capacity, other.Length), allocator);

            list._ListPtr->AddRange(*other.ListPtr);
            return list;
        }

        // --------------- CONVERTERS AS --------------- //
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly NativeArray<T> AsNativeArray()
        {
            if (IsCreated)
            {
                NativeArray<T> nativeArray =
                    NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<T>(GetUnsafePtrReadonly(), Length, Allocator.None);

                return nativeArray;
            }
            else { return default; }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly NativeArray<T> AsNativeArrayRange(int startIndex, int amountOfItems)
        {
            Assert.IsTrue(startIndex                 >= 0);
            Assert.IsTrue(startIndex                 < _ListPtr->Length);
            Assert.IsTrue(amountOfItems              > 0);
            Assert.IsTrue(startIndex + amountOfItems <= _ListPtr->Length);

            if (IsCreated)
            {
                var sizeofT = UnsafeUtility.SizeOf<T>();
                var nativeArray =
                    NativeArrayUnsafeUtility.
                        ConvertExistingDataToNativeArray<T>((byte*)GetUnsafePtrReadonly() + (startIndex * sizeofT),
                                                            amountOfItems,
                                                            Allocator.None);

                return nativeArray;
            }
            else
            {
                return default;
            }
        }

        // AsParallelReader Is Compatible method with NativeList<T>.AsParallelReader
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly NativeArray<T>.ReadOnly AsParallelReader() => AsNativeArray().AsReadOnly();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator NativeArray<T>(JUnsafeList<T> nativeList) => nativeList.AsNativeArray();

        // --------------- UTILITIES --------------- //
        /// <summary>
        /// Reinterprets the memory contents of a JUnsafeList<T> object as a JUnsafeList<U> object.
        /// </summary>
        /// <typeparam name="TTarget">The type to reinterpret the memory contents as. Must be unmanaged.</typeparam>
        /// <returns>A new JUnsafeList<U> object with the same underlying memory as the original list.</returns>
        public unsafe JUnsafeList<TTarget> Reinterpret<TTarget>()
            where TTarget : unmanaged
        {
            Assert.IsTrue(IsCreated);
            var sizeofU = UnsafeUtility.SizeOf<TTarget>();
            var sizeofT = UnsafeUtility.SizeOf<T>();
            Assert.IsTrue(sizeofT == sizeofU);

            var list = new JUnsafeList<TTarget> { _ListPtr = (UnsafeList<TTarget>*)_ListPtr, };

            return list;
        }

        // --------------- IENUMERATORS --------------- //
        [BurstDiscard] IEnumerator<T> IEnumerable<T>.GetEnumerator() => new JMemoryEnumerator<T>(GetPtrTyped(), Length);

        [BurstDiscard] public IEnumerator GetEnumerator() => new JMemoryEnumerator<T>(GetPtrTyped(), Length);
    }
}
