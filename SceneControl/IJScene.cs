using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace JReact.SceneControl
{
    public interface IJScene
    {
        string SceneId { get; }
        UniTask LoadSceneAsync(LoadSceneMode        mode);
        UniTask UnloadSceneAsync(UnloadSceneOptions options);

        public async UniTask LoadWithPreLoading(IJScene loadingScene, bool removePreviousScenes = false)
        {
            var loadScene = LoadSceneMode.Additive;
            if (removePreviousScenes) { loadScene = LoadSceneMode.Single; }

            await loadingScene.LoadSceneAsync(loadScene);
#if UNITY_ADDRESSABLES
            if (removePreviousScenes) { await J_SO_AddressableScene.RemoveAllSceneFromMemory(); }
#endif

            await LoadSceneAsync(LoadSceneMode.Additive);
            await loadingScene.UnloadSceneAsync(UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
        }
    }
}
