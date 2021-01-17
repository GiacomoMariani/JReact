using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

namespace JReact.J_Input
{
    public class J_Mono_Input_Command : MonoBehaviour
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private InputAction _input;
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private J_ProcessableAction[] _actions;

        // --------------- INITIALIZATION --------------- //
        private void Awake() { SanityChecks(); }

        private void SanityChecks()
        {
            Assert.IsNotNull(_input,   $"{gameObject.name} requires a {nameof(_input)}");
            Assert.IsNotNull(_actions, $"{gameObject.name} requires a {nameof(_actions)}");
            for (int i = 0; i < _actions.Length; i++) { Assert.IsNotNull(_actions[i], $"{gameObject.name} null action at {i}"); }
        }

        private void Process(InputAction.CallbackContext callback)
        {
            for (int i = 0; i < _actions.Length; i++) { _actions[i].Process(); }
        }

        // --------------- LISTENER SETUP --------------- //
        private void OnEnable()
        {
            _input.performed += Process;
            _input.Enable();
        }

        private void OnDisable()
        {
            _input.Disable();
            _input.performed -= Process;
        }
    }
}
