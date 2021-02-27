using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace JReact.UiView.Collections
{
    /// <summary>
    /// shows the index of a page
    /// </summary>
    public sealed class J_UiView_PageIndex : J_UiView_Text
    {
        private const string _format = "{0} / {1}";

        // --------------- FIELDS AND PROPERTIES --------------- //
        [BoxGroup("Setup", true, true), SerializeField, Required] private J_PagerEvents _events;

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private List<Image> _currentPoints = new List<Image>();

        // --------------- METHODS --------------- //
        private void ChangeIndex(int index) => SetText(string.Format(_format, index, _events.Total));

        private void OnEnable()
        {
            ChangeIndex(_events.Current);
            _events.OnIndexChanged += ChangeIndex;
        }

        private void OnDisable() { _events.OnIndexChanged -= ChangeIndex; }
    }
}
