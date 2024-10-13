using JReact.Singleton;
using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.Advertisting
{
    public sealed class JBannerOverlay : J_MonoSingleton<JBannerOverlay>
    {
        [BoxGroup("Setup", true, true, 0), SerializeField, ChildGameObjectsOnly, Required]
        private GameObject _panel;

        public void Show() { _panel.SetActive(true); }

        public void Hide() { _panel?.SetActive(false); }
    }
}
