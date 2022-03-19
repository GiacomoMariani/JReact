using UnityEngine;

namespace JReact.JScreen
{
    public class J_ScreenControls
    {
        public static J_ScreenControls Main = new J_ScreenControls();

        /// <summary>
        /// screen width, initially taken from the screen
        /// </summary>
        private float Width = Screen.width;
        /// <summary>
        /// screen Height, initially taken from the screen
        /// </summary>
        private float Height = Screen.height;

        /// <summary>
        /// check if a value is inside the screen
        /// </summary>
        /// <param name="screenPosition">the screen position to check</param>
        /// <returns>true if the position is inside the screen</returns>
        public bool IsInsideScreen(Vector2 screenPosition) => screenPosition.x >= 0     &&
                                                              screenPosition.y >= 0     &&
                                                              screenPosition.x <= Width &&
                                                              screenPosition.y <= Height;

        /// <summary>
        /// adjust the size of the screen
        /// </summary>
        /// <param name="width">set a width</param>
        /// <param name="height">set a height</param>
        public void SetSize(float width, float height)
        {
            Height = height;
            Width  = width;
        }

        /// <summary>
        /// updates the controls with the current screen sizes
        /// </summary>
        public void UpdateFromScreen()
        {
            Width  = Screen.width;
            Height = Screen.height;
        }
    }
}
