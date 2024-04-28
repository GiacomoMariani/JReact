using Sirenix.OdinInspector;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace JReact.SceneControls
{
    [CreateAssetMenu(menuName = "Reactive/Scenes/Quit")]
    public class J_ApplicationQuit : J_ProcessableAction
    {
        [BoxGroup("Setup", true, true), SerializeField] private int _exitCode;

        public override void Process() => Quit();

        [ButtonGroup("Commands", 200), Button("Activate", ButtonSizes.Medium)]
        public void Quit()
        {
            //send the event before the quit
            Application.wantsToQuit += WantsToQuit;
#if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
#endif
            JLog.Log($"{name} Quit - Exit Code {_exitCode}", JLogTags.State, this);
            Application.Quit();

            Application.wantsToQuit -= WantsToQuit;
        }

        protected virtual bool WantsToQuit() { return true; }
    }
}
