#if NX_BITBUFFER
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace JReact.Localization.LocalizationText
{
    public sealed class J_LocalizedText : MonoBehaviour
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private J_SO_LocalizationLibrary _library;
        [BoxGroup("Setup", true, true, 0), SerializeField, ChildGameObjectsOnly, Required]
        private TextMeshProUGUI _text;
        [BoxGroup("Setup", true, true, 0), SerializeField] private J_SO_LocalizationEntry _entry;

        // --------------- EVENTS --------------- //
        private void UpdateText(J_St_Localization localization)
        {
            int currentLanguage = localization.CurrentLanguageId;

            string localizedText = _entry.GetTextOrDefault(currentLanguage);
            _text.text = localizedText;
        }

        private void OnEnable()
        {
            if (_entry == default)
            {
                JLog.Warning($"{gameObject.name} searching {nameof(_entry)} for {_text.text}", JLogTags.Localization, this);
                TryCatchEntry();
            }

            if (_entry == default)
            {
                JLog.Error($"{name} has no {nameof(_entry)} set for {_text.text}", JLogTags.Localization, this);
                return;
            }

            UpdateText(J_St_Localization.GetInstanceSafe());
            J_St_Localization.GetInstanceSafe().Subscribe(UpdateText);
        }

        private void OnDisable() { J_St_Localization.GetInstanceSafe().Unsubscribe(UpdateText); }

        [Button]
        private void TryCatchEntry()
        {
            if (_entry   != default ||
                _library == default ||
                _text    == default) { return; }

            _entry = _library.TryCatch(_text.text);
        }

        // --------------- UNITY EDITOR --------------- //
#if UNITY_EDITOR
        [Button]
        private void GenerateNewEntry()
        {
            if (string.IsNullOrEmpty(_text.text))
            {
                JLog.Error($"{_text.text} empty key", JLogTags.Localization, this);
                return;
            }

            if (_entry != null)
            {
                JLog.Error($"{nameof(_entry)} already exists", JLogTags.Localization, this);
                return;
            }

            _entry = _library.AddEntry(_text.text);
        }

        [Button] private void UpdateEntry() { _library.UpdateEntry(_entry, _text.text); }

        private void OnValidate()
        {
            if (_text == default) { _text = GetComponent<TextMeshProUGUI>(); }

            if (_text == default) { return; }

            string text = _text.text;
            if (string.IsNullOrEmpty(text) &&
                _entry != default)
            {
                _text.text = _entry.Key;
                return;
            }

            if (text.Length < 10) { return; }

            TryCatchEntry();
        }
#endif
    }
}
#endif
