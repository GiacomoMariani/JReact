#if UNITY_ADDRESSABLES
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace JReact.J_Addressables
{
    public class J_Ui_Addressable_Image : MonoBehaviour
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField, ChildGameObjectsOnly, Required]
        private Image _image;
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private AssetReferenceAtlasedSprite _spriteReference;

        // --------------- COMMANDS --------------- //
        private void Unload(AssetReferenceAtlasedSprite spriteReference) { spriteReference.ReleaseAsset(); }

        private async void Load(AssetReferenceAtlasedSprite spriteReference)
        {
            _image.enabled = false;
            await spriteReference.ToImage(_image);
            _image.enabled = true;
        }

        public void AssignNewReference(AssetReferenceAtlasedSprite reference)
        {
            if (_spriteReference != null) { Unload(_spriteReference); }

            _spriteReference = reference;
            if (_spriteReference != null) { Load(_spriteReference); }
        }
        
        // --------------- UNITY EVENTS --------------- //
        private async void OnEnable()
        {
            if (_spriteReference != null) { Load(_spriteReference); }
        }

        private void OnDisable()
        {
            if (_spriteReference != null) { Unload(_spriteReference); }
        }
    }
}
#endif