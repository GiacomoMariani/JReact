#if UNITY_ADDRESSABLES
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Assertions;

namespace JReact.J_Addressables
{
    public class J_Addressable_SpriteRenderer : MonoBehaviour
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField, ChildGameObjectsOnly, Required]
        private SpriteRenderer _renderer;
        internal SpriteRenderer Renderer => _renderer;
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private AssetReferenceAtlasedSprite _spriteReference;

        // --------------- UNITY EVENTS --------------- //
        private void OnEnable()
        {
            Assert.IsNotNull(_spriteReference, $"{gameObject.name} requires a {nameof(_spriteReference)}");
            _renderer.sprite = _spriteReference.LoadAssetAsync<Sprite>().WaitForCompletion();
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
                _renderer != null) { _renderer.sprite = _spriteReference.editorAsset.GetSprite(_spriteReference.SubObjectName); }
        }
#endif
    }
}
#endif
