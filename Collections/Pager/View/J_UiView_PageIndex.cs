using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace JReact.UiView.Collections
{
    /// <summary>
    /// shows the index of a page
    /// </summary>
    public sealed class J_UiView_PageIndex : J_UiView_TextElement
    {
        private const string _format = "{0} / {1}";

        // --------------- FIELDS AND PROPERTIES --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField, Required] private J_Pager _pager;

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private List<Image> _currentPoints = new List<Image>();

        // --------------- METHODS --------------- //
        private void ChangeIndex(J_UiView_Page page) => SetText(string.Format(_format, page.Identifier, _pager.TotalPages));

        protected void OnEnable()
        {
            ChangeIndex(_pager.Current);
            _pager.OnPage_Change += ChangeIndex;
        }

        protected void OnDisable() { _pager.OnPage_Change -= ChangeIndex; }
    }
}
