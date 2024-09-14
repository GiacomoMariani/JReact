using System;
using System.IO;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using UniTask = Cysharp.Threading.Tasks.UniTask;

namespace JReact.SceneControl
{
    /// <summary>
    /// Represents a scene that can be loaded and unloaded.
    /// </summary>
    [CreateAssetMenu(menuName = "Reactive/Scenes/Scene", fileName = "Scene")]
    public class J_SO_Scene : J_ProcessableAction, IJScene
    {
        public static event Action<J_SO_Scene> OnSceneLoadStart;
        public static event Action<J_SO_Scene> OnSceneLoadComplete;

        // --------------- FIELDS AND PROPERTIES --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField] private int _sceneIndex;
        public int SceneIndex => _sceneIndex;
        public string SceneId => ScenePath;

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public string ScenePath { get; private set; }
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public string SceneName { get; private set; }
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public float LoadState { get; private set; }
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public bool IsLoading { get; private set; }
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public bool IsReady
            => ScenePath == SceneUtility.GetScenePathByBuildIndex(_sceneIndex);
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public bool IsMainScene => IsMainScene_Impl();
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public bool IsActive => IsSceneLoaded();
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public bool IsOnlySceneActive => IsOnlySceneActive_Impl();
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private static IJScene[] _CacheLoading = new IJScene[1];

        private void Awake() { Init(); }

        public void Init()
        {
            ScenePath          = SceneUtility.GetScenePathByBuildIndex(SceneIndex);
            SceneName          = Path.GetFileNameWithoutExtension(ScenePath);
            _actionDescription = $"Load Scene {ScenePath}";
            LoadState          = 0f;
            IsLoading = false;
        }

        // --------------- COMMANDS - LOADING --------------- //
        [BoxGroup("Debug", true, true, 100), Button(ButtonSizes.Medium)]
        public override void Process() { LoadScene(LoadSceneMode.Single); }

        public J_SO_Scene LoadScene(LoadSceneMode mode = LoadSceneMode.Single)
        {
            StartLoading();
            SceneManager.LoadScene(SceneIndex, LoadSceneMode.Single);
            return this;
        }

        public async UniTask LoadWithLoading(J_SO_Scene loadingScene)
        {
            _CacheLoading[0] = this;
            await JSceneUtils.LoadTogether(_CacheLoading, loadingScene);
        }

        public async UniTask LoadSceneAsync(LoadSceneMode mode, bool setMainScene = false)
        {
            StartLoading();
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(SceneIndex, mode);

            while (!asyncLoad.isDone)
            {
                //wait one frame and send the progress event
                await UniTask.WaitForEndOfFrame();
                LoadState = asyncLoad.progress;
            }

            if (setMainScene) { SceneManager.SetActiveScene(SceneManager.GetSceneByName(SceneName)); }

            await WaitSceneLoaded();

            LoadState = 1f;
        }

        // --------------- COMMANDS - UNLOADING --------------- //
        public async UniTask UnloadSceneAsync(UnloadSceneOptions options = UnloadSceneOptions.UnloadAllEmbeddedSceneObjects)
        {
            JLog.Log($"{name} Unload scene start: {SceneName}", JLogTags.SceneManager, this);
            Assert.IsFalse(IsOnlySceneActive, $"{SceneName} is the only one active. It cannot be unloaded.");

            AsyncOperation asyncLoad = SceneManager.UnloadSceneAsync(SceneIndex, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);

            while (!asyncLoad.isDone)
            {
                //wait one frame and send the progress event
                await UniTask.WaitForEndOfFrame();
                LoadState = 1f - asyncLoad.progress;
            }

            LoadState = 0f;
            JLog.Log($"{name} Unload scene complete: {SceneName}", JLogTags.SceneManager, this);
        }

        // --------------- SCENE CHANGE EVENT --------------- //
        private void StartLoading()
        {
            JLog.Log($"{name} load scene start: {SceneName} from Scene {SceneManager.GetActiveScene().buildIndex}",
                     JLogTags.SceneManager, this);

            IsLoading                       =  true;
            SceneManager.activeSceneChanged -= EndLoading;
            SceneManager.activeSceneChanged += EndLoading;
            OnSceneLoadStart?.Invoke(this);
        }

        private void EndLoading(Scene oldScene, Scene newScene)
        {
            if (newScene.name != SceneName) { return; }

            IsLoading                       =  false;
            SceneManager.activeSceneChanged -= EndLoading;
            JLog.Log($"{name} scene loaded.\nFrom id {oldScene.buildIndex}- to -{newScene.name}-", JLogTags.SceneManager, this);
            OnSceneLoadComplete?.Invoke(this);
        }

        // --------------- QUERIES --------------- //
        public bool IsSceneLoaded()
        {
            var scenesCount = SceneManager.sceneCount;
            for (int i = 0; i < scenesCount; i++)
            {
                if (SceneManager.GetSceneAt(i).buildIndex == SceneIndex) { return true; }
            }

            return false;
        }

        private bool IsMainScene_Impl() => SceneManager.GetActiveScene().buildIndex == SceneIndex;

        private bool IsOnlySceneActive_Impl()
            => SceneManager.sceneCount == 1 && SceneManager.GetActiveScene().buildIndex == SceneIndex;

        public async UniTask WaitSceneLoaded()
        {
            if (IsSceneLoaded()) { return; }

            await J_Async_Utils.WaitUntilReady(IsSceneLoaded, ActionDescription);
        }

        /// <summary>
        /// Converts the J_SO_Scene object to a Unity Scene object. </summary> <returns>
        /// The Unity Scene object corresponding to the J_SO_Scene. </returns>
        /// /
        public Scene ToUnityScene() => SceneManager.GetSceneByBuildIndex(_sceneIndex);

        // --------------- HELPERS --------------- //
        private void OnValidate()
        {
            if (!IsReady) { Init(); }
        }

        public override string ToString() => $"{SceneIndex}: {ScenePath} ({ScenePath})";
    }
}
