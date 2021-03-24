using JReact.UiView;
using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.StateControl.PopUp
{
    public abstract class J_UiVIew_PopupButton_Generic<T> : J_UiView_ButtonItem
        where T : J_State
    {
        // --------------- SETUP --------------- //
        private enum PopUpButtonType { Confirm, Deny }
        protected abstract J_GenericPopup<T> _popUp { get; }

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
