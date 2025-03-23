using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.Localization.LocalizedImages
{
    [CreateAssetMenu(menuName = "Reactive/Localization/Image", fileName = "J_SO_LocalizationImage", order = 0)]
    public sealed class J_SO_LocalizedImageEntry : ScriptableObject
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private Sprite[] _images;
        public Sprite Default => _images?[0];
        
        public Sprite GetImageOrDefault(int currentLanguage) => _images.ValidIndex(currentLanguage) ? _images[currentLanguage] : Default;
        
        [Button]
        private void PrintLanguages() => JLog.Log(J_St_Localization.GetInstanceSafe().LanguageIds(), JLogTags.Localization, this);
    }
}
