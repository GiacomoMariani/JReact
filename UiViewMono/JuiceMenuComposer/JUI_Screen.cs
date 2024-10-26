using System;
using System.Collections.Generic;
using MEC;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.JuiceMenuComposer
{
    /// <summary>
    /// Represents a screen in the JUI (Juice UI) system.
    /// Do not add any logic to a class deriving from this, it's used just as a tag to retrive a given screen/ui
    /// </summary>
    public abstract class JUI_Screen : MonoBehaviour
    {
        // --------------- SETUP --------------- //
        /// <summary>
        /// Represents all UI items used in a JUI_Screen.
        /// </summary>
        [BoxGroup("Setup", true, true, 0), SerializeField, ChildGameObjectsOnly, Required]
        private JUI_Item[] _uiItems = Array.Empty<JUI_Item>();
        [BoxGroup("Setup", true, true, 0), SerializeField, ChildGameObjectsOnly, Required]
        private GameObject _mainView;
        [BoxGroup("Setup", true, true, 0), SerializeField] private Segment _operationSegment = Segment.LateUpdate;

        // --------------- STATE --------------- //
        [BoxGroup("State", false, true, 5), ReadOnly, ShowInInspector] public JScreenStatus CurrentState { get; private set; } =
            JScreenStatus.NotRegistered;
        [BoxGroup("State", false, true, 5), ReadOnly, ShowInInspector] public bool IsShowingOrShown
            => CurrentState == JScreenStatus.Shown || CurrentState == JScreenStatus.Showing;
        [BoxGroup("State", false, true, 5), ReadOnly, ShowInInspector] public bool IsHidingOrHidden
            => CurrentState == JScreenStatus.Hidden && CurrentState == JScreenStatus.Hiding;
        [BoxGroup("State", false, true, 5), ReadOnly, ShowInInspector] public bool IsShowing => _showCoroutines.Count > 0;
        [BoxGroup("State", false, true, 5), ReadOnly, ShowInInspector] public bool IsHiding => _hideCoroutines.Count  > 0;
        [BoxGroup("State", false, true, 5), ReadOnly, ShowInInspector] public bool IsOperationRunning
            => _mainOperationHandle.IsRunning;
        [BoxGroup("State", false, true, 5), ReadOnly, ShowInInspector] internal bool IsFromAddressable { get; private set; }
        [BoxGroup("State", false, true, 5), ReadOnly, ShowInInspector] public bool IsReady
            => CurrentState != JScreenStatus.NotRegistered;

        // --------------- TYPE DATA --------------- //
        [BoxGroup("State", false, true, 5), ReadOnly, ShowInInspector] internal Type TypeFast { get; private set; }
        [BoxGroup("State", false, true, 5), ReadOnly, ShowInInspector] private string NameOfThis => $"{name}_{TypeFast?.Name}";

        // --------------- COROUTINE & OTHER HANDLES --------------- //
        private readonly List<CoroutineHandle> _hideCoroutines = new List<CoroutineHandle>();
        private readonly List<CoroutineHandle> _showCoroutines = new List<CoroutineHandle>();
        private CoroutineHandle _mainOperationHandle;

        // --------------- INITIALIZATION --------------- //
        internal void InitScreen(bool isFromAddressable = false)
        {
            IsFromAddressable = isFromAddressable;
            TypeFast          = this.GetType();
            for (int i = 0; i < _uiItems.Length; i++) { _uiItems[i].OnInit(this); }

            CurrentState = _mainView.activeSelf ? JScreenStatus.Shown : JScreenStatus.Hidden;
        }

        // --------------- SHOW SECTION --------------- //
        /// <summary>
        /// Handles the showing process of a JUI_Screen.
        /// </summary>
        /// <returns>A coroutine that handles the showing process.</returns>
        internal IEnumerator<float> ShowImpl()
        {
            Log("Showing Process Start");
            if (IsHiding) { ShutdownHidingProcess(); }

            if (IsShowingOrShown)
            {
                LogWaring($"was already {CurrentState}. Cancelling {nameof(ShowImpl)}");
                yield break;
            }

            CurrentState = JScreenStatus.Showing;
            //show the main view at opening, so all animations might be seen
            _mainView.SetActive(true);
            _showCoroutines.Clear();

            foreach (JUI_Item uiItem in _uiItems)
            {
                _showCoroutines.Add(Timing.RunCoroutine(uiItem.OnBeforeShow(this).CancelWith(gameObject), _operationSegment));
            }

            Assert.IsFalse(_mainOperationHandle.IsRunning);
            _mainOperationHandle = Timing.RunCoroutine(CompleteAllOperations(_showCoroutines), _operationSegment);
            yield return Timing.WaitUntilDone(_mainOperationHandle);

            _showCoroutines.Clear();
            ConfirmShow();
        }

        internal void ForceCompleteShow()
        {
            ShutDownAllAnimations();
            ConfirmShow();
        }

        private void ConfirmShow()
        {
            foreach (JUI_Item uiItem in _uiItems) { uiItem.OnCompleteShow(this); }

            CurrentState = JScreenStatus.Shown;
            Log("Show Complete");
        }

        // --------------- HIDE SECTION --------------- //
        /// <summary>
        /// Handles the hiding process of a JUI_Screen.
        /// </summary>
        /// <returns>A coroutine that handles the hiding process.</returns>
        internal IEnumerator<float> HideImpl()
        {
            Log("Hiding Process Start");
            ShutdownShowingProcess();

            if (IsHidingOrHidden)
            {
                LogWaring($"was already {CurrentState}. Cancelling {nameof(HideImpl)}");
                yield break;
            }

            CurrentState = JScreenStatus.Hiding;
            _hideCoroutines.Clear();

            foreach (JUI_Item uiItem in _uiItems)
            {
                _hideCoroutines.Add(Timing.RunCoroutine(uiItem.OnBeforeHide(this).CancelWith(gameObject), _operationSegment));
            }

            Assert.IsFalse(_mainOperationHandle.IsRunning);
            _mainOperationHandle = Timing.RunCoroutine(CompleteAllOperations(_hideCoroutines), _operationSegment);
            yield return Timing.WaitUntilDone(_mainOperationHandle);

            _hideCoroutines.Clear();
            ConfirmHide();
        }

        internal void ForceCompleteHide()
        {
            ShutDownAllAnimations();
            ConfirmHide();
        }

        private void ConfirmHide()
        {
            foreach (JUI_Item uiItem in _uiItems) { uiItem.OnCompleteHide(this); }

            //hide the main view at the end, so all end animations are shown
            _mainView.SetActive(false);
            CurrentState = JScreenStatus.Hidden;
            Log("Hide Complete");
        }
        
        // --------------- SHOTDOWN --------------- //
        private void ShutDownAllAnimations()
        {
            ShutdownShowingProcess();
            ShutdownHidingProcess();
        }
        
        private void ShutdownShowingProcess()
        {
            if (!IsShowing) { return; }

            Timing.KillCoroutines(_mainOperationHandle);
            for (int i = 0; i < _showCoroutines.Count; i++) { Timing.KillCoroutines(_showCoroutines[i]); }

            _showCoroutines.Clear();
            for (int i = 0; i < _uiItems.Length; i++) { _uiItems[i].OnStopShowing(this); }
        }

        private void ShutdownHidingProcess()
        {
            if (!IsHiding) { return; }

            Timing.KillCoroutines(_mainOperationHandle);
            for (int i = 0; i < _showCoroutines.Count; i++) { Timing.KillCoroutines(_hideCoroutines[i]); }

            _hideCoroutines.Clear();
            for (int i = 0; i < _uiItems.Length; i++) { _uiItems[i].OnStopHiding(this); }
        }

        // --------------- OPERATIONS AND UTILITIES --------------- //
        private IEnumerator<float> CompleteAllOperations(List<CoroutineHandle> operations)
        {
            bool allOpsCompleted = false;
            while (!allOpsCompleted)
            {
                allOpsCompleted = true;
                for (int i = 0; i < operations.Count; i++)
                {
                    if (!_showCoroutines[i].IsRunning) { continue; }

                    allOpsCompleted = false;
                    break;
                }

                if (!allOpsCompleted) { yield return Timing.WaitForOneFrame; }
            }
        }

        private void Log(string       message) => JLog.Log($"{NameOfThis} - {message}", JLogTags.UiView, this);
        private void LogWaring(string message) => JLog.Warning($"{NameOfThis} - {message}", JLogTags.UiView, this);

        private void OnValidate()
        {
            if (_mainView == default) { _mainView = this.gameObject; }

            if (_uiItems        == default ||
                _uiItems.Length == 0) { _uiItems = _mainView?.GetComponentsInChildren<JUI_Item>(true); }
        }
    }
}
