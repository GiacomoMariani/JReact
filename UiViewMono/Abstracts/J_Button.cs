using System.Collections.Generic;
using MEC;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace JReact.UiView
{
    /// <summary>
    /// used to show a button
    /// </summary>
    [RequireComponent(typeof(Button))]
    public abstract class J_Button : MonoBehaviour
    {
        [BoxGroup("Setup", true, true, 0), SerializeField, ChildGameObjectsOnly, Required]
        protected Button _button;
        [BoxGroup("Setup", true, true, 0), SerializeField, Min(0f)] private float _secondsDelay = 0f;

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private CoroutineHandle _coroutine;

        // --------------- IMPLEMENTATION --------------- //
        private void TrySendCommand()
        {
            if (_coroutine.IsRunning) { return; }

            if (_secondsDelay <= 0f) { ButtonCommand(); }
            else { _coroutine = Timing.RunCoroutine(WaitAndSend(_secondsDelay), Segment.Update); }
        }

        private IEnumerator<float> WaitAndSend(float secondsDelay)
        {
            yield return Timing.WaitForSeconds(secondsDelay);
            ButtonCommand();
        }

        protected abstract void ButtonCommand();

        // --------------- LISTENERS --------------- //
        protected virtual void OnEnable() { _button.onClick.AddListener(TrySendCommand); }

        protected virtual void OnDisable()
        {
            _button.onClick.RemoveListener(TrySendCommand);
            Timing.KillCoroutines(_coroutine);
        }

        private void OnValidate()
        {
            if (_button == default) { _button = GetComponent<Button>(); }
        }
    }
}
