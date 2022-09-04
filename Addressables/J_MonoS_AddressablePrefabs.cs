#if UNITY_ADDRESSABLES && UNITY_UNITASK
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using JReact.Singleton;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace JReact.J_Addressables
{
    /// <summary>
    /// manages, loads and unloads addressables as prefabs
    /// keeps track of the operation and of the loaded/unloaded assets
    /// </summary>
    public sealed class J_MonoS_AddressablePrefabs : J_MonoSingleton<J_MonoS_AddressablePrefabs>
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector]
        private readonly Dictionary<AssetReference, HashSet<GameObject>> _spawnedItems =
            new Dictionary<AssetReference, HashSet<GameObject>>();

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector]
        private readonly Dictionary<AssetReference, AsyncOperationHandle<GameObject>> _asyncHandles =
            new Dictionary<AssetReference, AsyncOperationHandle<GameObject>>();

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector]
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public async UniTask<GameObject> Spawn(AssetReference assetReference, Vector3 pos, Quaternion rotation)
        {
            if (_asyncHandles.ContainsKey(assetReference))
            {
                AsyncOperationHandle<GameObject> handleStart = _asyncHandles[assetReference];
                await handleStart
                     .WithCancellation(_cancellationTokenSource.Token)
                     .ToAsyncLazy();
            }
            else
            {
                AsyncOperationHandle<GameObject> handleStart = Addressables.LoadAssetAsync<GameObject>(assetReference);

                _asyncHandles[assetReference] = handleStart;
                await handleStart
                     .WithCancellation(_cancellationTokenSource.Token)
                     .ToAsyncLazy();
            }

            return await SpawnFromReference(assetReference, pos, rotation);
        }

        public async UniTask<GameObject> SpawnOnTransform(AssetReference assetReference, Transform parent)
        {
            var spawnedItem = await Spawn(assetReference, JConstants.Vector3Zero, JConstants.quarterionIdentity);

            spawnedItem.transform.PlaceOnParent(parent);

            return spawnedItem;
        }

        private async UniTask<GameObject> SpawnFromReference(AssetReference assetReference, Vector3 position, Quaternion rotation)
        {
            GameObject spawnedItem = await assetReference.InstantiateAsync(position, rotation);
            if (_spawnedItems.ContainsKey(assetReference) == false) { _spawnedItems[assetReference] = new HashSet<GameObject>(); }

            _spawnedItems[assetReference].Add(spawnedItem);
            return spawnedItem;
        }

        public void Release(AssetReference assetReference, GameObject item)
        {
            Addressables.ReleaseInstance(item);

            _spawnedItems[assetReference].Remove(item);
            if (_spawnedItems[assetReference].Count != 0) { return; }

            if (_asyncHandles[assetReference].IsValid()) { Addressables.Release(_asyncHandles[assetReference]); }

            _asyncHandles.Remove(assetReference);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _cancellationTokenSource.Cancel();

            foreach (var spawnedItem in _spawnedItems)
            {
                var       reference = spawnedItem.Key;
                using var iterator  = spawnedItem.Value.GetEnumerator();
                while (iterator.MoveNext()) { Addressables.ReleaseInstance(iterator.Current); }
            }

            Resources.UnloadUnusedAssets();
            _asyncHandles.Clear();
            _spawnedItems.Clear();
        }
    }
}
#endif
