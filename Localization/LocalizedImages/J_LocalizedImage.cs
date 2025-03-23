using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JReact.Localization.LocalizedImages
{
    public sealed class J_LocalizedImage : MonoBehaviour
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField, ChildGameObjectsOnly, Required]
        private Image _image;
        [BoxGroup("Setup", true, true, 0), SerializeField] private J_SO_LocalizedImageEntry _localizedImages;

        // --------------- EVENTS --------------- //
        private void UpdateImage(J_St_Localization localization)
        {
            int currentLanguage = localization.CurrentLanguageId;

            Sprite localizedImage = _localizedImages.GetImageOrDefault(currentLanguage);
            _image.sprite = localizedImage;
        }

        private void OnEnable()
        {
            if (_localizedImages == default)
            {
                JLog.Error($"{name} has no {nameof(_localizedImages)} set for {_localizedImages}", JLogTags.Localization, this);
                return;
            }

            UpdateImage(J_St_Localization.GetInstanceSafe());
            J_St_Localization.GetInstanceSafe().Subscribe(UpdateImage);
        }

        private void OnDisable() { J_St_Localization.GetInstanceSafe().Unsubscribe(UpdateImage); }

        // --------------- UNITY EDITOR --------------- //
#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_image == default) { _image = GetComponent<Image>(); }

            if (_image == default) { return; }

            if (_localizedImages == default) { return; }

            _image.sprite = _localizedImages.GetImageOrDefault(J_St_Localization.GetInstanceSafe().CurrentLanguageId);
        }
#endif
    }
}
