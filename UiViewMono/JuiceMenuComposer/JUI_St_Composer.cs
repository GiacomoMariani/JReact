using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using JReact.Singleton;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Assertions;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace JReact.JuiceMenuComposer
{
    /// <summary>
    /// This class is responsible for composing and managing UI screens in a scene.
    /// </summary>
    public class JUI_St_Composer : J_MonoSingleton<JUI_St_Composer>
    {
        [BoxGroup("Setup", true, true, 0), SerializeField] private string _addressPath = "UiScreen";
        [BoxGroup("Setup", true, true, 0), SerializeField, ChildGameObjectsOnly, Required]
        private Canvas _mainCanvas;
        public Canvas MainCanvas => _mainCanvas;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public Transform MainCanvasTransform => MainCanvas.transform;

        // --------------- STATE --------------- //
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private List<JUI_Screen> _shownScreens = new List<JUI_Screen>();

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector]
        private Dictionary<Type, JUI_Screen> _registeredScreens = new Dictionary<Type, JUI_Screen>();

        // --------------- QUERIES --------------- //
        /// <summary>
        /// Retrieves the current status of a registered screen.
        /// </summary>
        /// <typeparam name="T">The type of the screen.</typeparam>
        /// <returns>The current status of the screen.</returns>
        public static JScreenStatus GetStatus<T>() where T : JUI_Screen
        {
            Type screenTypeMember = typeof(T);
            return GetInstanceSafe()._registeredScreens.TryGetValue(screenTypeMember, out JUI_Screen result)
                       ? result.CurrentState
                       : JScreenStatus.NotRegistered;
        }

        /// <summary>
        /// Checks if a screen of type T is registered in the JUI_St_Composer.
        /// </summary>
        /// <typeparam name="T">The type of the screen to check.</typeparam>
        /// <returns>true if the screen is registered, false otherwise.</returns>
        public static bool TryGetRegisteredScreen<T>(out JUI_Screen screen) where T : JUI_Screen
            => GetInstanceSafe()._registeredScreens.TryGetValue(typeof(T), out screen);

        // --------------- INITIALIZATION --------------- //
        protected internal override void InitThis()
        {
            JUI_Screen[] inSceneScreens = MainCanvasTransform.GetComponentsInChildren<JUI_Screen>(true);

            for (int i = 0; i < inSceneScreens.Length; i++)
            {
                JUI_Screen screen = inSceneScreens[i];
                screen.InitScreen(false);
                _registeredScreens.Add(screen.TypeFast, screen);
            }

            base.InitThis();
        }

        // --------------- SHOW PROCESS --------------- //
        /// <summary>
        /// Shows a UI screen of type T asynchronously.
        /// </summary>
        /// <typeparam name="T">The type of the screen to show.</typeparam>
        /// <param name="showWithoutAnimations">Determines whether to show the screen without animations. Default is false.</param>
        /// <returns>A UniTask representing the asynchronous operation.</returns>
        public static async UniTask<T> Show<T>(bool showWithoutAnimations = false)
            where T : JUI_Screen => await GetInstanceSafe().ProcessShow<T>(showWithoutAnimations);

        /// <summary>
        /// Processes the show operation for a specified UI screen.
        /// </summary>
        /// <typeparam name="T">The type of the UI screen to show.</typeparam>
        /// <param name="showWithoutAnimations">A flag indicating whether to show the screen without animations.</param>
        /// <returns>A task representing the completion of the show operation, returning the shown UI screen.</returns>
        public async UniTask<T> ProcessShow<T>(bool showWithoutAnimations = false)
            where T : JUI_Screen
        {
            Type screenTypeMember = typeof(T);
            if (!_registeredScreens.TryGetValue(screenTypeMember, out JUI_Screen screenInstance))
            {
                screenInstance = await LoadScreenFromAddressable<T>(screenTypeMember);
                Assert.IsNotNull(screenInstance, $"{gameObject.name} requires a {nameof(screenInstance)}");
                screenInstance.InitScreen(true);
                _registeredScreens.Add(screenInstance.TypeFast, screenInstance);
            }

            return await ShowScreenImpl<T>(showWithoutAnimations, screenInstance);
        }

        private async UniTask<T> ShowScreenImpl<T>(bool showWithoutAnimations, JUI_Screen screenInstance) where T : JUI_Screen
        {
            if (showWithoutAnimations) { screenInstance.ForceCompleteShow(); }
            else { await screenInstance.ShowImpl(); }

            _shownScreens.Add(screenInstance);

            return (T)screenInstance;
        }

        // --------------- HIDE PROCESS --------------- //
        /// <summary>
        /// Hides a specified screen asynchronously.
        /// </summary>
        /// <typeparam name="T">The type of the screen to hide.</typeparam>
        /// <param name="hideWithoutAnimations">Determines if the screen should be hidden without any animations. The default value is false.</param>
        /// <param name="releaseAddressable">Determines if the addressable asset associated with the screen should be released. The default value is false.</param>
        /// <returns>The task representing the hiding process.</returns>
        public async UniTask Hide<T>(bool hideWithoutAnimations = false, bool releaseAddressable = false)
            where T : JUI_Screen => await GetInstanceSafe().ProcessHide<T>(hideWithoutAnimations, releaseAddressable);

        /// <summary>
        /// Hides a registered screen of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of the screen to hide.</typeparam>
        /// <param name="hideWithoutAnimations">Determines whether to hide the screen without playing any hide animations.</param>
        /// <param name="releaseAddressable">Determines whether to release the addressable resource associated with the screen.</param>
        /// <returns>A <see cref="UniTask{T}"/> representing the asynchronous operation of hiding the screen.</returns>
        public async UniTask ProcessHide<T>(bool hideWithoutAnimations = false, bool releaseAddressable = false)
            where T : JUI_Screen
        {
            Type screenTypeMember = typeof(T);

            if (!_registeredScreens.TryGetValue(screenTypeMember, out JUI_Screen screenInstance))
            {
                LogWaring($"No screen Registered of type {screenTypeMember.Name}");
                return;
            }

            await HideScreenImpl(screenInstance, hideWithoutAnimations, releaseAddressable);
        }

        private async UniTask HideScreenImpl(JUI_Screen screenInstance, bool hideWithoutAnimations, bool releaseAddressable)
        {
            if (hideWithoutAnimations) { screenInstance.ForceCompleteHide(); }
            else { await screenInstance.HideImpl(); }

            if (_shownScreens.Contains(screenInstance)) { _shownScreens.Remove(screenInstance); }
            else { LogWaring($"{screenInstance.TypeFast} was not in the open screens. TotalOpen Screens: {_shownScreens.Count}"); }

            if (releaseAddressable) { ReleaseScreenImpl(screenInstance); }
            else { screenInstance.gameObject.SetActive(false); }
        }

        /// <summary>
        /// The Back method is responsible for navigating back in the stack of shown screens.
        /// If there are no screens shown, a warning message is logged and the method returns.
        /// Otherwise, the top-most screen is removed from the stack and hidden.
        /// </summary>
        /// <param name="forceCloseLastMenu">Determines whether the top-most screen should be forcefully closed.</param>
        /// <param name="release">Determines whether the screen should be released.</param>
        public async UniTaskVoid Back(bool forceCloseLastMenu = false, bool release = false)
        {
            if (_shownScreens.Count <= 0)
            {
                LogWaring($"No screen shown: {_shownScreens.Count}");
                return;
            }

            JUI_Screen screenInstance = _shownScreens[^1];
            Assert.IsTrue(_registeredScreens.ContainsKey(screenInstance.TypeFast),
                          $"{screenInstance.TypeFast} not found in registered screens ({_registeredScreens.Count})");

            await HideScreenImpl(screenInstance, forceCloseLastMenu, release);
        }

        // --------------- ADDRESSABLE OPERATIONS --------------- //
        /// <summary>
        /// Releases the resources associated with a registered screen of type T.
        /// </summary>
        /// <typeparam name="T">The type of the screen.</typeparam>
        public static void Release<T>() where T : JUI_Screen => GetInstanceSafe().ProcessRelease<T>();

        /// <summary>
        /// Releases a screen instance of type T.
        /// </summary>
        /// <typeparam name="T">The type of the screen.</typeparam>
        public void ProcessRelease<T>()
            where T : JUI_Screen
        {
            Type screenTypeMember = typeof(T);
            if (!_registeredScreens.TryGetValue(screenTypeMember, out JUI_Screen screenInstance))
            {
                LogWaring($"No screen Registered of type {screenTypeMember.Name}");
                return;
            }

            ReleaseScreenImpl(screenInstance);
        }

        private void ReleaseScreenImpl(JUI_Screen instance)
        {
            _registeredScreens.Remove(instance.TypeFast);
            if (instance.IsFromAddressable) { Addressables.ReleaseInstance(instance.gameObject); }
        }

        private async UniTask<T> LoadScreenFromAddressable<T>(Type menuType)
            where T : JUI_Screen
        {
            AsyncOperationHandle<GameObject> handle =
                Addressables.InstantiateAsync($"{_addressPath}/{menuType.Name}", MainCanvasTransform);

            GameObject screenGameObject = await handle.Task;
            var        screenInstance   = screenGameObject.GetComponent<T>();

            return screenInstance;
        }

        // --------------- RESET --------------- //
        /// <summary>
        /// Resets the composer by hiding all registered screens and optionally releasing them.
        /// </summary>
        /// <param name="release">True if the screens should be released, false otherwise.</param>
        /// <returns>A UniTask representing the asynchronous operation.</returns>
        public async UniTask ResetComposer(bool release = false)
        {
            using Dictionary<Type, JUI_Screen>.ValueCollection.Enumerator screenEnumerator = _registeredScreens.Values.GetEnumerator();

            while (screenEnumerator.MoveNext())
            {
                JUI_Screen current = screenEnumerator.Current;
                if (current == default) { return; }

                await HideScreenImpl(current, true, release);
            }
        }

        // --------------- LOGGERS & UTILITIES --------------- //
        private void Log(string       message) => JLog.Log($"{gameObject.name} Composer - {message}", JLogTags.UiView, this);
        private void LogWaring(string message) => JLog.Warning($"{gameObject.name} Composer - {message}", JLogTags.UiView, this);

        private void OnValidate()
        {
            if (_mainCanvas == default) { _mainCanvas = GetComponentInChildren<Canvas>(true); }
        }
    }
}
