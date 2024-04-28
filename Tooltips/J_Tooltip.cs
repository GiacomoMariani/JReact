using System;
using System.Collections.Generic;
using MEC;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;

namespace JReact.Tooltips
{
    public class J_Tooltip : MonoBehaviour,
                             IPointerEnterHandler,
                             IPointerExitHandler,
                             IPointerClickHandler
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField, ChildGameObjectsOnly, Required]
        private RectTransform _rectTransform;

        [BoxGroup(                                 "Setup", true, true, 0), SerializeField] private bool _canBePermanent = true;
        [ShowIf(nameof(_canBePermanent)), BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required]
        private PointerEventData.InputButton _permanenceButton;

        // --------------- STATE --------------- //
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public bool IsMouseHovering { get; private set; } = false;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public bool IsOnActivator { get; private set; } = false;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public bool IsPermanent { get; private set; } = false;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public bool IsOpen => _tooltipLoop.IsRunning;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private ITooltipController _controller;

        // --------------- CANCELLATION --------------- //
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private CoroutineHandle _tooltipLoop;

        // --------------- INITIALIZATION --------------- //
        private void CloseSafe()
        {
            Timing.KillCoroutines(_tooltipLoop);
            _controller.RemoveTooltip(this);
            IsMouseHovering = false;
            IsOnActivator   = false;
            IsPermanent     = false;
            _controller     = default;
        }

        // --------------- PLACEMENT --------------- //
        /// <summary>
        /// opens a tooltip above another ui
        /// </summary>
        /// <param name="parentTransform">the related item we want to use</param>
        /// <param name="direction">the direction we want to open this tooltip</param>
        /// <param name="offset">the offset, for manual alignment</param>
        /// <returns>returns the same tooltip, for fluent syntax</returns>
        internal virtual J_Tooltip PlaceAboveRectTransform(RectTransform parentTransform, J_Direction direction, Vector2 offset)
        {
            //switching parent to make sure it's on top also at the end of the opening
            var oldParent = _rectTransform.parent;
            _rectTransform.SetParent(parentTransform);
            _rectTransform.SetDirection(direction).WithAnchoredOffset(offset);
            _rectTransform.SetParent(oldParent);
            _rectTransform.SetAsLastSibling();
            _rectTransform.gameObject.SetActive(true);
            return this;
        }

        private IEnumerator<float> TooltipLoop()
        {
            while (IsOnActivator   ||
                   IsMouseHovering ||
                   IsPermanent     ||
                   _controller.KeepActive()) { yield return Timing.WaitForOneFrame; }

            CloseSafe();
        }

        // --------------- COMMANDS --------------- //
        private void TogglePermanent()
        {
            IsPermanent = !IsPermanent;
            if (!IsPermanent) { CloseSafe(); }
        }

        internal void StartTooltipLoop(ITooltipController controller)
        {
            if (!IsOpen)
            {
                _controller  = controller;
                _tooltipLoop = Timing.RunCoroutine(TooltipLoop().CancelWith(gameObject), Segment.Update);
            }
        }

        internal void SetOnActivator(bool isOnOpener) { IsOnActivator = isOnOpener; }

        // --------------- EVENT LISTENERS --------------- //
        public void OnPointerEnter(PointerEventData eventData) { IsMouseHovering = true; }

        public void OnPointerExit(PointerEventData eventData) { IsMouseHovering = false; }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_canBePermanent && eventData.button == _permanenceButton) { TogglePermanent(); }
        }

        // --------------- HELPERS --------------- //
        private void OnValidate()
        {
            if (_rectTransform == null) { _rectTransform = GetComponent<RectTransform>(); }
        }
    }
}
