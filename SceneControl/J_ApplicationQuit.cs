using System;
using Sirenix.OdinInspector;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace JReact.SceneControls
{
    [Serializable]
    public class J_ApplicationQuit
    {
        [BoxGroup("Setup", true, true, 0), SerializeField] private int _exitCode = 0;
        public J_ApplicationQuit(int exitCode) => _exitCode = exitCode;

        [ButtonGroup("Commands", 200), Button("Activate", ButtonSizes.Medium)]
        public void Quit()
        {
            //send the event before the quit
            Application.wantsToQuit += WantsToQuit;
#if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
#endif
            JLog.Log($"Quit - Exit Code {_exitCode}", JLogTags.State);
            Application.Quit();

            Application.wantsToQuit -= WantsToQuit;
        }

        protected virtual bool WantsToQuit() => true;
    }
}
