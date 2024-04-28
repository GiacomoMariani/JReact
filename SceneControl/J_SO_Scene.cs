using System;
using System.IO;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;
using UniTask = Cysharp.Threading.Tasks.UniTask;

namespace JReact.SceneControl
{
    /// <summary>
    /// Represents a scene that can be loaded and unloaded.
    /// </summary>
    [CreateAssetMenu(menuName = "Reactive/Scenes/Scene")]
    public class J_SO_Scene : J_ProcessableAction
    {
        public static event Action<J_SO_Scene> OnSceneLoadStart;
        public static event Action<J_SO_Scene> OnSceneLoadComplete;

        // --------------- FIELDS AND PROPERTIES --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField] private int _sceneIndex;
        public int SceneIndex => _sceneIndex;

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public string ScenePath { get; private set; }
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public string SceneName { get; private set; }
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public float LoadState { get; private set; }
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public bool IsReady
            => ScenePath == SceneUtility.GetScenePathByBuildIndex(_sceneIndex);
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public bool IsActive => IsSceneLoaded();

        private void Awake() { Init(); }

        private void Init()
        {
            ScenePath = SceneUtility.GetScenePathByBuildIndex(SceneIndex);
            SceneName = Path.GetFileNameWithoutExtension(ScenePath);
            LoadState = 0f;
        }

        // --------------- COMMANDS - DIRECT --------------- //
        [BoxGroup("Debug", true, true, 100), Button(ButtonSizes.Medium)]
        public override void Process() { LoadScene(LoadSceneMode.Single); }

        public J_SO_Scene LoadScene(LoadSceneMode mode = LoadSceneMode.Single)
        {
            StartLoading();
            SceneManager.LoadScene(SceneIndex, LoadSceneMode.Single);
            return this;
        }
        
        public async UniTask LoadSceneAsync(LoadSceneMode mode)
        {
            StartLoading();
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(SceneIndex, mode);

            while (!asyncLoad.isDone)
            {
                //wait one frame and send the progress event
                await UniTask.WaitForEndOfFrame();
                LoadState = asyncLoad.progress;
            }

            LoadState = 1f;
        }

        // --------------- COMMANDS - ASYNC --------------- //
        public async UniTask UnloadSceneAsync()
        {
            AsyncOperation asyncLoad = SceneManager.UnloadSceneAsync(SceneIndex, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);

            while (!asyncLoad.isDone)
            {
                //wait one frame and send the progress event
                await UniTask.WaitForEndOfFrame();
                LoadState = 1f - asyncLoad.progress;
            }

            LoadState = 0f;
        }

        // --------------- SCENE CHANGE EVENT --------------- //
        private void StartLoading()
        {
            JLog.Log($"{name} load scene start: {SceneName}\nFrom Scene {SceneManager.GetActiveScene().buildIndex}",
                     JLogTags.SceneManager, this);

            SceneManager.activeSceneChanged -= EndLoading;
            SceneManager.activeSceneChanged += EndLoading;
            OnSceneLoadStart?.Invoke(this);
        }

        private void EndLoading(Scene oldScene, Scene newScene)
        {
            SceneManager.activeSceneChanged -= EndLoading;
            JLog.Log($"{name} scene loaded.\nFrom: {oldScene.name}- to -{newScene.name}-", JLogTags.SceneManager, this);
            OnSceneLoadComplete?.Invoke(this);
        }

        // --------------- QUERIES --------------- //
        private bool IsSceneLoaded()
        {
            var scenesCount = SceneManager.sceneCount;
            for (int i = 0; i < scenesCount; i++)
            {
                if (SceneManager.GetSceneAt(i).buildIndex == SceneIndex) { return true; }
            }

            return false;
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
