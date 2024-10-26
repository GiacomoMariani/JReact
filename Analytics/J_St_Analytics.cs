using JReact.Singleton;
using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.Analytics
{
    public abstract class J_St_Analytics : J_MonoSingleton<J_St_Analytics>
    {
        [BoxGroup("Setup", true, true, 0), SerializeField] protected JAnalyticsTags _tags = new();

        // --------------- PROJECT EVENT IMPLEMENTATION --------------- //
        internal abstract void SendEvent(JAnalyticsEvent            eventToSend);

#if UNITY_EDITOR
        [Button]
        private void Test() { JAnalyticsEventExample.RunExample(); }
#endif
    }
}
