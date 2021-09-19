using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
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

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private List<(int width, int height)> _validResolutions;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private List<string> _stringList;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public Resolution[] UnityAllResolutions => Screen.resolutions;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public Resolution Current => Screen.currentResolution;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public bool UnityFullScreen => Screen.fullScreen;

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
        public List<(int width, int height)> GetResolutions()
        {
            if (_validResolutions == null) PopulateStrings();
            return _validResolutions;
        }

        public List<string> GetResolutionsAsString()
        {
            if (_stringList == null) PopulateStrings();
            return _stringList;
        }

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
            if (resolution.height < _minHeight) return false;
            if (resolution.width  < _minWidth) return false;
            if (!previousResolution.HasValue) return true;
            //avoids duplicate resolutions
            if (AreEqual(resolution, previousResolution.Value)) return false;
            return true;
        }

        // --------------- COMMANDS - RESOLUTION --------------- //
        public void SetResolution(Resolution res) => SetResolution(IndexFromResolution(res));

        public void SetResolution(int value)
        {
            var res = _validResolutions[value];
            //no changes if the resolutions were equal
            if (res.height == Current.height &&
                res.width  == Current.width) return;

            var mode = Screen.fullScreenMode;
            Screen.SetResolution(res.width, res.height, FullScreenMode.ExclusiveFullScreen);
            Screen.SetResolution(res.width, res.height, mode);
            J_ScreenControls.Main.SetSize(res.width, res.height);
            PlayerPrefs.SetInt(_prefResolution, value);
            OnResolutionChange?.Invoke((value, Current));
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
