using Unity.Mathematics;
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
        /// place the transform in the center
        /// </summary>
        public static RectTransform SetCentered(this RectTransform rectTransform)
        {
            rectTransform.pivot     = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            return rectTransform;
        }

        /// <summary>
        /// place the transform on the left
        /// </summary>
        public static RectTransform SetLeft(this RectTransform rectTransform, bool placeInside = true)
        {
            var pivot = new Vector2(0f, rectTransform.pivot.y);
            if (!placeInside) { pivot.x = 1f; }

            rectTransform.pivot     = pivot;
            rectTransform.anchorMax = new Vector2(0f, rectTransform.anchorMax.y);
            rectTransform.anchorMin = new Vector2(0f, rectTransform.anchorMin.y);
            return rectTransform;
        }

        /// <summary>
        /// place the transform on the right
        /// </summary>
        public static RectTransform SetRight(this RectTransform rectTransform, bool placeInside = true)
        {
            var pivot = new Vector2(1f, rectTransform.pivot.y);
            if (!placeInside) { pivot.x = 0f; }

            rectTransform.pivot     = pivot;
            rectTransform.anchorMax = new Vector2(1f, rectTransform.anchorMax.y);
            rectTransform.anchorMin = new Vector2(1f, rectTransform.anchorMin.y);
            return rectTransform;
        }

        /// <summary>
        /// place the transform up
        /// </summary>
        public static RectTransform SetUp(this RectTransform rectTransform, bool placeInside = true)
        {
            var pivot = new Vector2(rectTransform.pivot.x, 1f);
            if (!placeInside) { pivot.y = 0f; }

            rectTransform.pivot     = pivot;
            rectTransform.anchorMax = new Vector2(rectTransform.anchorMax.x, 1f);
            rectTransform.anchorMin = new Vector2(rectTransform.anchorMin.x, 1f);
            return rectTransform;
        }

        /// <summary>
        /// place the transform down
        /// </summary>
        public static RectTransform SetDown(this RectTransform rectTransform, bool placeInside = true)
        {
            var pivot = new Vector2(rectTransform.pivot.x, 0f);
            if (!placeInside) { pivot.y = 1f; }

            rectTransform.pivot     = pivot;
            rectTransform.anchorMax = new Vector2(rectTransform.anchorMax.x, 0f);
            rectTransform.anchorMin = new Vector2(rectTransform.anchorMin.x, 0f);
            return rectTransform;
        }

        /// <summary>
        /// place the transform on a given position, making sure it's inside the screen
        /// </summary>
        public static RectTransform PlaceAt(this RectTransform rectTransform, Vector2 screenPosition)
        {
            rectTransform.anchorMax = JConstants.VectorZero;
            rectTransform.anchorMin = JConstants.VectorZero;
            float pivotX = screenPosition.x / Screen.width;
            float pivotY = screenPosition.y / Screen.height;
            rectTransform.pivot            = new Vector2(pivotX, pivotY);
            rectTransform.anchoredPosition = screenPosition;

            return rectTransform;
        }

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

        /// <summary>
        /// sets the rect transform at the top of its hierarchy
        /// </summary>
        public static void TopHierarchy(this RectTransform rt) => rt.SetAsLastSibling();

        /// <summary>
        /// sets the rect transform at the bottom of its hierarchy
        /// </summary>
        public static void BottomHierarchy(this RectTransform rt) => rt.SetAsFirstSibling();

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

        /// <summary>
        /// a logger to prints the data of the rect transform on the console
        /// </summary>
        /// <param name="rectTransform"></param>
        public static string PrintData(this RectTransform rectTransform)
            => $"{rectTransform.name} - pivot {rectTransform.pivot}, position {rectTransform.anchoredPosition}, " +
               $"offset {rectTransform.offsetMin} - {rectTransform.offsetMax}";

        // --------------- VERTEX HELPER --------------- //
        /// <summary>
        /// draws a box withing a given rect
        /// </summary>
        /// <param name="vh">the vertex helper we're using</param>
        /// <param name="rect">the rect where to draw the border</param>
        /// <param name="size">the size of the border of the box</param>
        /// <param name="paddings">paddings in all directions, from left, anticlockwise. Left(x), bottom(y), right(z), top(w)(</param>
        /// <param name="color">the color we want to these borders</param>
        /// <returns>returns the same vertex helper</returns>
        public static VertexHelper DrawBordersOnRect(this VertexHelper vh, Rect rect, float size, float4 paddings, Color color,
                                                     bool              drawsLeft  = true, bool drawsTop = true,
                                                     bool              drawsRight = true, bool drawsBottom = true)
        {
            float rectWidth  = rect.width;
            float rectHeight = rect.height;

            var bottomLeft = new Vector2(paddings.x,             paddings.y);
            var topRight   = new Vector2(rectWidth - paddings.z, rectHeight - paddings.w);

            vh.DrawUiBox(bottomLeft, topRight, size, color, true, drawsLeft, drawsTop,
                         drawsRight, drawsBottom);

            return vh;
        }

        /// <summary>
        /// draws a box withing a given rect using horizontal and vertical padding
        /// </summary>
        public static VertexHelper DrawBordersOnRect(this VertexHelper vh, Rect rect, float size, float2 paddings, Color color,
                                                     bool drawsLeft = true, bool drawsTop = true,
                                                     bool drawsRight = true, bool drawsBottom = true)
            => DrawBordersOnRect(vh,       rect, size, new float4(paddings.x, paddings.y, paddings.x, paddings.y), color, drawsLeft,
                                 drawsTop, drawsRight, drawsBottom);

        /// <summary>
        /// draws a box withing a given rect using the same padding for all direction
        /// </summary>
        public static VertexHelper DrawBordersOnRect(this VertexHelper vh, Rect rect, float size, float padding, Color color,
                                                     bool              drawsLeft  = true, bool drawsTop = true,
                                                     bool              drawsRight = true, bool drawsBottom = true)
            => DrawBordersOnRect(vh,         rect, size, new float4(padding, padding, padding, padding), color, drawsLeft, drawsTop,
                                 drawsRight, drawsBottom);

        /// <summary>
        /// draws a box with a size using specific meshes
        /// </summary>
        /// <param name="vertexHelper">the vertex helper we're using</param>
        /// <param name="bottomLeft">the bottom left  point of the box</param>
        /// <param name="topRight">the top right  point of the box</param>
        /// <param name="size">the size of the box</param>
        /// <param name="color">the color we want to add to the box</param>
        /// <param name="clear">if we want to clear the vh before drawing</param>
        /// <param name="drawsLeft">if  we want to draw the left edge</param>
        /// <param name="drawsTop">if  we want to draw the top  edge</param>
        /// <param name="drawsRight">if  we want to draw the right edge</param>
        /// <param name="drawsBottom">if  we want to draw the bottom edge</param>
        /// <returns>returns the same vertex helper</returns>
        public static VertexHelper DrawUiBox(this VertexHelper vertexHelper, Vector2 bottomLeft, Vector2 topRight, float size,
                                             Color             color, bool clear = true, bool drawsLeft = true, bool drawsTop = true,
                                             bool              drawsRight = true, bool drawsBottom = true)
        {
            Assert.IsTrue(size > 0, $"Size must be positive {size}");
            if (clear) { vertexHelper.Clear(); }

            UIVertex vertex = UIVertex.simpleVert;
            vertex.color = color;

            // --------------- OUTER VERTEX --------------- //
            //0 - bottom left
            vertexHelper.AddVert(vertex.SetPosition(bottomLeft).SetUV(new Vector2(0, 0)));
            //1 - top left
            vertexHelper.AddVert(vertex.SetPosition(new Vector2(bottomLeft.x, topRight.y)).SetUV(new Vector2(0, 1)));
            //2 - top right
            vertexHelper.AddVert(vertex.SetPosition(topRight).SetUV(new Vector2(1, 1)));
            //3 - bottom right
            vertexHelper.AddVert(vertex.SetPosition(new Vector2(topRight.x, bottomLeft.y)).SetUV(new Vector2(1, 0)));
            // --------------- INNER VERTEX --------------- //
            Vector2 innerBottomLeft = new Vector2(bottomLeft.x + size, bottomLeft.y + size);
            Vector2 innerTopRight   = new Vector2(topRight.x   - size, topRight.y   - size);

            //calculations considering the lines to remove
            if (!drawsLeft) { innerBottomLeft.x = bottomLeft.x; }

            if (!drawsTop) { innerTopRight.y = topRight.y; }

            if (!drawsRight) { innerTopRight.x = topRight.x; }

            if (!drawsBottom) { innerBottomLeft.y = bottomLeft.y; }

            float horizontalUvOffset = size / (topRight.x - bottomLeft.x);
            float verticalUvOffset   = size / (topRight.y - bottomLeft.y);

            //4 - inner bottom left
            vertexHelper.AddVert(vertex.SetPosition(innerBottomLeft).SetUV(new Vector2(horizontalUvOffset, verticalUvOffset)));
            //5 - inner top left
            vertexHelper.AddVert(vertex.SetPosition(new Vector2(innerBottomLeft.x, innerTopRight.y))
                                       .SetUV(new Vector2(horizontalUvOffset,      1f - verticalUvOffset)));

            //6 - inner top right
            vertexHelper.AddVert(vertex.SetPosition(innerTopRight)
                                       .SetUV(new Vector2(1f - horizontalUvOffset, 1f - verticalUvOffset)));

            //7 - inner bottom right
            vertexHelper.AddVert(vertex.SetPosition(new Vector2(innerTopRight.x,   innerBottomLeft.y))
                                       .SetUV(new Vector2(1f - horizontalUvOffset, verticalUvOffset)));

            // --------------- TRIANGLES --------------- //
            //top edge
            if (drawsTop)
            {
                vertexHelper.AddTriangle(1, 2, 6);
                vertexHelper.AddTriangle(6, 5, 1);
            }

            //right edge
            if (drawsRight)
            {
                vertexHelper.AddTriangle(2, 3, 7);
                vertexHelper.AddTriangle(7, 6, 2);
            }

            //bottom edge
            if (drawsBottom)
            {
                vertexHelper.AddTriangle(3, 0, 4);
                vertexHelper.AddTriangle(4, 7, 3);
            }

            //left edge
            if (drawsLeft)
            {
                vertexHelper.AddTriangle(0, 1, 5);
                vertexHelper.AddTriangle(5, 4, 0);
            }

            return vertexHelper;
        }

        // --------------- UI VERTEX --------------- //
        public static UIVertex SetPosition(this UIVertex vertex, Vector2 position)
        {
            vertex.position = position;
            return vertex;
        }

        public static UIVertex SetUV(this UIVertex vertex, Vector2 uv)
        {
            vertex.uv0 = uv;
            return vertex;
        }
    }
}
