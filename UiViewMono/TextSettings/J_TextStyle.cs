#if NX_BITBUFFER
using Sirenix.OdinInspector;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace Jreact.UiViewMono.TextSettings
{
    public sealed class J_TextStyle : MonoBehaviour
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField, Required]
        private TextMeshProUGUI _text;

        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private J_SO_01_TextType _textType;
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private int _sizeIndex;

        // --------------- EVENTS --------------- //
        private void UpdateText(J_St_TextSettings textSettings)
        {
            // --------------- SIZE --------------- //
            JTextSetting setting  = textSettings.GetTextSetting(_textType);
            float        fontSize = _textType.GetFontSize(setting, _sizeIndex);
            SetSize(fontSize);
            // --------------- FONT --------------- //
            TMP_FontAsset font = _textType.GetFontAsset(setting);
            SetFont(font);
        }

        private void SetFont(TMP_FontAsset font)
        {
            if (font == _text.font) { return; }

            _text.font = font;
        }

        private void SetSize(float fontSize)
        {
            if (Mathf.Approximately(fontSize, _text.fontSize)) { return; }

            _text.fontSize = fontSize;
        }

        private async void OnEnable()
        {
            await J_St_TextSettings.WaitForInit(this);
            UpdateText(J_St_TextSettings.GetInstanceSafe());
            J_St_TextSettings.GetInstanceSafe().Subscribe(UpdateText);
        }

        private void OnDisable()
        {
            if(J_St_TextSettings.IsSingletonAlive) { J_St_TextSettings.GetInstanceSafe().Unsubscribe(UpdateText); }
        }

        [Button]
        private void ResetStyle()
        {
            if (_text     == default ||
                _textType == default) { return; }

            // --------------- SIZE --------------- //
            float fontSize = _textType.GetDefaultFontSize(_sizeIndex);
            SetSize(fontSize);

            // --------------- FONT --------------- //
            TMP_FontAsset font = _textType.GetDefaultFontAsset();
            SetFont(font); //
        }

        private void OnValidate()
        {
#if UNITY_EDITOR
            if (PrefabUtility.IsPartOfPrefabAsset(this)) { ResetStyle(); }
#endif
        }
    }
}
#endif
