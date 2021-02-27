using JReact.UiView;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.StateControl.LevelSystem
{
    /// <summary>
    /// shows the current level on a text
    /// </summary>
    [RequireComponent(typeof(TextMeshProUGUI))]
    public sealed class J_UiView_LevelText : J_UiView_Text
    {
        [BoxGroup("Setup", true, true), SerializeField, Required, AssetsOnly] private J_LevelProgression _levelControl;

        protected override void SanityChecks()
        {
            base.SanityChecks();
            Assert.IsNotNull(_levelControl, $"{name} requires a levelControl");
        }

        private void UpdateText((J_LevelState previous, J_LevelState current) transition) { SetText(transition.current.ToString()); }

        // --------------- UNITY EVENTS --------------- //
        private void OnEnable()
        {
            SetText(_levelControl.CurrentLevelInfo.ToString());
            _levelControl.Subscribe(UpdateText);
        }

        private void OnDisable() { _levelControl.UnSubscribe(UpdateText); }
    }
}
