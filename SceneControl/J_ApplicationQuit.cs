using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.SceneControls
{
    [CreateAssetMenu(menuName = "Reactive/Scenes/Quit")]
    public sealed class J_ApplicationQuit : J_ProcessableAction, jObservable
    {
        private event Action OnQuit;

        [BoxGroup("Setup", true, true), SerializeField] private int _exitCode;

        public override void Process() => Quit();

        [ButtonGroup("Commands", 200), Button("Activate", ButtonSizes.Medium)]
        public void Quit()
        {
            //send the event before the quit
            OnQuit?.Invoke();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
            JLog.Log($"{name} Quit - Exit Code {_exitCode}", JLogTags.State, this);
            Application.Quit();
        }

        public void Subscribe(Action   actionToSubscribe) { OnQuit += actionToSubscribe; }
        public void UnSubscribe(Action actionToSubscribe) { OnQuit -= actionToSubscribe; }
    }
}
