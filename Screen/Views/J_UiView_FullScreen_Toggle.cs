using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace JReact.JScreen.View
{
    [RequireComponent(typeof(Toggle))]
    public class J_UiView_FullScreen_Toggle : MonoBehaviour
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private J_St_ScreenControls _resolutions;
        [BoxGroup("Setup", true, true, 0), SerializeField, Required] private Toggle _toggle;
        [BoxGroup("Setup", true, true, 0), SerializeField] private bool _revertToggleValue;

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
            Assert.IsNotNull(_resolutions, $"{gameObject.name} requires a {nameof(_resolutions)}");
            Assert.IsNotNull(_toggle,      $"{gameObject.name} requires a {nameof(_toggle)}");
        }

        // --------------- COMMAND --------------- //
        private void SetFullScreen(bool toggleValue)
        {
            if (_revertToggleValue) _resolutions.SetFullScreen(!toggleValue);
            else _resolutions.SetFullScreen(toggleValue);
        }

        // --------------- LISTENER SETUP --------------- //
        private void OnEnable()
        {
            _toggle.isOn = _revertToggleValue ? !_resolutions.IsFullScreen : _resolutions.IsFullScreen;
            _toggle.onValueChanged.AddListener(SetFullScreen);
        }

        private void OnDisable() { _toggle.onValueChanged.RemoveListener(SetFullScreen); }
    }
}
