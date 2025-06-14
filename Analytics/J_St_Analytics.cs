using JReact.Singleton;
using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.Analytics
{
    public abstract class J_St_Analytics : J_MonoSingleton<J_St_Analytics>
    {
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] protected JAnalyticsTags _tags;

        protected internal override void InitThis()
        {
            _tags = new JAnalyticsTags();
            _tags.Init();
            base.InitThis();
        }

        // --------------- PROJECT EVENT IMPLEMENTATION --------------- //
        internal abstract void SendEvent(JAnalyticsEvent eventToSend);

#if UNITY_EDITOR
        [Button] private void Test() { JAnalyticsEventExample.RunExample(); }
#endif
    }
}
