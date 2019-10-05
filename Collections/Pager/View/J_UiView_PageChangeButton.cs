using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.UiView.Collections
{
    public class J_UiView_PageChangeButton : J_UiView_ButtonElement
    {
        // --------------- SETUP --------------- //
        [BoxGroup("Setup", true, true), SerializeField] private bool _forward;
        [BoxGroup("Setup", true, true), SerializeField, Required] private J_Pager _pager;

        protected override void ButtonCommand()
        {
            if (_forward) _pager.GoForward();
            else _pager.GoBack();
        }

        private void CheckInteractivity(J_UiView_Page page)
        {
            _button.interactable = _forward
                                          ? _pager.CanGoForward
                                          : _pager.CanGoBack;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            CheckInteractivity(_pager.Current);
            _pager.OnPage_Change += CheckInteractivity;
            _pager.OnPage_Create += CheckInteractivity;
            _pager.OnPage_Remove += CheckInteractivity;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            _pager.OnPage_Change -= CheckInteractivity;
            _pager.OnPage_Create -= CheckInteractivity;
            _pager.OnPage_Remove -= CheckInteractivity;
        }
    }
}
