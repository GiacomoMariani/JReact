#if UNITY_ADDRESSABLES && UNITY_UNITASK
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using JReact.Singleton;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;

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
        private readonly Dictionary<AssetReference, UniTask<GameObject>> _asyncHandles =
            new Dictionary<AssetReference, UniTask<GameObject>>();

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector]
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public async UniTask<GameObject> Spawn(AssetReference assetReference, Vector3 pos, Quaternion rotation)
        {
            if (!assetReference.RuntimeKeyIsValid())
            {
                JLog.Error($"{name} - Key not valid {assetReference.RuntimeKey}", JLogTags.Resources, this);
                return default;
            }

            if (_asyncHandles.ContainsKey(assetReference))
            {
                UniTask<GameObject> handle = _asyncHandles[assetReference];
                await handle;
            }
            else
            {
                var handle = Addressables.LoadAssetAsync<GameObject>(assetReference).WithCancellation(_cancellationTokenSource.Token);
                _asyncHandles[assetReference] = handle;
                await handle;
            }

            return await SpawnFromReference(assetReference, pos, rotation);
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

            Addressables.Release(_asyncHandles[assetReference]);
            _asyncHandles.Remove(assetReference);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _cancellationTokenSource.Cancel();
            _asyncHandles.Clear();

            foreach (var spawnedItem in _spawnedItems)
            {
                var       reference = spawnedItem.Key;
                using var iterator  = spawnedItem.Value.GetEnumerator();
                while (iterator.MoveNext()) { Release(reference, iterator.Current); }
            }

            _spawnedItems.Clear();
        }
    }
}
#endif
