using System.Collections.Generic;
using JReact.Pool;
using MEC;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;

namespace JReact.Tooltips
{
    public class J_TooltipActivator<T> : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ITooltipController
        where T : J_Tooltip
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField, ChildGameObjectsOnly, Required]
        protected RectTransform _rectTransform;
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private T _prefab;
        [BoxGroup("Setup", true, true, 0), SerializeField] private float _secondsBeforeOpening = 0.5f;
        [BoxGroup("Setup", true, true, 0), SerializeField] private J_AllDirectionEnum _directionEnum;

        [BoxGroup("Setup", true, true, 0), SerializeField] private Vector2 _offset;

        //in case we want to connect the life of the tooltip with another tooltip
        [BoxGroup("Setup", true, true, 0), SerializeField, ChildGameObjectsOnly] private J_Tooltip _connection;

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private J_Direction Direction => _directionEnum.ToDirection();
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] protected T _tooltipShown;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private CoroutineHandle _openAfterTimeCoroutine;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private bool IsOpening => _openAfterTimeCoroutine.IsRunning;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private bool IsOpen => _tooltipShown != default;

        // --------------- OPENING --------------- //
        private void Awake()
        {
            //make sure we have a place to show tooltips
            var tooltipRoot = J_TootlipRoot.GetInstanceSafe();
            Assert.IsNotNull(tooltipRoot, $"{gameObject.name} requires a {nameof(tooltipRoot)}");
            _prefab.AssurePool(parent: tooltipRoot.Root);
        }

        private void ActivateOpening()
        {
            _openAfterTimeCoroutine = Timing.RunCoroutine(WaitThenOpen().CancelWith(gameObject), Segment.Update);
        }

        private IEnumerator<float> WaitThenOpen()
        {
            float seconds = 0f;
            while (seconds < _secondsBeforeOpening)
            {
                seconds += Time.deltaTime;
                yield return Timing.WaitForOneFrame;
            }

            ActivateTooltip();
        }

        // --------------- ACTIVATION --------------- //
        protected virtual void ActivateTooltip()
        {
            Assert.IsNull(_tooltipShown, $"{gameObject.name} this should be null: {nameof(_tooltipShown)}");

            _tooltipShown = _prefab.Spawn();
            SetupTooltip(_tooltipShown);
            _tooltipShown.SetOnActivator(true);
            _tooltipShown.PlaceAboveRectTransform(_rectTransform, Direction, _offset);
            _tooltipShown.StartTooltipLoop(this);
        }

        protected virtual void SetupTooltip(T tooltip) {}

        // --------------- CLOSE --------------- //
        private void PointerMovedOut()
        {
            if (IsOpening) { Timing.KillCoroutines(_openAfterTimeCoroutine); }

            if (IsOpen) { _tooltipShown.SetOnActivator(false); }
        }

        public void RemoveTooltip(J_Tooltip tooltip)
        {
            Assert.AreEqual(tooltip, _tooltipShown, $"{name} expected {_tooltipShown.gameObject}, but got {tooltip.gameObject}");
            _prefab.DeSpawn(tooltip);
            _tooltipShown = default;
        }

        public bool KeepActive() => _connection && _connection.IsOpen;

        // --------------- INTERFACE IMPLEMENTATION --------------- //
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (IsOpening) { return; }

            if (!IsOpen) { ActivateOpening(); }
            //if we move out of the tooltip back on the activator
            else { _tooltipShown.SetOnActivator(true); }
        }

        public void OnPointerExit(PointerEventData eventData) { PointerMovedOut(); }

        private void OnDisable() { PointerMovedOut(); }

        // --------------- HELPERS --------------- //
        private void OnValidate()
        {
            if (_rectTransform == null) { _rectTransform = GetComponent<RectTransform>(); }
        }
    }
    public interface ITooltipController
    {
        public void RemoveTooltip(J_Tooltip tooltip);
        public bool KeepActive();
    }
}
