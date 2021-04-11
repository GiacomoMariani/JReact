using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.StateControl
{
    /// <summary>
    /// used to track the flow of events to move back to a previous state
    /// </summary>
    [CreateAssetMenu(menuName = "Reactive/Game States/J State Tracker")]
    public sealed class J_StateTracker : J_ABS_StateTracker<J_State>
    {
    }
}
