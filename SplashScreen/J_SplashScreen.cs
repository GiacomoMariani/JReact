using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace JReact.SplashScreen
{
    public abstract class J_SplashScreen : MonoBehaviour
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly] private J_ProcessableAction _afterSplashScreen;
        [BoxGroup("Setup", true, true, 0), SerializeField] private bool _closeIfInputPressed = true;

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public bool IsRunning { get; protected set; }

        [Button("Start Splash Screen"), Tooltip("Start the splash screen"), ButtonGroup("Actions")]
        public void StartSplashScreen() { SplashScreenImpl(); }

        [Button("Close Splash Screen"), Tooltip("Close the splash screen"), ButtonGroup("Actions")]
        public void CloseSplashScreen() { IsRunning = false; }

        private void Update()
        {
            if (!IsRunning) { return; }

            if (_closeIfInputPressed) { CheckForCloseButtons(); }

            CheckForCloseCustomized();
        }

        protected virtual void CheckForCloseCustomized() {}

        private void CheckForCloseButtons()
        {
            if (Keyboard.current != null &&
                Keyboard.current.anyKey.wasPressedThisFrame) { CloseSplashScreen(); }
            
            if (Mouse.current != null &&
                (Mouse.current.leftButton.wasPressedThisFrame ||
                 Mouse.current.rightButton.wasPressedThisFrame ||
                 Mouse.current.middleButton.wasPressedThisFrame)) { CloseSplashScreen(); }

            if (Touchscreen.current != null &&
                Touchscreen.current.primaryTouch.press.wasPressedThisFrame) { CloseSplashScreen(); }

            if (Gamepad.current != null &&
                (Gamepad.current.buttonSouth.wasPressedThisFrame ||
                 Gamepad.current.buttonEast.wasPressedThisFrame ||
                 Gamepad.current.buttonWest.wasPressedThisFrame ||
                 Gamepad.current.buttonNorth.wasPressedThisFrame ||
                 Gamepad.current.startButton.wasPressedThisFrame)) { CloseSplashScreen(); }
        }

        private async UniTaskVoid SplashScreenImpl()
        {
            IsRunning = true;
            await OnStartSplashScreen();
            while (IsRunning) { await UniTask.WaitForEndOfFrame(); }

            await OnCloseSplashScreen();
            _afterSplashScreen?.Process();
        }

        protected abstract UniTask OnStartSplashScreen();
        protected abstract UniTask OnCloseSplashScreen();
    }
}
