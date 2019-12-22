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

        protected virtual void InitThis()
        {
            if (_button == null) _button = GetComponent<Button>();
        }

        protected virtual void SanityChecks() => Assert.IsNotNull(_button, $"{gameObject.name} requires a {nameof(_button)}");

        // --------------- IMPLEMENTATION --------------- //
        //the main command sent by this button
        protected abstract void ButtonCommand();

        // --------------- LISTENERS --------------- //
        //start and stop tracking on enable
        protected virtual void OnEnable()
        {
            _button.onClick.RemoveListener(ButtonCommand);
            _button.onClick.AddListener(ButtonCommand);
        }

        protected virtual void OnDisable() => _button.onClick.RemoveListener(ButtonCommand);
    }
}
