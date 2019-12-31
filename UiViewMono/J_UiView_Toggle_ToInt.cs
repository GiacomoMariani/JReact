using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace JReact
{
    public sealed class J_UiView_Toggle_ToInt : MonoBehaviour
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        [BoxGroup("Setup", true, true), SerializeField, AssetsOnly, Required] private J_ReactiveInt _intValue;
        [BoxGroup("Setup", true, true), SerializeField] private Toggle _toggle;

        // --------------- INITIALIZATION --------------- //
        private void Awake()
        {
            InitThis();
            SanityChecks();
        }

        private void InitThis()
        {
            if (_toggle == null) _toggle = GetComponent<Toggle>();
        }

        private void SanityChecks()
        {
            Assert.IsNotNull(_toggle,   $"{gameObject.name} requires a {nameof(_toggle)}");
            Assert.IsNotNull(_intValue, $"{gameObject.name} requires a {nameof(_intValue)}");
        }

        // --------------- COMMAND --------------- //
        private void IsOnUpdate(bool isOn) => _intValue.Current = isOn
                                                                      ? 1
                                                                      : 0;

        // --------------- LISTENER SETUP --------------- //
        private void OnEnable()
        {
            _toggle.isOn = _intValue.Current > 0;
            _toggle.onValueChanged.AddListener(IsOnUpdate);
        }

        private void OnDisable() { _toggle.onValueChanged.RemoveListener(IsOnUpdate); }
    }
}
