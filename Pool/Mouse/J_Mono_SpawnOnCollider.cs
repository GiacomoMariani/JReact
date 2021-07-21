using JReact.Pool.SpecialEffect;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

namespace JReact
{
    /// <summary>
    /// spawn effects when pointer is on a gameobject
    /// </summary>
    public sealed class J_Mono_SpawnOnCollider : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        // --------------- FIELDS AND VALUES --------------- //
        [BoxGroup("Setup", true, true), SerializeField, AssetsOnly, Required] private J_Pool_SpecialEffects _effectsPool;
        [BoxGroup("Setup", true, true), SerializeField, Required] private Camera _mainCamera;
        [BoxGroup("Setup", true, true), SerializeField] private bool _spawnOnEnter;
        [BoxGroup("Setup", true, true), SerializeField] private bool _spawnOnExit;
        [BoxGroup("Setup", true, true), SerializeField] private Quaternion _particleRotation;

        // --------------- EVENT STARTERS --------------- //
        private void EnterArea()
        {
            if (_spawnOnEnter) { _effectsPool.TriggerEffectOnPosition(GetMousePosition(), _particleRotation); }
        }

        private void ExitArea()
        {
            if (_spawnOnExit) { _effectsPool.TriggerEffectOnPosition(GetMousePosition(), _particleRotation); }
        }

        // --------------- HELPER --------------- //
        //this method is used to get the mouse current position
        private Vector3 GetMousePosition() => _mainCamera.ScreenToWorldPoint(Input.mousePosition);

        // --------------- INTERFACE IMPLEMENTATION --------------- //
        //when mouse enter and exit the collider
        public void OnPointerEnter(PointerEventData eventData) { EnterArea(); }
        public void OnPointerExit(PointerEventData  eventData) { ExitArea(); }
    }
}
