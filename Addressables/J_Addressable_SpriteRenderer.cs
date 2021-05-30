#if UNITY_ADDRESSABLES
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace JReact.J_Addressables
{
    public class J_Addressable_SpriteRenderer : MonoBehaviour
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField, ChildGameObjectsOnly, Required]
        private SpriteRenderer _renderer;
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private AssetReferenceAtlasedSprite _spriteReference;

        // --------------- COMMANDS --------------- //
        private void Unload(AssetReferenceAtlasedSprite spriteReference) { spriteReference.ReleaseAsset(); }

        private async UniTask Load(AssetReferenceAtlasedSprite spriteReference)
        {
            _renderer.enabled = false;
            await spriteReference.ToSpriteRenderer(_renderer);
            _renderer.enabled = true;
        }

        public async UniTask AssignNewReference(AssetReferenceAtlasedSprite reference)
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
