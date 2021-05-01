using UnityEngine;
using UnityEngine.Events;

namespace JReact
{
    public abstract class J_ProcessableAction : ScriptableObject, iProcessable
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
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
