#if UNITY_ADDRESSABLES
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace JReact.J_Addressables
{
    public class J_SpriteRenderer_Addressable : MonoBehaviour
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField, ChildGameObjectsOnly, Required]
        private SpriteRenderer _renderer;
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private AssetReferenceAtlasedSprite _spriteReference;

        // --------------- INITIALIZATION --------------- //
        private async void OnEnable()
        {
            _renderer.enabled = false;
            await _renderer.FromAddressable(_spriteReference);
            _renderer.enabled = true;
        }

        private void OnDisable() { _renderer.UnloadAddressable(); }
    }
}
#endif
