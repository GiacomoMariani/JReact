using System;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Collections.LowLevel.Unsafe.NotBurstCompatible;

namespace JReact.SaveSystem.Unmanaged
{
    [BurstCompile]
    public unsafe struct JBinaryWriter : IDisposable
    {
        private UnsafeAppendBuffer _buffer;
        public bool IsCreated => _buffer.IsCreated;

        // --------------- CONSTRUCTORS --------------- //
        public JBinaryWriter(int expectedCapacity)
            => _buffer = new UnsafeAppendBuffer(expectedCapacity, sizeof(ulong), Allocator.Persistent);

        public JBinaryWriter(void* voidPtr, int expectedCapacity) => _buffer = new UnsafeAppendBuffer(voidPtr, expectedCapacity);

        // --------------- CONVERSIONS --------------- //
        public byte[]            AsByteArray()                          => _buffer.ToBytesNBC();
        public NativeArray<byte> AsByteNativeArray(Allocator allocator) => new NativeArray<byte>(_buffer.ToBytesNBC(), allocator);

        public ArraySegment<byte> AsByteSegment => new ArraySegment<byte>(AsByteArray(), 0, _buffer.Length);

        // --------------- OPERATIONS --------------- //
        [BurstCompile, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public JBinaryWriter Add<T>(in T value) where T : unmanaged
        {
            _buffer.Add(value);
            return this;
        }

        [BurstCompile, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public JBinaryWriter AddUnmanaged<T>(T* value, int length) where T : unmanaged
        {
            if (length <= 0) { return this; }

            int sizeOfT = UnsafeUtility.SizeOf<T>();
            _buffer.Add(value, sizeOfT * length);
            return this;
        }

        [BurstDiscard]
        public void ToFile(J_SO_FileOperator fileOperator, string fileName, bool compress)
        {
            ArraySegment<byte> byteSegment = AsByteSegment;
            if (compress) { fileOperator.WriteToFile(fileName, byteSegment.Array, byteSegment.Count, byteSegment.Offset); }
            else { fileOperator.WriteToFileCompressed(fileName, byteSegment.Array, byteSegment.Count, byteSegment.Offset); }

            Dispose();
        }

        public void Dispose() { _buffer.Dispose(); }
    }
}
