using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.StateControl
{
    [CreateAssetMenu(menuName = "Reactive/Game States/Reactive State")]
    public class J_State : J_Service
    {
        // --------------- EVENTS --------------- //
        [BoxGroup("Setup - Events", true, true, 5), SerializeField] private JUnityEvent _unityEvents_AtStart = new JUnityEvent();
        [BoxGroup("Setup - Events", true, true, 5), SerializeField] private JUnityEvent _unityEvents_AtEnd = new JUnityEvent();

        /// <summary>
        /// creates a new state from a template
        /// </summary>
        /// <param name="template">the state to be copied</param>
        public static T Copy<T>(T template)
            where T : J_State
        {
            var newState = CreateInstance<T>();
            newState.name                 = template.name + "_Copy";
            newState._unityEvents_AtStart = template._unityEvents_AtStart;
            newState._unityEvents_AtEnd   = template._unityEvents_AtEnd;
            return newState;
        }

        protected override void ActivateThis()
        {
            base.ActivateThis();
            _unityEvents_AtStart?.Invoke();
        }

        protected override void EndThis()
        {
            base.EndThis();
            _unityEvents_AtEnd?.Invoke();
        }
    }
}
