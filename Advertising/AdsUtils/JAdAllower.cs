using System;
using UnityEngine;

namespace JReact.Advertisting
{
    public class JAdAllower : IAllower
    {
        private Func<bool> _isAdsAllowed;
        private const string _AdsEnabledPlayerPrefString = "IsAdsEnabled_J";

        internal bool AdsEnabled
        {
            get => PlayerPrefs.GetInt(_AdsEnabledPlayerPrefString, 1) == 1;
            private set => PlayerPrefs.SetInt(_AdsEnabledPlayerPrefString, value ? 1 : 0);
        }

        public void AddAdsAllowChecker(Func<bool> act)
        {
            if (_isAdsAllowed == null) { _isAdsAllowed =  act; }
            else { _isAdsAllowed                       += act; }
        }

        public bool IsAllowed()
        {
            if (!AdsEnabled) { return false; }

            bool isAllow = true;
            if (_isAdsAllowed != null) { isAllow = _isAdsAllowed(); }

            return isAllow;
        }

        internal void DisableAds() { AdsEnabled = false; }
    }

    public interface IAllower
    {
        bool IsAllowed();
    }
}
