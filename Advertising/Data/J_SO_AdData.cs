#if JADVERTISING
using JReact.Advertisting.AdUnit;
using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.Advertisting
{
    [CreateAssetMenu(menuName = "Reactive/Adverising/Data", fileName = "AdvertisingData", order = 0)]
    public sealed class J_SO_AdData : ScriptableObject
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField] private bool _keepLoadingUntilReady = true;

        // --------------- BANNERS --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField] private bool _hasBannerAdUnit;
        [BoxGroup("Setup", true, true, 0), SerializeField] private string _bannerAdUnitAndroid = JAdMobTestData.AndroidBannerAdUnitId;
        [BoxGroup("Setup", true, true, 0), SerializeField] private string _bannerAdUnitIOS = JAdMobTestData.IosBannerAdUnitId;
        public string BannerAdUnit => Application.platform == RuntimePlatform.Android ? _bannerAdUnitAndroid : _bannerAdUnitIOS;

        // --------------- INTERSTITIALS --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField] private bool _hasInterstitialAdUnit;
        [BoxGroup("Setup", true, true, 0), SerializeField]
        private string _interstitialAdUnitAndroid = JAdMobTestData.AndroidInterstitialAdUnitId;
        [BoxGroup("Setup", true, true, 0), SerializeField]
        private string _interstitialAdUnitIOS = JAdMobTestData.IosInterstitialAdUnitId;
        public string InterstitialAdUnit
            => Application.platform == RuntimePlatform.Android ? _interstitialAdUnitAndroid : _interstitialAdUnitIOS;

        // --------------- REWARDED VIDEOS --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField] private bool _hasRewardedVideoAdUnit;
        [BoxGroup("Setup", true, true, 0), SerializeField]
        private string _rewardedVideoAdUnitAndroid = JAdMobTestData.AndroidRewardedAdUnitId;
        [BoxGroup("Setup", true, true, 0), SerializeField] private string _rewardedVideoAdUnitIOS = JAdMobTestData.IosRewardedAdUnitId;
        public string RewardedVideoAdUnit
            => Application.platform == RuntimePlatform.Android ? _rewardedVideoAdUnitAndroid : _rewardedVideoAdUnitIOS;

        public JAdUnit_RewardedVideo GetRewardedVideo(JAdAllower allower)
            => _hasRewardedVideoAdUnit ? new JAdUnit_RewardedVideo(allower, RewardedVideoAdUnit, _keepLoadingUntilReady) : default;

        [Button]
        public void ResetBannerAdUnitToDefault()
        {
            _bannerAdUnitAndroid = JAdMobTestData.AndroidBannerAdUnitId;
            _bannerAdUnitIOS     = JAdMobTestData.IosBannerAdUnitId;
        }

        [Button]
        public void ResetInterstitialAdUnitToDefault()
        {
            _interstitialAdUnitAndroid = JAdMobTestData.AndroidInterstitialAdUnitId;
            _interstitialAdUnitIOS     = JAdMobTestData.IosInterstitialAdUnitId;
        }

        [Button]
        public void ResetRewardedVideoAdUnitToDefault()
        {
            _rewardedVideoAdUnitAndroid = JAdMobTestData.AndroidRewardedAdUnitId;
            _rewardedVideoAdUnitIOS     = JAdMobTestData.IosRewardedAdUnitId;
        }
    }
}
#endif
