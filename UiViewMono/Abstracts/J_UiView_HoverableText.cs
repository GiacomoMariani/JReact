using System;
using System.Collections.Generic;
using MEC;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace JReact.UiView
{
    public class J_UiView_HoverableText : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler
    {
        // --------------- SETUP --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField, ChildGameObjectsOnly, Required]
        private TextMeshProUGUI _text;

        //can be null
        [BoxGroup("Setup", true, true, 0), SerializeField] private Camera _camera;

        // --------------- STATE --------------- //
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private CoroutineHandle _coroutine;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public string HoverKey { get; private set; }
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public float HoverTiming { get; private set; }

        /// <summary>
        /// set the camera in case it is required for specific Canvas (Screen Space Overlay requires a null camera)
        /// </summary>
        /// <param name="cameraToSet">The scene camera which may be assigned to a Canvas using ScreenSpace Camera or WorldSpace render
        /// mode. Set to null is using ScreenSpace Overlay.</param>
        public void SetCamera(Camera cameraToSet) { _camera = cameraToSet; }

        private IEnumerator<float> HoveringText()
        {
            while (true)
            {
                yield return Timing.WaitForOneFrame;
                // --------------- CHECK THE LINK ON THE POSITION --------------- //
                var mousePosition = Mouse.current.position.ReadValue();
                int linkIndex     = TMP_TextUtilities.FindIntersectingWord(_text, mousePosition, _camera);

                //OPTION 1 - No Text => Reset Hovering
                if (linkIndex == -1)
                {
                    NoTextHovering();
                    continue;
                }

                TMP_WordInfo linkInfo     = _text.textInfo.wordInfo[linkIndex];
                string       currentHover = linkInfo.GetWord();

                //OPTION 2 - Same string => add timing
                if (currentHover == HoverKey) { CountTime(Time.deltaTime); }
                //OPTION 3 - New string => start counting
                else { StartCountingFor(currentHover); }
            }
        }

        private void NoTextHovering()
        {
            HoverTiming = 0f;
            HoverKey    = string.Empty;
        }

        private void CountTime(float seconds) { HoverTiming += seconds; }

        private void StartCountingFor(string currentHover)
        {
            HoverKey    = currentHover;
            HoverTiming = 0f;
        }

        // --------------- EVENT LISTENERS --------------- //
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!_coroutine.IsRunning) { _coroutine = Timing.RunCoroutine(HoveringText().CancelWith(gameObject), Segment.Update); }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Timing.KillCoroutines(_coroutine);
            NoTextHovering();
        }

        // --------------- HELPERS --------------- //
        private void OnValidate()
        {
            if (_text == null) { _text = GetComponent<TextMeshProUGUI>(); }
        }
    }
}
