using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;

namespace JReact
{
    public static class JNativeArrayExtensions
    {
        /// <summary>
        /// Quickly converts a native array to a managed array. This is more efficient than NativeArray{T}.ToArray for large arrays.
        /// </summary>
        /// <typeparam name="T">A type that is unmanaged</typeparam>
        /// <param name="sourceArray">The native array to be converted</param>
        /// <returns>A new managed array containing the copied data</returns>
        public static unsafe T[] ToManagedArrayFast<T>(this NativeArray<T> sourceArray) where T : unmanaged
        {
            var targetArray = new T[sourceArray.Length];
            fixed (void* targetPtr = targetArray)
            {
                UnsafeUtility.MemCpy(targetPtr,
                                     NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks(sourceArray),
                                     sourceArray.Length * (long)UnsafeUtility.SizeOf<T>());
            }

            return targetArray;
        }

        public static int SafeLength<T>(this NativeArray<T> sourceArray) where T : unmanaged
            => !sourceArray.IsCreated ? 0 : sourceArray.Length;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe T* GetReadonlyPtrTyped<T>(this NativeArray<T> array) where T : unmanaged
            => (T*)array.GetUnsafeReadOnlyPtr();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe T* GetUntypedPtrWithOffset<T>(this NativeArray<T> array, int offset) where T : unmanaged
            => (T*)array.GetUnsafePtr() + (offset * UnsafeUtility.SizeOf<T>());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void* GetUntypedPtrWithOffset<T>(this NativeArray<T>.ReadOnly array, int offset) where T : struct
            => (byte*)array.GetUnsafeReadOnlyPtr() + (offset * UnsafeUtility.SizeOf<T>());

        public static T GetArrayItem<T>(this T[] array, int width, int2 index)    => GetArrayItem(array, width, index.x, index.y);
        public static T GetArrayItem<T>(this T[] array, int width, int  x, int y) => array[(x * width) + y];

        public static void SafeDispose<T>(this NativeArray<T> array) where T : unmanaged
        {
            if (array.IsCreated) { array.Dispose(); }
        }
    }
}
