using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace JReact
{
    public abstract class J_ProcessableAction : ScriptableObject, iProcessable
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField] protected string _actionDescription;
        public string ActionDescription => _actionDescription;

        public UnityAction ThisAction => Process;
        public abstract void Process();
    }

    public static class ProcessableActionExtension
    {
        public static void ProcessAll(J_ProcessableAction[] actions)
        {
            for (int i = 0; i < actions.Length; i++) { actions[i].Process(); }
        }
    }
}
