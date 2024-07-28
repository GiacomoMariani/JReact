#if UNITY_ADDRESSABLES
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Assertions;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace JReact.SceneControl
{
    [CreateAssetMenu(menuName = "Reactive/Scenes/AddressableScene")]
    public class J_SO_AddressableScene : ScriptableObject, IJScene
    {
        // --------------- SETUP --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField] private AssetReference _sceneAddress;
        public string SceneId => _sceneAddress.AssetGUID;

        // --------------- STATE --------------- //
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public bool IsInMemory
            => _ScenesInMemory.ContainsKey(_sceneAddress.AssetGUID);
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector]
        private static Dictionary<string, SceneInstance> _ScenesInMemory = new Dictionary<string, SceneInstance>(4);

        // --------------- OPERATIONS --------------- //
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private AsyncOperationHandle<SceneInstance> _loadOperation;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public float PercentageLoading
            => _loadOperation.IsValid() ? _loadOperation.PercentComplete : 0;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public bool IsLoaded
            => _loadOperation.IsValid() && _loadOperation.IsDone;

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private AsyncOperationHandle<SceneInstance> _unloadOperation;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public float UnPercentageLoading
            => _unloadOperation.IsValid() ? _unloadOperation.PercentComplete : 0;

        // --------------- LOADING --------------- //
        /// <summary>
        /// Asynchronously loads an addressable scene.
        /// </summary>
        /// <param name="loadMode">The load mode for the scene.</param>
        /// <returns>A UniTask representing the asynchronous load operation.</returns>
        public async UniTask LoadSceneAsync(LoadSceneMode loadMode)
        {
            Assert.IsFalse(_ScenesInMemory.ContainsKey(_sceneAddress.AssetGUID), $"{this} was already loaded");
            Log($"{this} - Load Addressable Scene {_sceneAddress.AssetGUID}");
            //we reset the opposite operation to confirm we're currently changing state
            _unloadOperation = default;
            _loadOperation   = Addressables.LoadSceneAsync(_sceneAddress, loadMode);
            SceneInstance sceneInstance = await _loadOperation;
            _ScenesInMemory[_sceneAddress.AssetGUID] = sceneInstance;
        }

        // --------------- UNLOADING --------------- //
        /// <summary>
        /// Safely unloads an addressable scene asynchronously.
        /// </summary>
        /// <param name="options">The unload options for the scene.</param>
        /// <returns>A UniTask representing the asynchronous unload operation.</returns>
        public async UniTask UnloadSceneAsync(UnloadSceneOptions options = UnloadSceneOptions.UnloadAllEmbeddedSceneObjects)
        {
            if (!IsInMemory)
            {
                Warning($"Not in memory. Cancel command. Total in memory: {_ScenesInMemory.Count}");
                return;
            }

            Log($"{this} - Request Unload Addressable Scene {_sceneAddress.AssetGUID}");
            //we reset the opposite operation to confirm we're currently changing state
            _loadOperation = default;
            SceneInstance sceneInstance = _ScenesInMemory[_sceneAddress.AssetGUID];
            _unloadOperation = Addressables.UnloadSceneAsync(sceneInstance, options);
            await _unloadOperation;
            _ScenesInMemory.Remove(_sceneAddress.AssetGUID);
        }

        // --------------- RESET SCENES --------------- //
        private static readonly List<UniTask> _ListCache = new List<UniTask>();

        /// <summary>
        /// Removes all scenes from memory that are currently loaded and tracked by the addressable scene manager.
        /// </summary>
        /// <returns>A UniTask representing the asynchronous reset operation.</returns>
        public static async UniTask RemoveAllSceneFromMemory()
        {
            _ListCache.Clear();

            using Dictionary<string, SceneInstance>.Enumerator sceneEnumerator = _ScenesInMemory.GetEnumerator();
            while (sceneEnumerator.MoveNext())
            {
                _ListCache.Add(Addressables.UnloadSceneAsync(sceneEnumerator.Current.Value,
                                                             UnloadSceneOptions.UnloadAllEmbeddedSceneObjects).
                                            ToUniTask());
            }

            _ScenesInMemory.Clear();

            await UniTask.WhenAll(_ListCache);
        }

        /// <summary>
        /// Resets all loaded scenes from memory.
        /// </summary>
        /// <returns>A UniTask representing the asynchronous reset operation.</returns>
        public static async UniTask ResetAll() => await RemoveAllSceneFromMemory();

        // --------------- UTILITIES --------------- //
        public override string ToString()              => $"{name}: {_sceneAddress.AssetGUID}";
        private         void   Log(string     message) => JLog.Log($"{this} - {message}", JLogTags.SceneManager, this);
        private         void   Warning(string message) => JLog.Warning($"{this} - {message}", JLogTags.SceneManager, this);
    }
}
#endif
