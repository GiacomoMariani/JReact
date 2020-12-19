using JReact.ScreenMessage;
using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.Conditions.Tasks
{
    /// <summary>
    /// this is a tutorial that sends line of text
    /// </summary>
    [CreateAssetMenu(menuName = "Reactive/Task/Tutorial Message")]
    public class J_MessageTask : J_CompletableTask
    {
        [BoxGroup("Setup", true, true), SerializeField, AssetsOnly, Required] private J_MessageSender _messageSender;
        //the string related to this tutorial
        [BoxGroup("Setup", true, true), SerializeField] private string _message;

        //sends the message required by this tutorial
        protected override void RunTask()
        {
            base.RunTask();
            _messageSender.Send(_message);
        }
    }
}
