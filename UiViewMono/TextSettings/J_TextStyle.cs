using System;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace Jreact.UiViewMono.TextSettings
{
    public sealed class J_TextStyle : MonoBehaviour
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField, ChildGameObjectsOnly, Required]
        private TextMeshProUGUI _text;

        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private J_SO_TextFont _font;

        // --------------- EVENTS --------------- //
        private void UpdateText(J_St_TextSettings textSettings)
        {
            J_SO_TextSize textSize    = textSettings.CurrentSize;
            bool          hasFontSize = textSize.HasFontSize(_font, out float size);

            if (!hasFontSize ||
                Mathf.Approximately(size, _text.fontSize)) { return; }

            _text.fontSize = size;
        }

        private void OnEnable()
        {
            UpdateText(J_St_TextSettings.GetInstanceSafe());
            J_St_TextSettings.GetInstanceSafe().Subscribe(UpdateText);
        }

        private void OnDisable() { J_St_TextSettings.GetInstanceSafe().Unsubscribe(UpdateText); }

        [Button]
        private void ResetStyle()
        {
            if (_text.font == _font.FontAsset) { return; }

            _text.font     = _font.FontAsset;
            _text.fontSize = _font.DefaultSize;
        }

        private void OnValidate()
        {
#if UNITY_EDITOR
            if (UnityEditor.PrefabUtility.IsPartOfPrefabAsset(this)) { ResetStyle(); }
#endif
        }
    }
}
