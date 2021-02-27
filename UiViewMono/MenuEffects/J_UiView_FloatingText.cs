using DG.Tweening;
using UnityEngine;

namespace JReact.UiView
{
    /// <summary>
    /// a message floating on the screen
    /// </summary>
    public sealed class J_UiView_FloatingText : J_UiView_Text
    {
        /// <summary>
        /// sends a message that floats on the screen
        /// </summary>
        /// <param name="message">the message to show</param>
        /// <param name="color">the color of the message</param>
        /// <param name="direction">direction (and force) of the floating</param>
        /// <param name="timeOfAppearance">time of floating</param>
        /// <param name="easeType">the animation of this element</param>
        public void PublishThisMessage(string message, Color color, Vector2 direction, float timeOfAppearance, Ease easeType)
        {
            //set message and color
            SetText(message);
            SetColor(color);

            //calculating the final position
            Vector2 finalPosition = (Vector2) transform.localPosition + direction;

            //setup the transition
            transform.DOLocalMove(finalPosition, timeOfAppearance).SetEase(easeType);
        }
    }
}
