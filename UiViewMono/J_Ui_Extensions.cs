using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace JReact
{
    public static class J_Ui_Extensions
    {
        // --------------- RECT TRANSFORM --------------- //
        /// <summary>
        /// make this transform as large as the parent
        /// </summary>
        public static RectTransform FitParent(this RectTransform rectTransform)
        {
            Assert.IsTrue(rectTransform.GetComponentInParent<RectTransform>(),
                          $"{rectTransform.name} parent ({rectTransform.parent.name}) is not a valid");

            rectTransform.anchorMin = JConstants.VectorZero;
            rectTransform.anchorMax = JConstants.VectorOne;
            rectTransform.offsetMin = JConstants.VectorZero;
            rectTransform.offsetMax = JConstants.VectorOne;
            return rectTransform;
        }

        /// <summary>
        /// returns the screen position of the given rect
        /// </summary>
        public static Vector2 ToScreenPosition(this RectTransform rectTransform, Camera camera)
            => RectTransformUtility.WorldToScreenPoint(camera, rectTransform.transform.position);

        /// <summary>
        /// gets the size of a rect transform
        /// </summary>
        public static Vector2 GetSize(this RectTransform rt) => rt.rect.size;

        /// <summary>
        /// gets the width of a rect transform
        /// </summary>
        public static float GetWidth(this RectTransform rt) => rt.rect.width;

        /// <summary>
        /// gets the height of a rect transform
        /// </summary>
        public static float GetHeight(this RectTransform rt) => rt.rect.height;

        /// <summary>
        /// sets the size of a rect transform
        /// </summary>
        public static void SetSize(this RectTransform rt, Vector2 size)
        {
            Vector2 oldSize   = rt.rect.size;
            Vector2 deltaSize = size - oldSize;
            Vector2 pivot     = rt.pivot;
            rt.offsetMin -= new Vector2(deltaSize.x * pivot.x,        deltaSize.y * pivot.y);
            rt.offsetMax += new Vector2(deltaSize.x * (1f - pivot.x), deltaSize.y * (1f - pivot.y));
        }

        /// <summary>
        /// sets the width of a rect transform
        /// </summary>
        public static void SetWidth(this RectTransform rt, float width) => SetSize(rt, new Vector2(width, rt.rect.size.y));

        /// <summary>
        /// sets the height of a rect transform
        /// </summary>
        public static void SetHeight(this RectTransform rt, float height) => SetSize(rt, new Vector2(rt.rect.size.x, height));

        // --------------- IMAGE --------------- //
        /// <summary>
        /// used to set a transparency on a given image
        /// </summary>
        /// <param name="image">the image to adjust</param>
        /// <param name="transparency">the transparency we want to set</param>
        public static Image SetTransparency(this Image image, float transparency)
        {
            Assert.IsTrue(transparency >= 0f && transparency <= 1.0f,
                          $"The transparency to be set on {image.gameObject.name} should be between 0 and 1. Received value: {transparency}");

            transparency = Mathf.Clamp(transparency, 0f, 1f);
            Color fullColor = image.color;
            image.color = new Color(fullColor.r, fullColor.g, fullColor.b, transparency);
            return image;
        }

        // --------------- CANVAS GROUP --------------- //
        /// <summary>
        /// used to quickly activate and deactivate a canvas group
        /// </summary>
        /// <param name="activate">true if we want to activate</param>
        /// <returns>the same canvas groups is returned</returns>
        public static CanvasGroup Activate(this CanvasGroup canvasGroup, bool activate)
        {
            canvasGroup.alpha = activate ? 1f : 0f;

            canvasGroup.interactable = canvasGroup.blocksRaycasts = activate;
            return canvasGroup;
        }

        /// <summary>
        /// used to quickly setup a canvas group
        /// </summary>
        /// <returns>the same canvas groups is returned</returns>
        public static CanvasGroup SetCanvas(this CanvasGroup canvasGroup, float alpha, bool interactable, bool blockRaycast)
        {
            canvasGroup.alpha = alpha;

            canvasGroup.interactable   = interactable;
            canvasGroup.blocksRaycasts = blockRaycast;
            return canvasGroup;
        }
    }
}
