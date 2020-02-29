using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact
{
    /// <summary>
    /// a service to handle some processes
    /// IMPORTANT CONSIDERATION
    /// => ACTIVATION starts the service if not active
    /// => END stop the service, but keeps the listeners - nothing happen if this was inactive
    /// => RESET make sure the service is ended
    /// </summary>
    public class J_Service : ScriptableObject, jStateJObservable, iActivable
    {
        private event Action OnActivate;
        private event Action OnExit;

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public bool IsActive { get; private set; }
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public string Name => name;

        public static J_Service CreateService(string nameToSet)
        {
            var state = CreateInstance<J_Service>();
            state.name = nameToSet;
            return state;
        }

        /// <summary>j
        /// Activates the service. It would be better if the service has just one activation/entry point
        /// </summary>
        [ButtonGroup("Commands", 200), Button("Activate", ButtonSizes.Medium)]
        public void Activate()
        {
            if (IsActive)
            {
                JLog.Warning($"{name} was already active. Cancel command.", JLogTags.Task, this);
                return;
            }

            IsActive = true;
            ActivateThis();
            OnActivate?.Invoke();
            JLog.Log($"{name} service activated.", JLogTags.Service, this);
        }

        //the specific implementation of activate
        protected virtual void ActivateThis() {}

        /// <summary>
        /// Ends the service. It would be better if the service has just one end/exit point
        /// </summary>
        [ButtonGroup("Commands", 200), Button("End", ButtonSizes.Medium)]
        public virtual void End()
        {
            if (!IsActive)
            {
                JLog.Warning($"{name} was not active. Cancel command.", JLogTags.Task, this);
                return;
            }

            EndThis();
            IsActive = false;
            OnExit?.Invoke();
            JLog.Log($"{name} service ended.", JLogTags.Service, this);
        }

        //the specific implementation of activate
        protected virtual void EndThis() {}

        public void Subscribe(Action   actionToSubscribe) { OnActivate += actionToSubscribe; }
        public void UnSubscribe(Action actionToSubscribe) { OnActivate -= actionToSubscribe; }

        public void SubscribeToEnd(Action   actionToSend) { OnExit += actionToSend; }
        public void UnSubscribeToEnd(Action actionToSend) { OnExit -= actionToSend; }

        /// <summary>
        /// You may use Reset to make sure the service has been ended before activation
        /// </summary>
        public virtual void ResetThis()
        {
            if (IsActive) End();
            JLog.Log($"{name} service reset.", JLogTags.Service, this);
        }
    }
}
