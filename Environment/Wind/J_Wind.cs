using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.Environment.Wind
{
    public enum WindKind { None, Calm, Strong }

    /// <summary>
    /// Scene-wide wind. Broadcasts one world-space wind vector to every wind-reading shader
    /// via the global shader vector "_GlobalWind" (xy = direction * strength, where
    /// the magnitude is the strength). One per scene; all wind materials read it.
    /// </summary>
    public sealed class J_Wind : MonoBehaviour, jObservable<J_Wind>
    {
        /// <summary>The global shader property every wind-reading material samples.</summary>
        public const string GlobalWindProperty = "_GlobalWind";
        private static readonly int _GlobalWindId = Shader.PropertyToID(GlobalWindProperty);

        // --------------- FIELDS AND PROPERTIES --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField] private Vector2 _startWind = new Vector2(0.576f, 0f);
        [BoxGroup("Setup", true, true, 0), SerializeField] private float _strongWindThreshold = 1f;

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public Vector2 WindFlow { get; private set; }
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public WindKind WindKind { get; private set; }
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public float WindStrength { get; private set; }

        /// <summary>Listeners notified every time the wind changes, after the new values are applied.</summary>
        private readonly List<Action<J_Wind>> _listeners = new List<Action<J_Wind>>();

        // --------------- COMMANDS --------------- //
        public void SetWind(Vector2 wind)
        {
            WindFlow     = wind;
            WindStrength = wind.magnitude;
            WindKind     = WindStrength > _strongWindThreshold ? WindKind.Strong : WindKind.Calm;
            Broadcast();
            TriggerEvent();
            JLog.Log($"Wind set to {WindFlow}", JLogTags.Environment, this);
        }

        private void Broadcast() => Shader.SetGlobalVector(_GlobalWindId, new Vector4(WindFlow.x, WindFlow.y, 0f, 0f));

        private void TriggerEvent()
        {
            for (int i = _listeners.Count - 1; i >= 0; i--) { _listeners[i].Invoke(this); }
        }

        private void OnEnable() => SetWind(_startWind);

        public void Subscribe(Action<J_Wind>   actionToSend) => _listeners.Add(actionToSend);
        public void UnSubscribe(Action<J_Wind> actionToSend) => _listeners.Remove(actionToSend);

        private void OnValidate() => SetWind(_startWind);
    }
}
