using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace JReact.UiView
{
    /// <summary>
    /// used to show a button
    /// </summary>
    [RequireComponent(typeof(Button))]
    public abstract class J_UiView_ButtonElement : MonoBehaviour
    {
        //the button related to this element
        [BoxGroup("Setup", true, true, 0), SerializeField, Required] protected Button _button;

        // --------------- INITIALIZATION --------------- //
        private void Awake()
        {
            InitThis();
            SanityChecks();
        }

        protected virtual void InitThis() {}

        protected virtual void SanityChecks() => Assert.IsNotNull(_button, $"{gameObject.name} requires a {nameof(_button)}");

        // --------------- PRECHECKS --------------- //
        //this is used in case we want to apply any condition, as default it is true
        protected virtual bool CanBePressed() => true;

        //try pressing the button, send the command if the button can be pressed
        private void TryPressButton()
        {
            if (CanBePressed()) ButtonCommand();
        }

        // --------------- IMPLEMENTATION --------------- //
        //the main command sent by this button
        protected abstract void ButtonCommand();

        // --------------- LISTENERS --------------- //
        //start and stop tracking on enable
        protected virtual void OnEnable()  => _button.onClick.AddListener(TryPressButton);
        protected virtual void OnDisable() => _button.onClick.RemoveListener(TryPressButton);
        
#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_button == null) _button = GetComponent<Button>();
        }
#endif
    }
}
