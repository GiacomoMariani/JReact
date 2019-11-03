using System.Collections.Generic;
using JReact.JScreen;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace JReact.JScreen.View
{
    [RequireComponent(typeof(Dropdown))]
    public class J_UiView_ScreenResolution_DropDown : MonoBehaviour
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private J_ScreenResolutions _resolutions;
        [BoxGroup("Setup", true, true, 0), SerializeField, Required] private Dropdown _dropDown;

        // --------------- INITIALIZATION --------------- //
        private void Awake()
        {
            SanityChecks();
            InitThis();
        }

        private void SanityChecks() { Assert.IsNotNull(_resolutions, $"{gameObject.name} requires a {nameof(_resolutions)}"); }

        private void InitThis()
        {
            //fill the dropdown
            _dropDown.ClearOptions();
            _dropDown.AddOptions(_resolutions.GetResolutionAsString());
            _dropDown.value = _resolutions.GetResolutionIndex();
        }

        // --------------- ACTION --------------- //        
        private void SetResolution(int resolutionIndex) => _resolutions.SetResolution(resolutionIndex);

        // --------------- LISTENER SETUP --------------- //
        private void OnEnable()
        {
            _dropDown.value = _resolutions.GetResolutionIndex();
            _dropDown.onValueChanged.AddListener(SetResolution);
        }

        private void OnDisable() { _dropDown.onValueChanged.RemoveListener(SetResolution); }
    }
}
