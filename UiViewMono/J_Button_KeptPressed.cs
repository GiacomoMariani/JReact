using System.Collections.Generic;
using MEC;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace JReact.UiView
{
    public sealed class J_Button_KeptPressed : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        private const string KeptPress_Tag = "KeptPress_Tag";

        [BoxGroup("Setup", true, true), SerializeField] private UnityEvent _action;
        [BoxGroup("Setup", true, true), SerializeField, MinValue(1f)] private ushort[] _actionPerSeconds;
        [BoxGroup("Setup", true, true), SerializeField] private float _secondsBetweenIterations = 2f;

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private float _secondsPressed;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private int _instanceId;

        private void Awake() => _instanceId = GetInstanceID();

        // --------------- ACTION LOOP --------------- //
        private IEnumerator<float> PressingButton(int iteration)
        {
            // --------------- SANITY CHECK --------------- //
            if (iteration > _actionPerSeconds.Length - 1)
            {
                JLog.Warning($"{gameObject.name} Iteration {iteration} is too high. Setting Max: {_actionPerSeconds.Length}",
                             JLogTags.UiView, this);

                iteration = _actionPerSeconds.Length - 1;
            }

            // --------------- PER CALCULATION --------------- //
            //calculate the amount of iteration per second
            float iterationPerSecond = 1f / _actionPerSeconds[iteration];
            float startTime          = Time.unscaledTime;

            // --------------- ITERATION LOOP --------------- //
            //do these iterations for an amount of time
            while (Time.unscaledTime < startTime + _secondsBetweenIterations)
            {
                _action?.Invoke();
                yield return Timing.WaitForSeconds(iterationPerSecond);
            }

            // --------------- NEXT ITERATION --------------- //
            int nextIteration = Mathf.Min(iteration + 1, _actionPerSeconds.Length - 1);
            Timing.RunCoroutine(PressingButton(nextIteration), Segment.Update, _instanceId, KeptPress_Tag);
        }

        // --------------- CLICK EVENTS --------------- //
        public void OnPointerDown(PointerEventData eventData) => Timing.RunCoroutine(PressingButton(0), Segment.Update, _instanceId, KeptPress_Tag);
        public void OnPointerUp(PointerEventData   eventData) => ResetThis();

        // --------------- DISABLE AND RESET --------------- //
        private void OnDisable() => ResetThis();

        private void ResetThis()
        {
            Timing.KillCoroutines(_instanceId, KeptPress_Tag);
            _secondsPressed = 0;
        }
    }
}
