using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.UiView.Collections
{
    public sealed class J_UiView_PageChangeButton : J_UiView_ButtonItem
    {
        // --------------- SETUP --------------- //
        [BoxGroup("Setup", true, true), SerializeField] private bool _forward;
        [BoxGroup("Setup", true, true), SerializeField, Required] private J_PagerEvents _events;

        protected override void ButtonCommand()
        {
        }

        private void CheckInteractivity(int index)
        {
            _button.interactable = _forward
                                          ? _events.CanGoForward
                                          : _events.CanGoBack;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            CheckInteractivity(_events.Total);
            _events.OnIndexChanged += CheckInteractivity;
            _events.OnTotalChanged += CheckInteractivity;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            _events.OnIndexChanged -= CheckInteractivity;
            _events.OnTotalChanged -= CheckInteractivity;
        }
    }
}
