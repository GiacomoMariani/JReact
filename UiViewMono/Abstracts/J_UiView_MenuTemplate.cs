using UnityEngine;

namespace JReact.UiView
{
    /// <summary>
    /// the basic template for a menu
    /// </summary>
    public abstract class J_UiView_MenuTemplate : MonoBehaviour
    {
        protected virtual void Awake() { InitThisMenu(); }

        /// initialization logic
        public abstract void InitThisMenu();

        /// open menu logic
        public abstract void OpenThisMenu();

        /// close menu logic
        public abstract void CloseThisMenu();
    }
}
