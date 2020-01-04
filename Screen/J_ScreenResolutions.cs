using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.JScreen
{
    [CreateAssetMenu(menuName = "Reactive/Screen/Resolutions", fileName = "J_Resolutions", order = 0)]
    public sealed class J_ScreenResolutions : ScriptableObject
    {
        // --------------- EVENTS --------------- //
        private Action<(int index, Resolution current)> OnResolutionChange;
        private Action<bool> OnFullScreenChannge;

        // --------------- FIELDS AND PROPERTIES --------------- //
        [BoxGroup("Setup", true, true), SerializeField, Required] private string _prefResolution = "J_ScreenResolution";
        [BoxGroup("Setup", true, true), SerializeField, Required] private string _prefWindowed = "J_FullScreenActive";
        [BoxGroup("Setup", true, true), SerializeField, Required] private float _minWidth = 1000f;
        [BoxGroup("Setup", true, true), SerializeField, Required] private float _minHeight = 500f;

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public Resolution[] UnityAllResolutions => Screen.resolutions;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public Resolution UnityCurrentResolution
            => Screen.currentResolution;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public bool UnityFullScreen => Screen.fullScreen;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private List<string> _resolutionAsString;

        // --------------- QUERIES - FULLSCREEN --------------- //
        public bool IsFullScreen() => GetFullScreen() == 0;

        private int GetFullScreen() => PlayerPrefs.HasKey(_prefWindowed)
                                           ? PlayerPrefs.GetInt(_prefWindowed)
                                           : StartWithFullScreen();

        private int StartWithFullScreen()
        {
            PlayerPrefs.SetInt(_prefWindowed, 0);
            return 0;
        }

        // --------------- COMMANDS - FULLSCREEN --------------- //
        public void SetFullScreen(bool isEnabled)
        {
            if (Screen.fullScreen == isEnabled) return;
            Screen.fullScreen = isEnabled;
            PlayerPrefs.SetInt(_prefWindowed, isEnabled
                                                  ? 0
                                                  : 1);

            OnFullScreenChannge?.Invoke(isEnabled);
        }

        // --------------- QUERIES - RESOLUTION --------------- //
        public int GetResolutionIndex() => PlayerPrefs.HasKey(_prefResolution)
                                               ? PlayerPrefs.GetInt(_prefResolution)
                                               : CalculateFirstResolution();

        private int CalculateFirstResolution()
        {
            var index = IndexFromResolution(Screen.currentResolution);
            PlayerPrefs.SetInt(_prefResolution, index);
            return index;
        }

        private int IndexFromResolution(Resolution res)
        {
            int resolutionsLength = Screen.resolutions.Length;
            for (int i = resolutionsLength - 1; i >= 0; i--)
                if (AreEqual(Screen.resolutions[i], res))
                    return i;

            JLog.Break($"{name} cannot calculate resolution. Total Resolutions = {resolutionsLength}. Current = {Screen.currentResolution}");
            return -1;
        }

        public List<string> GetResolutionAsString()
        {
            if (_resolutionAsString == null) PopulateStrings();
            return _resolutionAsString;
        }

        private void PopulateStrings()
        {
            int resolutionsLength = Screen.resolutions.Length;
            _resolutionAsString = new List<string>();
            for (int i = resolutionsLength - 1; i >= 0; i--)
            {
                Resolution  nextResolution     = Screen.resolutions[i];
                Resolution? previousResolution = new Resolution();

                //to avoid duplicate resolutions we get the one previous one. They are in order, with the higher refresh rate as late
                if (_resolutionAsString.Count == 0) previousResolution = null;
                else previousResolution                                = Screen.resolutions[i + 1];

                if (IsValid(nextResolution, previousResolution)) _resolutionAsString.Add(ConvertToString(nextResolution));
            }
        }

        private bool IsValid(Resolution resolution, Resolution? previousResolution)
        {
            if (resolution.height < _minHeight) return false;
            if (resolution.width  < _minWidth) return false;
            if (!previousResolution.HasValue) return false;
            //avoids duplicate resolutions
            if (AreEqual(resolution, previousResolution.Value)) return false;
            return true;
        }

        // --------------- COMMANDS - RESOLUTION --------------- //
        public void SetResolution(Resolution res) => SetResolution(IndexFromResolution(res));

        public void SetResolution(int value)
        {
            var res = Screen.resolutions[value];
            //no changes if the resolutions were equal
            if (AreEqual(res, Screen.currentResolution)) return;
            Screen.SetResolution(res.width, res.height, Screen.fullScreen);
            PlayerPrefs.SetInt(_prefResolution, value);
            OnResolutionChange?.Invoke((value, res));
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
