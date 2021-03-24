using System.Collections.Generic;
using MEC;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace JReact.UiView
{
    /// <summary>
    /// used to show a button
    /// </summary>
    [RequireComponent(typeof(Button))]
    public abstract class J_UiView_ButtonItem : MonoBehaviour
    {
        private const string CoroutineTag = "JUV_ButtonElement";

        [BoxGroup("Setup", true, true, 0), SerializeField, ChildGameObjectsOnly, Required]
        protected Button _button;
        [BoxGroup("Setup", true, true, 0), SerializeField, Min(0f)] private float _secondsDelay = 0f;

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private int _instanceId;

        // --------------- INITIALIZATION --------------- //
        private void Awake()
        {
            InitThis();
            SanityChecks();
            _instanceId = GetInstanceID();
        }

        protected virtual void InitThis()
        {
            if (_button == null) _button = GetComponent<Button>();
        }

        protected virtual void SanityChecks() => Assert.IsNotNull(_button, $"{gameObject.name} requires a {nameof(_button)}");

        // --------------- IMPLEMENTATION --------------- //
        //the main command sent by this button
        private void TrySendCommand()
        {
            if (_secondsDelay <= 0f) ButtonCommand();
            else Timing.RunCoroutine(WaitAndSend(_secondsDelay), Segment.Update, _instanceId, CoroutineTag);
        }

        private IEnumerator<float> WaitAndSend(float secondsDelay)
        {
            yield return Timing.WaitForSeconds(secondsDelay);
            ButtonCommand();
        }

        protected abstract void ButtonCommand();

        // --------------- LISTENERS --------------- //
        //start and stop tracking on enable
        protected virtual void OnEnable()
        {
            _button.onClick.RemoveListener(TrySendCommand);
            _button.onClick.AddListener(TrySendCommand);
        }

        protected virtual void OnDisable()
        {
            _button.onClick.RemoveListener(TrySendCommand);
            Timing.KillCoroutines(_instanceId, CoroutineTag);
        }
    }
}
