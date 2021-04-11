using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.StateControl
{
    /// <summary>
    /// used to track the flow of events to move back to a previous state
    /// </summary>
    [CreateAssetMenu(menuName = "Reactive/Game States/J State Tracker")]
    public sealed class J_SimpleStateTracker : J_StateTracker<J_State>
    {
        protected override J_StateControl<J_State> _stateControl => _simpleStateControl;
        [BoxGroup("Setup", true, true), SerializeField, Required, AssetsOnly] private J_SimpleStateControl _simpleStateControl;
    }
}
