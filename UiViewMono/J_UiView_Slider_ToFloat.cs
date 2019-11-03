using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace JReact.UiView
{
    [RequireComponent(typeof(Slider))]
    public class J_UiView_Slider_ToFloat : MonoBehaviour
    {
        // --------------- FIELDS --------------- //
        [BoxGroup("Setup", true, true), SerializeField, AssetsOnly, Required] private J_ReactiveFloat _floatValue;

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private Slider _slider;

        // --------------- INITIALIZATION --------------- //
        private void Awake()
        {
            if (_slider == null) _slider = GetComponentInChildren<Slider>(true);
            Assert.IsNotNull(_slider, $"{gameObject.name} requires a {nameof(_slider)}");
        }
        
        // --------------- UPDATES --------------- //
        private void UpdateValue(float sliderValue) => _floatValue.Current = sliderValue;
        private void UpdateSlider(float current) => _slider.SetValueWithoutNotify(current);
        
        // --------------- LISTENERS --------------- //
        private void OnEnable()
        {
            _slider.value = _floatValue.Current;
            _slider.onValueChanged.AddListener(UpdateValue);
            _floatValue.Subscribe(UpdateSlider);
        }

        private void OnDisable()
        {
            _slider.onValueChanged.RemoveListener(UpdateValue); 
            _floatValue.UnSubscribe(UpdateSlider);
        }
    }
}
