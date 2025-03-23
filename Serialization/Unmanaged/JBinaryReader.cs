using System;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace JReact.SaveSystem.Unmanaged
{
    public unsafe struct JBinaryReader : IDisposable
    {
        private UnsafeAppendBuffer.Reader _stream;
        public readonly bool IsCreated => _stream.Ptr != default;

        // --------------- CONSTRUCTORS --------------- //
        public JBinaryReader(J_SO_FileOperator fileOperator, string fileName, bool compressed, int offset = 0)
        {
            byte[] decompressedBytes =
                compressed ? fileOperator.ReadFromFileCompressed(fileName) : fileOperator.ReadFromFile(fileName);

            fixed (byte* bytesPtr = &decompressedBytes[0])
            {
                _stream = new UnsafeAppendBuffer.Reader(bytesPtr + offset, decompressedBytes.Length - offset);
            }
        }

        public JBinaryReader(byte[] source, int offset = 0)
        {
            fixed (byte* bytesPtr = &source[0]) { _stream = new UnsafeAppendBuffer.Reader(bytesPtr + offset, source.Length - offset); }
        }

        //todo not properly tested
        public JBinaryReader(NativeArray<byte> bytesData, int offset)
        {
            byte* bytesPtr = bytesData.GetReadonlyPtrTyped();
            _stream = new UnsafeAppendBuffer.Reader(bytesPtr + offset, bytesData.Length - offset);
        }

        // --------------- OPERATIONS --------------- //
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void AddOffset(int offset) { _stream.Offset += offset; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public T Get<T>() where T : unmanaged => _stream.ReadNext<T>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void GetArray<T>(T* value, int length) where T : unmanaged
        {
            if (length <= 0) { return; }

            int   sizeOfT = UnsafeUtility.SizeOf<T>();
            int   memSize = length * sizeOfT;
            void* ptr     = _stream.ReadNext(memSize);
            UnsafeUtility.MemCpy(value, ptr, memSize);
        }

        public void Dispose()
        {
            if (!_stream.EndOfBuffer)
            {
                JLog.Warning($"Buffer disposed at offset {_stream.Offset}, end of buffer is at {_stream.Size}");
            }
        }
    }
}
