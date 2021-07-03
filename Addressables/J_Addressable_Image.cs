#if UNITY_ADDRESSABLES
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace JReact.J_Addressables
{
    public class J_Addressable_Image : MonoBehaviour
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField, ChildGameObjectsOnly, Required]
        private Image _image;
        internal Image Image => _image;

        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private AssetReferenceAtlasedSprite _spriteReference;

        // --------------- UNITY EVENTS --------------- //
        private void OnEnable()
        {
            Assert.IsNotNull(_spriteReference, $"{gameObject.name} requires a {nameof(_spriteReference)}");
            _image.sprite = _spriteReference.LoadAssetAsync<Sprite>().WaitForCompletion();
        }

        private void OnDisable()
        {
            Assert.IsNotNull(_spriteReference, $"{gameObject.name} requires a {nameof(_spriteReference)}");
            _spriteReference.ReleaseAsset();
        }

#if UNITY_EDITOR
        // --------------- VALIDATION FOR EDITOR --------------- //
        private void OnValidate()
        {
            if (_spriteReference != null &&
                _image           != null) { _image.sprite = _spriteReference.editorAsset.GetSprite(_spriteReference.SubObjectName); }
        }
#endif
    }
}
#endif
