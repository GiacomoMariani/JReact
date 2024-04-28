using UnityEngine;

namespace JReact
{
    public static class J_CameraExtensions
    {
        /// <summary>
        /// converts the mouse position on a given camera into a world position
        /// </summary>
        /// <param name="camera">the world position related to the camera</param>
        public static Vector3 MouseToWorldPosition(this Camera camera) => camera.ScreenToWorldPoint(Input.mousePosition);
    }
}
