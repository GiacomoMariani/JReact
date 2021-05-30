#if UNITY_ADDRESSABLES
using Unity.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Assertions;
#if UNITY_UNITASK
using Cysharp.Threading.Tasks;
#else
using System.Threading.Tasks;
#endif

namespace JReact.J_Addressables
{
    public static class J_Addressable_Utils
    {
#if UNITY_UNITASK
        public static async UniTask<bool> AddressableExist(string location, int expected = 1)
#else
        public static async Task<bool> AddressableExist(string location, int expected = 1)
#endif
        {
            var task = Addressables.LoadResourceLocationsAsync(location);
            await task.Task;
            Assert.IsTrue(task.Status == AsyncOperationStatus.Succeeded, $"Operation failed: {location}.\n{task.OperationException}");
            return task.Result.Count == expected;
        }

#if UNITY_UNITASK
        public static async UniTask<NativeArray<byte>> AddressableToByte(string path, Allocator allocator = Allocator.Temp)
#else
        public static async Task<NativeArray<byte>> AddressableToByte(string path, Allocator allocator = Allocator.Temp)
#endif
        {
            AsyncOperationHandle<TextAsset> task      = Addressables.LoadAssetAsync<TextAsset>(path);
            TextAsset                       textAsset = await task.Task;
            Assert.IsTrue(task.Status == AsyncOperationStatus.Succeeded, $"Op failed {path}.\n{task.OperationException}");
            var bytes = new NativeArray<byte>(textAsset.bytes, allocator);
            Addressables.Release(task);
            return bytes;
        }
    }
}
#endif
