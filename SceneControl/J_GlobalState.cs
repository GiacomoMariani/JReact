using System;
using JReact.SceneControls;
using JReact.Singleton;
using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact
{
    public class J_GlobalState : J_MonoSingleton<J_GlobalState>
    {
        // --------------- EVENTS --------------- //
        public event Action OnBeforeQuit;
        public event Action OnQuit;
        public event Action<bool> OnPause;

        // --------------- FIELDS --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField] private J_ApplicationQuit _defaultQuitAction = new J_ApplicationQuit(0);

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private bool _isQuitting = false;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private bool _isQuit = false;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private bool _isPaused = false;

        // --------------- PROPERTIES --------------- //
        public static bool IsQuitting => GetInstanceSafe()._isQuitting;
        public static bool IsQuit => GetInstanceSafe()._isQuit;
        public static bool IsPaused => GetInstanceSafe()._isPaused;

        // --------------- PAUSE --------------- //
        /// <summary>
        /// Pauses or resumes the game.
        /// </summary>
        /// <param name="pauseEnabled">Whether to pause or resume the game.</param>
        public static void Pause(bool pauseEnabled) => GetInstanceSafe().PauseImpl(pauseEnabled);

        private void PauseImpl(bool pauseEnabled)
        {
            if (_isPaused == pauseEnabled) { return; }

            _isPaused = pauseEnabled;
            OnPause?.Invoke(pauseEnabled);
        }

        // --------------- QUIT --------------- //
        /// <summary>
        /// quits the application
        /// </summary>
        public static void Quit(J_ApplicationQuit quitAction = default) => GetInstanceSafe().QuitImpl(quitAction);

        private void QuitImpl(J_ApplicationQuit quitAction)
        {
            quitAction ??= _defaultQuitAction;
            OnBeforeQuit?.Invoke();
            _isQuitting = true;
            quitAction.Quit();
            OnQuit?.Invoke();
            _isQuit = true;
        }
    }
}
