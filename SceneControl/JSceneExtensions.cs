using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

namespace JReact.SceneControl
{
    public static class JSceneExtensions
    {
        private static List<UniTask> _loadOperations = new List<UniTask>(2);
        private static List<UniTask> _UnloadOperations = new List<UniTask>(2);
        public static bool IsLoading { get; private set; }
        public static bool IsUnloading { get; private set; }

        public static async UniTask LoadTogether(J_SO_Scene[] scenes, bool removePreviousScenes = true)
        {
            Assert.IsFalse(IsLoading, $"Loading operation already running");
            IsLoading = false;
            _loadOperations.Clear();
            if (removePreviousScenes) { _loadOperations.Add(UnloadAllScenes()); }

            for (int i = 0; i < scenes.Length; i++) { _loadOperations.Add(scenes[i].LoadSceneAsync(LoadSceneMode.Additive)); }

            await UniTask.WhenAll(_UnloadOperations);
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
                _UnloadOperations.Add(SceneManager.UnloadSceneAsync(sceneId, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects)
                                                  .ToUniTask());
            }

            await UniTask.WhenAll(_UnloadOperations);
            IsUnloading = false;
        }
    }
}
