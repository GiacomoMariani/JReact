#if UNITY_ADDRESSABLES
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
#if UNITY_UNITASK
using Cysharp.Threading.Tasks;

#else
using System.Threading.Tasks;
#endif

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

#if UNITY_UNITASK
        private async UniTask Load(AssetReferenceAtlasedSprite spriteReference)
#else
        private async Task Load(AssetReferenceAtlasedSprite spriteReference)
#endif

        {
            _image.enabled = false;
            await spriteReference.ToImage(_image);
            _image.enabled = true;
        }

#if UNITY_UNITASK
        public async UniTask AssignNewReference(AssetReferenceAtlasedSprite reference)
#else
        public async Task AssignNewReference(AssetReferenceAtlasedSprite reference)
#endif
        {
            if (_spriteReference != null) { Unload(_spriteReference); }

            _spriteReference = reference;
            if (_spriteReference != null) { await Load(_spriteReference); }
        }

        // --------------- UNITY EVENTS --------------- //
        private async void OnEnable()
        {
            if (_spriteReference != null) { await Load(_spriteReference); }
        }

        private void OnDisable()
        {
            if (_spriteReference != null) { Unload(_spriteReference); }
        }
    }
}
#endif
