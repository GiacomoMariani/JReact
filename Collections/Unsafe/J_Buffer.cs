using System;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace JReact.Collections.Unsafe
{
    /// <summary>
    /// Represents an unsafe buffer that stores elements of type T in unmanaged memory.
    /// </summary>
    /// <typeparam name="T">The type of elements to store in the buffer.</typeparam>
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct J_Buffer<T> : IDisposable where T : unmanaged
    {
        /// <summary>
        /// Gets the length of the buffer (number of elements it can hold).
        /// </summary>
        public readonly int Length { get; }

        /// <summary>
        /// Gets a pointer to the start of the buffer.
        /// </summary>
        public T* Ptr { get; private set; }

        /// <summary>
        /// Checks whether the buffer is allocated and created.
        /// </summary>
        public bool IsCreated => (IntPtr)Ptr != IntPtr.Zero;

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The index of the element to get or set.</param>
        /// <returns>The element at the specified index.</returns>
        public T this[int index]
        {
            get
            {
                IsIndexValid(index);
                return *(T*)((long)Ptr + index * sizeof(T));
            }
            set
            {
                IsIndexValid(index);
                *(T*)((long)Ptr + index * sizeof(T)) = value;
            }
        }

        // --------------- CONSTRUCTOR --------------- //

        /// <summary>
        /// Initializes a new instance of the <see cref="J_Buffer{T}"/> struct with the specified size.
        /// </summary>
        /// <param name="size">The number of elements the buffer can hold.</param>
        public J_Buffer(int size)
        {
            Length = size;
            Ptr    = (T*)UnsafeUtility.Malloc(Length * sizeof(T), UnsafeUtility.AlignOf<T>(), Allocator.Persistent);
            UnsafeUtility.MemClear(Ptr, Length * sizeof(T));
        }

        // --------------- METHODS --------------- //

        /// <summary>
        /// Safely disposes of the buffer if it is created.
        /// </summary>
        public void SafeDispose()
        {
            if (!IsCreated) { return; }

            Dispose();
        }

        /// <summary>
        /// Disposes of the buffer and releases the allocated memory.
        /// </summary>
        public void Dispose()
        {
            UnsafeUtility.Free(Ptr, Allocator.Persistent);
            Ptr = (T*)IntPtr.Zero;
        }

        /// <summary>
        /// Copies the contents of this buffer to the specified destination buffer.
        /// </summary>
        /// <param name="destinationBuffer">The destination buffer.</param>
        public void CopyTo(J_Buffer<T> destinationBuffer)
        {
            if (!IsCreated) throw new ObjectDisposedException("The origin buffer was disposed.");
            if (!destinationBuffer.IsCreated) throw new ObjectDisposedException("The destination buffer was disposed.");
            int length = Math.Min(Length, destinationBuffer.Length);
            UnsafeUtility.MemCpy(Ptr, destinationBuffer.Ptr, length * sizeof(T));
        }

        /// <summary>
        /// Generates a deep copy of the specified origin buffer.
        /// </summary>
        /// <param name="origin">The buffer to copy from.</param>
        /// <returns>A new buffer containing a deep copy of the elements in the origin buffer.</returns>
        public J_Buffer<T> GenerateDeepCopy(J_Buffer<T> origin)
        {
            var result = new J_Buffer<T>(origin.Length);
            origin.CopyTo(result);
            return result;
        }

        // --------------- QUERIES --------------- //

        /// <summary>
        /// Checks if the specified index is valid for accessing the buffer.
        /// Throws an exception if the index is out of range.
        /// </summary>
        /// <param name="index">The index to check.</param>
        private void IsIndexValid(int index)
        {
            if (!IsCreated) throw new ObjectDisposedException("Buffer is disposed.");
            if (index >= Length || index < 0)
                throw new IndexOutOfRangeException($"Index {index} is out of range: 0 - {Length - 1}");
        }
    }
}
