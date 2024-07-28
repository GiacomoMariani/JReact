using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

namespace JReact.SceneControl
{
    public static class JSceneUtils
    {
        private const string DontDestroyOnLoadScene = "DontDestroyOnLoad";

        private static List<UniTask> _loadOperations = new List<UniTask>(2);
        private static List<UniTask> _UnloadOperations = new List<UniTask>(2);
        public static bool IsLoading { get; private set; }
        public static bool IsUnloading { get; private set; }
        public static bool IsBusy => IsLoading || IsUnloading;

        public static async UniTask LoadTogether(IJScene[] scenes, bool removePreviousScenes = true, IJScene loadingScene = default)
        {
            Assert.IsFalse(IsLoading, $"Loading operation already running");
            IsLoading = true;
            _loadOperations.Clear();
            if (removePreviousScenes)
            {
                if (loadingScene != default) { await loadingScene.LoadSceneAsync(LoadSceneMode.Single); }
                else { _loadOperations.Add(UnloadAllScenes()); }
            }

            for (int i = 0; i < scenes.Length; i++) { _loadOperations.Add(scenes[i].LoadSceneAsync(LoadSceneMode.Additive)); }

            await UniTask.WhenAll(_UnloadOperations);

            if (loadingScene != default) { await loadingScene.UnloadSceneAsync(UnloadSceneOptions.UnloadAllEmbeddedSceneObjects); }

            IsLoading = false;
        }

        public static async UniTask UnloadAllScenes()
        {
            Assert.IsFalse(IsUnloading, $"Unloading operation already running");
            IsUnloading = true;
            _UnloadOperations.Clear();
            var totalScenes = SceneManager.loadedSceneCount;
            for (int i = 0; i < totalScenes; i++)
            {
                Scene sceneId = SceneManager.GetSceneAt(i);
                _UnloadOperations.Add(SceneManager.UnloadSceneAsync(sceneId, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects).
                                                   ToUniTask());
            }

            await UniTask.WhenAll(_UnloadOperations);
            IsUnloading = false;
        }

        public static void FullyCleanScene()
        {
            GameObject[] objects = Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID);

            foreach (GameObject obj in objects) { DestroyIfPossible(obj); }
        }

        public static void CleanDontDestroyOnLoad()
        {
            GameObject[] objects = Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID);

            foreach (GameObject obj in objects)
            {
                if (!obj.scene.name.Equals(DontDestroyOnLoadScene)) { continue; }

                DestroyIfPossible(obj);
            }
        }

        private static void DestroyIfPossible(GameObject obj)
        {
            if (obj.transform.root != obj.transform) { return; }

            if (obj.GetComponentInChildren<INonDestroyable>() != default) { return; }

            obj.AutoDestroy();
        }

        public interface INonDestroyable
        {
        }
    }
}
