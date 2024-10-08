using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.JScreen.View
{
    [RequireComponent(typeof(TMP_Dropdown))]
    public class J_UiView_ScreenResolution_DropDown : MonoBehaviour
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private J_St_ScreenControls _resolutions;
        [BoxGroup("Setup", true, true, 0), SerializeField, Required] private TMP_Dropdown _dropDown;

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
            // _dropDown.AddOptions(_resolutions.GetResolutionsAsString());
            // _dropDown.value = _resolutions.GetResolutionIndex();
        }

        // --------------- ACTION --------------- //        
        private void SetResolution(int resolutionIndex) => _resolutions.SetResolution(resolutionIndex);
        
        // --------------- LISTENER SETUP --------------- //
        private void OnEnable()
        {
            // _dropDown.value = _resolutions.GetResolutionIndex();
            _dropDown.onValueChanged.AddListener(SetResolution);
        }

        private void OnDisable() { _dropDown.onValueChanged.RemoveListener(SetResolution); }
    }
}
