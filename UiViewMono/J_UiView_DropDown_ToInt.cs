using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact
{
    public sealed class J_UiView_DropDown_ToInt : MonoBehaviour
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        [BoxGroup("Setup", true, true), SerializeField, AssetsOnly, Required] private J_ReactiveInt _intValue;
        [BoxGroup("Setup", true, true), SerializeField] private TMP_Dropdown _dropDown;

        // --------------- INITIALIZATION --------------- //
        private void Awake()
        {
            InitThis();
            SanityChecks();
        }

        private void InitThis()
        {
            if (_dropDown == null) _dropDown = GetComponent<TMP_Dropdown>();
        }

        private void SanityChecks()
        {
            Assert.IsNotNull(_dropDown, $"{gameObject.name} requires a {nameof(_dropDown)}");
            Assert.IsNotNull(_intValue, $"{gameObject.name} requires a {nameof(_intValue)}");
        }

        // --------------- COMMAND --------------- //
        private void IntUpdate(int newIntValue) => _intValue.Current = newIntValue;

        // --------------- LISTENER SETUP --------------- //
        private void OnEnable()
        {
            _dropDown.value = _intValue.Current;
            _dropDown.onValueChanged.AddListener(IntUpdate);
        }

        private void OnDisable() { _dropDown.onValueChanged.RemoveListener(IntUpdate); }
    }
}
