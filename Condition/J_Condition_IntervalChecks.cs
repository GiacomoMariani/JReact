using System.Collections.Generic;
using MEC;
using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.Conditions
{
    public abstract class J_Condition_IntervalChecks : J_ReactiveCondition
    {
        private string COROUTINE_Tag => name;

        [BoxGroup("Setup", true, true), SerializeField, MaxValue(10f), MinValue(0.1f)]
        private float _secondsForCheck = 3;
        [BoxGroup("Setup", true, true), SerializeField] private Segment _segmentInterval = Segment.SlowUpdate;

        [BoxGroup("State", true, true, 5), ShowInInspector, ReadOnly] private int _instanceID;

        protected abstract void ConditionCheck();

        protected override void StartCheckingCondition()
        {
            _instanceID = this.JUniqueId();
            Timing.RunCoroutine(KeepChecking(_secondsForCheck), _segmentInterval, _instanceID, COROUTINE_Tag);
        }

        protected override void StopCheckingCondition() => Timing.KillCoroutines(_instanceID, COROUTINE_Tag);

        private IEnumerator<float> KeepChecking(float secondsForCheck)
        {
            while (true)
            {
                ConditionCheck();
                yield return Timing.WaitForSeconds(secondsForCheck);
            }
        }
    }
}
