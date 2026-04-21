#if UNITY_DOTS
using Sirenix.Utilities;
using Unity.Entities;
using UnityEngine;

namespace JECS
{
    public abstract class J_ECSEventListener<T> : MonoBehaviour
        where T : unmanaged, IBufferElementData
    {
        protected virtual J_ECSEventWatcher<T> Watcher { get; set; }

        // --------------- FIELDS AND PROPERTIES --------------- //
        public             void OnEvent(T eventData, int version) => Process(eventData, version);
        protected abstract void Process(T eventData, int version);

        // --------------- LISTENER SETUP --------------- //
        protected virtual void OnEnable()
        {
            if (Watcher.SafeIsUnityNull()) { return; }

            Watcher.Subscribe(this);
        }

        protected void InjectWatcher(J_ECSEventWatcher<T> watcher)
        {
            Terminate();
            Watcher = watcher;
            Watcher.Subscribe(this);
        }

        protected virtual void OnDisable() { Terminate(); }

        private void Terminate()
        {
            if (Watcher.SafeIsUnityNull()) { return; }

            Watcher.Unsubscribe(this);
        }
    }
}
#endif
