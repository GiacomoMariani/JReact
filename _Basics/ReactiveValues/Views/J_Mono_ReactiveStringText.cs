using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.UiView
{
    /// <summary>
    /// used to show the message on the text
    /// </summary>
    public class J_Mono_ReactiveStringText : J_UiView_Text
    {
        //the string with the text
        [BoxGroup("Setup", true, true), SerializeField, AssetsOnly, Required] private J_ReactiveString _stringScreenMessage;

        //start and stop tracking on enable
        private void OnEnable()
        {
            SetText(_stringScreenMessage.Current);
            _stringScreenMessage.Subscribe(SetText);
        }

        private void OnDisable() { _stringScreenMessage.UnSubscribe(SetText); }
    }
}
