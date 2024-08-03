using System.Collections.Generic;
using UnityEngine;

namespace JReact.JuiceMenuComposer
{
    /// <summary>
    /// Represents an item in the JUI_Screen.
    /// </summary>
    public abstract class JUI_Item : MonoBehaviour
    {
        // --------------- INIT/DEINIT --------------- //
        /// <summary>
        /// Executed when the JUI_Item is initialized.
        /// </summary>
        /// <param name="parentScreen">The parent JUI_Screen.</param>
        public virtual void OnInit(JUI_Screen     parentScreen) {}
        
        // --------------- SHOW METHODS --------------- //
        /// <summary>
        /// Executes before showing the JUI_Screen.
        /// </summary>
        /// <param name="parentScreen">The parent JUI_Screen.</param>
        /// <returns>A coroutine that can be executed.</returns>
        public virtual IEnumerator<float> OnBeforeShow(JUI_Screen   parentScreen) { yield break; }

        /// <summary>
        /// Executes after the JUI_Screen finishes hiding.
        /// </summary>
        /// <param name="parentScreen">The parent JUI_Screen.</param>
        public virtual void               OnStopShowing(JUI_Screen     parentScreen) {}

        /// <summary>
        /// Executes after the JUI_Screen finishes showing.
        /// </summary>
        /// <param name="parentScreen">The parent JUI_Screen.</param>
        public virtual void               OnCompleteShow(JUI_Screen parentScreen) {}

        // --------------- HIDE METHODS --------------- //
        /// <summary>
        /// Executes before hiding the JUI_Screen.
        /// </summary>
        /// <param name="parentScreen">The parent JUI_Screen.</param>
        /// <returns>A coroutine that can be executed.</returns>
        public virtual IEnumerator<float> OnBeforeHide(JUI_Screen parentScreen) { yield break; }

        /// <summary>
        /// Executes after the JUI_Screen finishes hiding.
        /// </summary>
        /// <param name="parentScreen">The parent JUI_Screen.</param>
        public virtual void OnStopHiding(JUI_Screen parentScreen) {}

        /// <summary>
        /// Executes after the JUI_Screen finishes hiding.
        /// </summary>
        /// <param name="parentScreen">The parent JUI_Screen.</param>
        public virtual void OnCompleteHide(JUI_Screen parentScreen) {}
    }
}
