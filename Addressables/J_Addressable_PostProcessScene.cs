#if UNITY_EDITOR && UNITY_ADDRESSABLES
using UnityEditor.Callbacks;
using UnityEngine;

namespace JReact.J_Addressables
{
    public static class J_Addressable_PostProcessScene
    {
        [PostProcessScene]
        private static void RemoveAddressableSprites()
        {
            var images = Object.FindObjectsByType<J_Addressable_Image>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            for (int i = 0; i < images.Length; i++)
            {
                if (images[i].Image != null) { images[i].Image.sprite = null; }
            }

            var sprites =
                Object.FindObjectsByType<J_Addressable_SpriteRenderer>(FindObjectsInactive.Include, FindObjectsSortMode.None);

            for (int i = 0; i < sprites.Length; i++)
            {
                if (sprites[i].Renderer != null) { sprites[i].Renderer.sprite = null; }
            }
        }
    }
}
#endif
