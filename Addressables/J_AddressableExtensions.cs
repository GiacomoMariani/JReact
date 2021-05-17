using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Assertions;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace JReact.J_Addressables
{
    public static class J_AddressableExtensions
    {
        private const int ExpectedAmount = 50;
        private static Dictionary<SpriteRenderer, AsyncOperationHandle<Sprite>> _loaded =
            new Dictionary<SpriteRenderer, AsyncOperationHandle<Sprite>>(ExpectedAmount);

        // --------------- SPRITE RENDERER --------------- //
        public static async Task<SpriteRenderer> FromAddressable(this SpriteRenderer renderer, AssetReferenceAtlasedSprite reference)
        {
            if (_loaded.ContainsKey(renderer)) { Addressables.Release(_loaded[renderer]); }

            var    current = _loaded[renderer] = reference.LoadAssetAsync<Sprite>();
            Sprite sprite  = await current.Task;
            renderer.sprite = sprite;

            return renderer;
        }

        public static void UnloadAddressable(this SpriteRenderer renderer)
        {
            Assert.IsTrue(_loaded.ContainsKey(renderer), $"Renderer {renderer.name} was not loaded");
            Addressables.Release(_loaded[renderer]);
            _loaded.Remove(renderer);
        }

        // --------------- IMAGES --------------- //
        private static Dictionary<Image, AsyncOperationHandle<Sprite>> _uiLoaded =
            new Dictionary<Image, AsyncOperationHandle<Sprite>>(ExpectedAmount);

        public static async Task<Image> FromAddressable(this Image image, AssetReferenceAtlasedSprite reference)
        {
            if (_uiLoaded.ContainsKey(image)) { Addressables.Release(_uiLoaded[image]); }

            var    current = _uiLoaded[image] = reference.LoadAssetAsync<Sprite>();
            Sprite sprite  = await current.Task;
            image.sprite = sprite;

            return image;
        }

        public static void UnloadAddressable(this Image image)
        {
            Assert.IsTrue(_uiLoaded.ContainsKey(image), $"Image {image.name} was not loaded");
            Addressables.Release(_uiLoaded[image]);
            _uiLoaded.Remove(image);
        }
    }
}
