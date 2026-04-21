using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.Conditions
{
    public abstract class J_AbsSO_Condition : ScriptableObject
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        [BoxGroup("Setup", true, true, 0), ReadOnly] public abstract bool IsTrue { get; }
        
        public static bool operator true(J_AbsSO_Condition  item) => item.IsTrue;
        public static bool operator false(J_AbsSO_Condition item) => !item.IsTrue;

        public static implicit operator bool(J_AbsSO_Condition condition) => condition.IsTrue;
    }
}
