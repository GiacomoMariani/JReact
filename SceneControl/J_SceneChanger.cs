using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using MEC;
using Sirenix.OdinInspector;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace JReact.SceneControls
{
    /// <summary>
    /// this class is used to change the scene
    /// </summary>
    [CreateAssetMenu(menuName = "Reactive/Scenes/Scene Changer", fileName = "SceneChanger")]
    public sealed class J_SceneChanger : ScriptableObject, jObservable<(Scene previous, Scene current)>
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        private event Action<(Scene previous, Scene current)> OnSceneChange;
        private event Action<float> OnLoadProgress;

        //to make sure we save one first scene
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private bool _isInitialized;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public float CurrentProgress { get; private set; } = 0f;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector]
        private List<AsyncOperation> _operations = new List<AsyncOperation>(2);
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public bool IsLoading => _operations.Count != 0;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public int AmountOfSceneLoading
            => math.max(0, _operations.Count - 1);

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private Scene _currentScene;
        public Scene CurrentScene { get => _currentScene; private set => _currentScene = value; }

        // --------------- INITIALIZATION --------------- //
        private void SetupThis()
        {
            _isInitialized = true;
            _currentScene  = SceneManager.GetActiveScene();
            _operations.Clear();
            CurrentProgress = 0f;
            JLog.Log($"{name} scene manager initialized", JLogTags.SceneManager, this);
        }

        // --------------- COMMANDS --------------- //
        /// <summary>
        /// loads a specific scene from its name
        /// </summary>
        /// <param name="sceneName">the name of the scene to load</param>
        public void LoadScene(string sceneName)
        {
            if (!CanChangeScene()) { return; }

            if (!_isInitialized) { SetupThis(); }

            JLog.Log($"{name} load scene with name {sceneName}", JLogTags.SceneManager, this);
            _operations.Add(SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive));
            Timing.RunCoroutine(LoadingScenes(), Segment.Update, JCoroutineTags.COROUTINE_SceneChangerTag);
        }

        /// <summary>
        /// loads multipleScenes from their names
        /// </summary>
        /// <param name="sceneNames">the name of the scenes to load</param>
        public void LoadMultipleScenes(string[] sceneNames)
        {
            if (!CanChangeScene()) { return; }

            if (!_isInitialized) { SetupThis(); }

            JLog.Log($"{name} load scene with name {sceneNames.PrintAll()}", JLogTags.SceneManager, this);
            for (int i = 0; i < sceneNames.Length; i++)
            {
                _operations.Add(SceneManager.LoadSceneAsync(sceneNames[i], LoadSceneMode.Additive));
            }

            Timing.RunCoroutine(LoadingScenes(), Segment.Update, JCoroutineTags.COROUTINE_SceneChangerTag);
        }

        // --------------- QUERIES --------------- //
        private bool CanChangeScene()
        {
            if (IsLoading)
            {
                JLog.Warning($"{name} {CurrentProgress}% loading {_operations.Count} scenes from {CurrentScene.name}");
                return false;
            }

            return true;
        }

        // --------------- SCENE PROCESSING --------------- //
        private IEnumerator<float> LoadingScenes()
        {
            _operations.Add(SceneManager.UnloadSceneAsync(_currentScene));

            int totalOperations = _operations.Count;
            for (int i = 0; i < totalOperations; i++)
            {
                if (_operations[i].isDone) continue;
                CurrentProgress = 0f;
                for (int j = 0; j < totalOperations; j++)
                {
                    CurrentProgress += _operations[j].progress;
                    CurrentProgress /= totalOperations;
                    yield return Timing.WaitForOneFrame;
                    OnLoadProgress?.Invoke(CurrentProgress);
                }
            }

            _operations.Clear();
            CurrentProgress = 1f;
            Scene newScene = SceneManager.GetActiveScene();
            SceneChanged(CurrentScene, newScene);
        }

        //this is sent when the new scene is changed
        private void SceneChanged(Scene oldScene, Scene newScene)
        {
            JLog.Log($"{name} scene change from -{oldScene.name}- to -{newScene.name}-", JLogTags.SceneManager, this);
            CurrentScene = newScene;
            OnSceneChange?.Invoke((oldScene, newScene));
        }

        // --------------- SUBSCRIBERS --------------- //
        public void Subscribe(Action<(Scene previous, Scene current)>   actionToAdd)    { OnSceneChange += actionToAdd; }
        public void UnSubscribe(Action<(Scene previous, Scene current)> actionToRemove) { OnSceneChange -= actionToRemove; }

        public void SubscribeToLoadProgress(Action<float>   actionToAdd)    { OnLoadProgress += actionToAdd; }
        public void UnSubscribeToLoadProgress(Action<float> actionToRemove) { OnLoadProgress -= actionToRemove; }

#if UNITY_EDITOR
        // --------------- DEBUG --------------- //
        [BoxGroup("Debug", true, true), SerializeField] private string _sceneToLoad;
        [BoxGroup("Debug", true, true), Button(ButtonSizes.Medium)] private void DebugLoadScene() => LoadScene(_sceneToLoad);
#endif
    }
}
