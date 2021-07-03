#if UNITY_EDITOR
using UnityEditor.Callbacks;
using UnityEngine;

namespace JReact.J_Addressables
{
    public static class J_Addressable_PostProcessScene
    {
        [PostProcessScene]
        private static void RemoveAddressableSprites()
        {
            var images = Object.FindObjectsOfType<J_Addressable_Image>(true);
            for (int i = 0; i < images.Length; i++)
            {
                if (images[i].Image != null) { images[i].Image.sprite = null; }
            }

            var sprites = Object.FindObjectsOfType<J_Addressable_SpriteRenderer>(true);
            for (int i = 0; i < sprites.Length; i++)
            {
                if (sprites[i].Renderer != null) { sprites[i].Renderer.sprite = null; }
            }
        }
    }
}
#endif
