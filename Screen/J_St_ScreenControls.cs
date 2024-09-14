using System;
using System.Collections.Generic;
using JReact.Singleton;
using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.JScreen
{
    public sealed class J_St_ScreenControls : J_MonoSingleton<J_St_ScreenControls>
    {
        // --------------- EVENTS --------------- //
        private Action<(int index, Resolution current)> OnResolutionChange;
        private Action<bool> OnFullScreenChannge;

        // --------------- FIELDS AND PROPERTIES --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField] private string _prefResolution = "StoredResolutionXJP";
        [BoxGroup("Setup", true, true, 0), SerializeField] private JScreenSize[] _validScreenSizes;
        
        // --------------- QUERIES --------------- //
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private List<(int width, int height)> _validResolutions;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private List<string> _stringList;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public Resolution[] UnityAllResolutions => Screen.resolutions;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public bool UnityFullScreen => Screen.fullScreen;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public float Width => Screen.width;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public float Height => Screen.height;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public RefreshRate RefreshRate
            => Screen.currentResolution.refreshRateRatio;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public bool IsFullScreen => Screen.fullScreen;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private FullScreenMode FullScreenMode => Screen.fullScreenMode;

        protected internal override void InitThis()
        {
            base.InitThis();
            if (!PlayerPrefs.HasKey(_prefResolution)) { CalculateFirstResolution(); }
        }
        
        private int CalculateFirstResolution()
        {
            var index = IndexFromResolution(Screen.currentResolution);
            PlayerPrefs.SetInt(_prefResolution, index);
            return index;
        }

        // --------------- COMMANDS - FULLSCREEN --------------- //
        public void SetFullScreen(bool isEnabled)
        {
            if (Screen.fullScreen == isEnabled) return;
            Screen.fullScreen = isEnabled;
            OnFullScreenChannge?.Invoke(isEnabled);
        }

        // --------------- QUERIES - RESOLUTION --------------- //
        /// <summary>
        /// check if a value is inside the screen
        /// </summary>
        /// <param name="screenPosition">the screen position to check</param>
        /// <returns>true if the position is inside the screen</returns>
        public bool IsInsideScreen(Vector2 screenPosition) => screenPosition.x >= 0     &&
                                                              screenPosition.y >= 0     &&
                                                              screenPosition.x <= Width &&
                                                              screenPosition.y <= Height;
        
        private int IndexFromResolution(Resolution res)
        {
            int resolutionsLength = Screen.resolutions.Length;
            for (int i = resolutionsLength - 1; i >= 0; i--)
                if (AreEqual(Screen.resolutions[i], res))
                    return i;

            JLog.Break($"{name} cannot calculate resolution. Total Resolutions = {resolutionsLength}. Current = {Screen.currentResolution}",
                       JLogTags.Camera, this);

            return -1;
        }

        private void PopulateStrings()
        {
            int resolutionsLength = Screen.resolutions.Length;
            _validResolutions = new List<(int width, int height)>();
            _stringList       = new List<string>();
            Resolution? previousResolution = null;
            for (int i = resolutionsLength - 1; i >= 0; i--)
            {
                Resolution nextResolution = Screen.resolutions[i];
                //to avoid duplicate resolutions we get the one previous one
                if (IsValid(nextResolution, previousResolution))
                {
                    _validResolutions.Add((nextResolution.width, nextResolution.height));
                    _stringList.Add(ConvertToString(nextResolution));
                }

                previousResolution = nextResolution;
            }
        }

        private bool IsValid(Resolution resolution, Resolution? previousResolution)
        {
            // if (resolution.height < _minHeight) { return false; }
            //
            // if (resolution.width < _minWidth) { return false; }

            if (!previousResolution.HasValue) { return true; }

            //avoids duplicate resolutions
            if (AreEqual(resolution, previousResolution.Value)) { return false; }

            return true;
        }

        // --------------- COMMANDS - RESOLUTION --------------- //
        public void SetResolution(Resolution res) => SetResolution(IndexFromResolution(res));

        public void SetResolution(int value)
        {
            // var res = _validResolutions[value];
            // //no changes if the resolutions were equal
            // if (res.height == Current.height &&
            //     res.width  == Current.width) return;
            //
            // var mode = Screen.fullScreenMode;
            // Screen.SetResolution(res.width, res.height, FullScreenMode.ExclusiveFullScreen);
            // // Screen.SetResolution(res.width, res.height, mode);
            // // J_ScreenControls.Main.SetSize(res.width, res.height);
            // PlayerPrefs.SetInt(_prefResolution, value);
            // OnResolutionChange?.Invoke((value, Current));
        }

        // --------------- HELPERS --------------- //
        public bool   AreEqual(Resolution        resA, Resolution resB) => resA.width == resB.width && resA.height == resB.height;
        public string ConvertToString(Resolution res) => $"{res.width} x {res.height}";

        // --------------- SUBSCRIBERS --------------- //
        public void SubscribeToResolution(Action<(int index, Resolution current)>   action) => OnResolutionChange += action;
        public void UnSubscribeToResolution(Action<(int index, Resolution current)> action) => OnResolutionChange -= action;

        public void SubscribeToFullScreen(Action<bool>   action) => OnFullScreenChannge += action;
        public void UnSubscribeToFullScreen(Action<bool> action) => OnFullScreenChannge -= action;
    }
}
