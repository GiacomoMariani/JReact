using UnityEngine;

namespace JReact
{
    public static class J_GraphicsExtensions
    {
        private static MaterialPropertyBlock _PropertyBlock = new MaterialPropertyBlock();

        /// <summary>
        /// changes the color of a renderer material without cloning the material
        /// </summary>
        /// <param name="renderer">the renderer we want to use</param>
        /// <param name="desiredColor">the color to set</param>
        /// <param name="colorProperty">the name of the color material</param>
        /// <returns>returns the same renderer for fluent syntax</returns>
        public static Renderer SetColor(this Renderer renderer, Color desiredColor, string colorProperty = "_color")
        {
            _PropertyBlock.Clear();
            renderer.GetPropertyBlock(_PropertyBlock);
            _PropertyBlock.SetColor(colorProperty, desiredColor);
            renderer.SetPropertyBlock(_PropertyBlock);
            return renderer;
        }

        /// <summary>
        /// Sets a floating-point property on a renderer material without cloning the material.
        /// </summary>
        /// <param name="renderer">The renderer to modify.</param>
        /// <param name="floatValue">The floating-point value to set on the material property.</param>
        /// <param name="floatProperty">The name of the floating-point material property to modify.</param>
        /// <returns>Returns the same renderer for fluent syntax.</returns>
        public static Renderer SetFloatProperty(this Renderer renderer, float floatValue, string floatProperty)
        {
            _PropertyBlock.Clear();
            renderer.GetPropertyBlock(_PropertyBlock);
            _PropertyBlock.SetFloat(floatProperty, floatValue);
            renderer.SetPropertyBlock(_PropertyBlock);
            return renderer;
        }

        /// <summary>
        /// Sets an integer property on a renderer material without cloning the material.
        /// </summary>
        /// <param name="renderer">The renderer to modify.</param>
        /// <param name="intValue">The integer value to set on the material property.</param>
        /// <param name="intProperty">The name of the integer material property to modify.</param>
        /// <returns>Returns the same renderer for fluent syntax.</returns>
        public static Renderer SetIntProperty(this Renderer renderer, int intValue, string intProperty)
        {
            _PropertyBlock.Clear();
            renderer.GetPropertyBlock(_PropertyBlock);
            _PropertyBlock.SetInt(intProperty, intValue);
            renderer.SetPropertyBlock(_PropertyBlock);
            return renderer;
        }

        // --------------- FLUENT STYLE EXTENSIONS --------------- //
        public static Renderer SetPropertyBlock(this Renderer renderer)
        {
            _PropertyBlock.Clear();
            renderer.GetPropertyBlock(_PropertyBlock);
            return renderer;
        }

        public static Renderer WithColor(this Renderer renderer, Color desiredColor, string colorProperty = "_color")
        {
            _PropertyBlock.SetColor(colorProperty, desiredColor);
            return renderer;
        }

        public static void FinalizeProperty(this Renderer renderer) { renderer.SetPropertyBlock(_PropertyBlock); }
    }
}
