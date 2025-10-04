using UnityEngine;
using UnityEngine.Assertions;

namespace JReact
{
    public static class J_CameraExtensions
    {
        /// <summary>
        /// converts the mouse position on a given camera into a world position
        /// </summary>
        /// <param name="camera">the world position related to the camera</param>
        public static Vector3 MouseToWorldPosition(this Camera camera) => camera.ScreenToWorldPoint(Input.mousePosition);

        /// <summary>
        /// Clamps the camera's position within specified bottom-left and top-right boundaries in world space.
        /// </summary>
        /// <param name="unityCamera">The camera whose position is being clamped.</param>
        /// <param name="bottomLeft">The bottom-left boundary of the clamping region in world space.</param>
        /// <param name="topRight">The top-right boundary of the clamping region in world space.</param>
        /// <returns>The clamped position of the camera in world space.</returns>
        public static Vector3 ClampCameraPosition(this Camera unityCamera, Vector3 bottomLeft, Vector3 topRight)
        {
            Vector3 cameraPosition = unityCamera.transform.position;

            float verticalSize   = unityCamera.orthographicSize;
            float horizontalSize = verticalSize * unityCamera.aspect;

            float   clampedX = Mathf.Clamp(cameraPosition.x, bottomLeft.x + horizontalSize, topRight.x - horizontalSize);
            float   clampedY = Mathf.Clamp(cameraPosition.y, bottomLeft.y + verticalSize,   topRight.y - verticalSize);
            Vector3 result   = new Vector3(clampedX, clampedY, cameraPosition.z);
            unityCamera.transform.position = new Vector3(result.x, result.y, cameraPosition.z);
            return result;
        }

        /// <summary>
        /// Determines whether the camera's position is within the specified target bounds, factoring in a border offset.
        /// </summary>
        /// <param name="unityCamera">The camera whose position is being checked.</param>
        /// <param name="bottomLeft">The bottom-left boundary of the target region in world space.</param>
        /// <param name="topRight">The top-right boundary of the target region in world space.</param>
        /// <param name="border">An additional tolerance value added to the boundaries for the check.</param>
        /// <returns>True if the camera's position is within the target bounds with the specified border, otherwise false.</returns>
        public static bool IsWithinTarget(this Camera unityCamera, Vector3 bottomLeft, Vector3 topRight, float border)
        {
            Vector3 cameraPosition = unityCamera.transform.position;

            float verticalSize   = unityCamera.orthographicSize;
            float horizontalSize = verticalSize * unityCamera.aspect;

            if (cameraPosition.x < bottomLeft.x + horizontalSize + border) return false;
            if (cameraPosition.x > topRight.x   - horizontalSize - border) return false;
            if (cameraPosition.y < bottomLeft.y + verticalSize   + border) return false;
            if (cameraPosition.y > topRight.y   - verticalSize   - border) return false;

            return true;
        }

        /// <summary>
        /// Clamps the orthographic size (zoom level) of a camera to ensure it fits within specified bottom-left and top-right boundaries in world space.
        /// </summary>
        /// <param name="thisCamera">The orthographic camera whose zoom level is being clamped.</param>
        /// <param name="bottomLeft">The bottom-left boundary of the clamping region in world space.</param>
        /// <param name="topRight">The top-right boundary of the clamping region in world space.</param>
        /// <returns>The clamped orthographic size of the camera.</returns>
        public static float ClampZoom(this Camera thisCamera, Vector2 bottomLeft, Vector2 topRight)
        {
            Assert.IsTrue(thisCamera.orthographic, $"This is intended for orthographic cameras.");

            // --------------- VERTICAL --------------- //
            float verticalOrthographicSize = thisCamera.orthographicSize;
            float verticalLength           = (topRight.y - bottomLeft.y) * 0.5f;

            if (verticalOrthographicSize > verticalLength) { verticalOrthographicSize = verticalLength; }

            // --------------- HORIZONTAL --------------- //
            float horizontalSize   = verticalOrthographicSize    * thisCamera.aspect;
            float horizontalLength = (topRight.x - bottomLeft.x) * 0.5f;

            if (horizontalSize > horizontalLength) { verticalOrthographicSize = horizontalLength / thisCamera.aspect; }

            thisCamera.orthographicSize = verticalOrthographicSize;
            return verticalOrthographicSize;
        }
        
        private static readonly Vector2[] _cameraCache = new Vector2[4];
        
        public static Vector2[] GetCameraBoundaries(this Camera unityCamera)
        {
            Vector3 cameraPosition = unityCamera.transform.position;
            float   verticalSize   = unityCamera.orthographicSize;
            float   horizontalSize = verticalSize * unityCamera.aspect;
            _cameraCache[0] = new Vector2(cameraPosition.x - horizontalSize, cameraPosition.y - verticalSize); // Bottom-left
            _cameraCache[1] = new Vector2(cameraPosition.x - horizontalSize, cameraPosition.y + verticalSize); // Top-left
            _cameraCache[2] = new Vector2(cameraPosition.x + horizontalSize, cameraPosition.y + verticalSize); // Top-right
            _cameraCache[3] = new Vector2(cameraPosition.x + horizontalSize, cameraPosition.y - verticalSize); // Bottom-right
            return _cameraCache;
        }
    }
}
