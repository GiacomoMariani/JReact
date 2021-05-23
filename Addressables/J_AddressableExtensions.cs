#if UNITY_ADDRESSABLES
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

#if UNITY_UNITASK
using Cysharp.Threading.Tasks;
#endif

namespace JReact.J_Addressables
{
    public static class J_AddressableExtensions
    {
        // --------------- SPRITE RENDERER --------------- //
#if UNITY_UNITASK
        public static async UniTask<SpriteRenderer> ToSpriteRenderer(this AssetReferenceAtlasedSprite reference, SpriteRenderer renderer)
#else
        public static async Task<SpriteRenderer> ToSpriteRenderer(this AssetReferenceAtlasedSprite reference, SpriteRenderer renderer)
#endif
        {
            var    current = reference.LoadAssetAsync<Sprite>();
            Sprite sprite  = await current.Task;
            renderer.sprite = sprite;
            return renderer;
        }

#if UNITY_UNITASK
        public static async Task<Image> ToImage(this AssetReferenceAtlasedSprite reference, Image image)
#else
        public static async Task<Image> ToImage(this AssetReferenceAtlasedSprite reference, Image image)
#endif
        {
            var    current = reference.LoadAssetAsync<Sprite>();
            Sprite sprite  = await current.Task;
            image.sprite = sprite;

            return image;
        }
    }
}
#endif
