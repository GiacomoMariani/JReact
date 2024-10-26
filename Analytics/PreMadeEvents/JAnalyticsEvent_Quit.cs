using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.Analytics.PreMadeEvents
{
    public class JAnalyticsEvent_Quit : MonoBehaviour
    {
        private const string _QuitEvent = "Quit";
        
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector]
        private JAnalyticsEvent _quitEvent = JAnalyticsEvent.GetOrGenerate(_QuitEvent);

        private void OnApplicationQuit() { ConfirmQuit(); }

        protected virtual void ConfirmQuit() { _quitEvent.Prepare().Send(); }
    }
}
