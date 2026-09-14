using System.Collections.Generic;
using MEC;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace JReact.UiView
{
    //do an action when the button is kept pressed
    public sealed class J_ButtonPressAccelerator : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        private const string ThisTag = "ButtonPressTag";

        [BoxGroup("Setup", true, true), SerializeField] private float _accelerationInterval = 2f;
        [BoxGroup("Setup", true, true), SerializeField, MinValue(1f)] private ushort[] _actionPerSeconds;
        [BoxGroup("Setup", true, true), SerializeField] private UnityEvent _action;

        //the amount of time this button has been pressed
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private int _instanceId;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private int MaxIterations => _actionPerSeconds?.Length - 1 ?? 0;

        // --------------- INITIALIZATION --------------- //
        private void Awake() => _instanceId = this.JUniqueId();

        // --------------- ACTION LOOP --------------- //
        private IEnumerator<float> StartPressingButton()
        {
            int iteration = 0;
            while (true)
            {
                // --------------- PRE LOOP CALCULATION --------------- //
                float secondsOfInterval = GetCurrentIterationIntervalInSeconds(iteration);
                float timeAtStart       = Time.unscaledTime;

                // --------------- ITERATION LOOP --------------- //
                while (Time.unscaledTime < timeAtStart + _accelerationInterval)
                {
                    _action?.Invoke();
                    yield return Timing.WaitForSeconds(secondsOfInterval);
                }

                // --------------- NEXT ITERATION --------------- //
                iteration = Mathf.Min(iteration + 1, MaxIterations);
            }
        }

        private float GetCurrentIterationIntervalInSeconds(int iteration) => 1f / _actionPerSeconds[iteration];

        // --------------- POINTER EVENTS --------------- //
        public void OnPointerDown(PointerEventData eventData)
            => Timing.RunCoroutine(StartPressingButton(), Segment.Update, _instanceId, ThisTag);

        public void OnPointerUp(PointerEventData eventData) => ResetThis();

        // --------------- RESET --------------- //
        private void OnDisable() => ResetThis();
        private void ResetThis() => Timing.KillCoroutines(_instanceId, ThisTag);
    }
}
