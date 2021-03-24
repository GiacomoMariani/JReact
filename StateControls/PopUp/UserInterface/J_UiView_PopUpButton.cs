using JReact.UiView;
using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.StateControl.PopUp
{
    //this is used by a pop up command
    public sealed class J_UiView_PopUpButton : J_UiView_ButtonItem
    {
        // --------------- SETUP --------------- //
        private enum PopUpButtonType { Confirm, Deny }
        [BoxGroup("Setup", true, true), SerializeField, AssetsOnly, Required] private J_PopUp _popUp;
        [BoxGroup("Setup", true, true), SerializeField] private PopUpButtonType _buttonType;

        // --------------- STATE and BOOK KEEPING --------------- //
        [FoldoutGroup("Book Keeping", false, 10), ReadOnly, ShowInInspector] private JUnityEvent _commandAction
        {
            get
            {
                switch (_buttonType)
                {
                    case PopUpButtonType.Confirm: return _popUp.ConfirmAction;
                    case PopUpButtonType.Deny:    return _popUp.CancelAction;
                    default:                      return null;
                }
            }
        }

        // --------------- METHODS --------------- //
        protected override void ButtonCommand() { _commandAction?.Invoke(); }
    }
}
