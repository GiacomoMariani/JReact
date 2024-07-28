using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.ScreenMessage
{
    /// <summary>
    /// shows a view of the given message
    /// </summary>
    public class J_Mono_MessageTypeView : J_Mono_ActorElement<JMessage>, iInitiator<J_Mono_MessagePrinter>
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        [BoxGroup("Setup", true, true), SerializeField, Required] private GameObject[] _views;

        [BoxGroup("Setup", true, true), SerializeField] private bool _disableWhenPrinting;
        [BoxGroup("Setup", true, true), SerializeField] private bool _enableWhenPrinting;

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private J_Mono_MessagePrinter _printer;

        // --------------- BASE VIEW --------------- //
        //update the views
        protected override void ActorUpdate(JMessage element)
        {
            //check if this is a valid view and activate views accordingly
            bool isValid = ValidPrinterState();
            ShowViews(isValid);
        }

        private void ShowViews(bool isActiveToShow)
        {
            //activate the views accordingly
            for (int i = 0; i < _views.Length; i++)
            {
                if (isActiveToShow != _views[i].activeSelf) _views[i].SetActive(isActiveToShow);
            }
        }

        // --------------- PRINT EFFECT --------------- //
        //used to inject the printer effect
        public void InjectThis(J_Mono_MessagePrinter printEffect)
        {
            //ignore if we do not require the print effect
            if (!_disableWhenPrinting &&
                !_enableWhenPrinting) return;

            //set the print controls
            _printer = printEffect;
            //update the state
            ActorUpdate(_actor);
            //subscribe to change
            _printer.OnPrinting += PrinterUpdate;
        }

        //update when the print state changes
        private void PrinterUpdate(bool isActive) { ActorUpdate(_actor); }

        //update with print effect
        private bool ValidPrinterState()
        {
            //we return true if we don't require tracking the print effect
            if (!_disableWhenPrinting &&
                !_enableWhenPrinting) return true;

            //required when enabled
            if (_enableWhenPrinting)
            {
                //false if we have no print effect yet, or if it is still printing
                if (!_printer) return false;
                return _printer.IsPrinting;
            }

            //required when disabled
            if (_disableWhenPrinting)
            {
                //false if we have no print effect yet, or if it is still printing
                if (!_printer) return false;
                return !_printer.IsPrinting;
            }

            //return true in all other cases
            return true;
        }

        // --------------- UNITY EVENTS --------------- //
        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (_printer != null) _printer.OnPrinting -= PrinterUpdate;
        }
    }
}
