using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace JReact.UiViewMono
{
    [RequireComponent(typeof(Toggle))]
    public class J_UiView_ToggleViews : MonoBehaviour
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        [InfoBox("NULL => no activation at On"), BoxGroup("Setup", true, true, 0), SerializeField]
        private J_Mono_ViewActivator _onViews;
        [InfoBox("NULL => no activation at Off"), BoxGroup("Setup", true, true, 0), SerializeField]
        private J_Mono_ViewActivator _offViews;
        [BoxGroup("Setup", true, true, 0), SerializeField] private Toggle _toggle;

        // --------------- INITIALIZATION --------------- //
        private void Awake() { SanityChecks(); }

        private void SanityChecks() { Assert.IsNotNull(_toggle, $"{gameObject.name} requires a {nameof(_toggle)}"); }

        // --------------- COMMAND --------------- //
        private void UpdateViews(bool isOn)
        {
            _onViews.ActivateView(isOn);
            _offViews.ActivateView(!isOn);
        }

        // --------------- LISTENER SETUP --------------- //
        private void OnEnable()
        {
            UpdateViews(_toggle.isOn);
            _toggle.onValueChanged.AddListener(UpdateViews);
        }

        private void OnDisable() => _toggle.onValueChanged.RemoveListener(UpdateViews);
    }
}
