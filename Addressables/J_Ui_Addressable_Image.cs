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

        // --------------- INITIALIZATION --------------- //
        private async void OnEnable()
        {
            _image.enabled = false;
            await _image.FromAddressable(_spriteReference);
            _image.enabled = true;
        }

        private void OnDisable() { _image.UnloadAddressable(); }
    }
}
#endif
