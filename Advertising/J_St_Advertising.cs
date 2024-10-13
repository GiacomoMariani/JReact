#if JADVERTISING
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using GoogleMobileAds.Api;
using JReact.Advertisting.AdUnit;
using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.Advertisting
{
    public class J_St_Advertising : MonoBehaviour
    {
        // --------------- CONSTANTS --------------- //
        // Always use test ads.
        // https://developers.google.com/admob/unity/test-ads
        internal static List<string> TestDeviceIds = new List<string>()
        {
            AdRequest.TestDeviceSimulator,
#if UNITY_IPHONE
            "96e23e80653bb28980d3f40beb58915c",
#elif UNITY_ANDROID
            "702815ACFC14FF222DA1DC767672A573"
#endif
        };

        // --------------- FIELDS AND PROPERTIES --------------- //
        public static J_St_Advertising Instance { get; private set; }
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private J_SO_AdData _data;

        [BoxGroup("Setup", true, true, 0), SerializeField, ChildGameObjectsOnly, Required]
        private J_UserConsentController _consent;

        // --------------- STATE --------------- //
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private Dictionary<string, AdapterStatus> _adapters;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public static bool IsAdsReady => Instance != null;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public bool IsLoading { get; private set; }
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public JAdAllower Allower { get; private set; }
        public static bool AdsEnabled => Instance ?? Instance.Allower.IsAllowed();

        // --------------- AD UNIT IMPLEMENTATION --------------- //
        [FoldoutGroup("AdUnit", false, 10), ShowInInspector] public JAdUnit_Banner Banner { get; private set; }
        [FoldoutGroup("AdUnit", false, 10), ShowInInspector] public JAdUnit_Interstitial Interstitial { get; private set; }
        [FoldoutGroup("AdUnit", false, 10), ShowInInspector] public JAdUnit_RewardedVideo RewardedVideo { get; private set; }

        [Button]
        public async UniTask InitNetwork()
        {
            if (IsLoading || IsAdsReady) { return; }

            JLog.Log($"{name} Init Advertising -- START", JLogTags.Advertisemnt, this);
            IsLoading = true;
            // --------------- ALLOWER --------------- //
            Allower = GenerateAllower();

            // --------------- CONFIGURATION --------------- //
            MobileAds.SetiOSAppPauseOnBackground(true);
            MobileAds.RaiseAdEventsOnUnityMainThread = true;
            RequestConfiguration configuration = SetupConfigurationRequest();
            MobileAds.SetRequestConfiguration(configuration);

            // --------------- CONSENT --------------- //
            bool gdprConsent = await GetConsent();

            // --------------- INIT --------------- //
            if (_consent.CanRequestAds) { MobileAds.Initialize(InitCompleteCallback); }

            await UniTask.WaitWhile(() => IsLoading);
            JLog.Log($"{name} Init Advertising -- END", JLogTags.Advertisemnt, this);
        }

        private JAdAllower GenerateAllower() => new JAdAllower();

        protected virtual RequestConfiguration SetupConfigurationRequest()
            => new RequestConfiguration() { TestDeviceIds = TestDeviceIds };

        private void InitCompleteCallback(InitializationStatus status)
        {
            if (status == null)
            {
                JLog.Error($"{name} AdMobInit Failed - {nameof(status)} null. Try again.", JLogTags.Advertisemnt, this);
                MobileAds.Initialize(InitCompleteCallback);
                return;
            }

            IsLoading = false;
            _adapters = status.getAdapterStatusMap();
            if (_adapters != null)
            {
                foreach (var item in _adapters)
                {
                    JLog.Log($"{name} Adapter => {item.Key} - {item.Value.InitializationState}", JLogTags.Advertisemnt, this);
                }
            }

            InitAdUnits();
            Instance = this;
            JLog.Log($"{name} AdMobInit Setup complete.", JLogTags.Advertisemnt, this);
        }

        private void InitAdUnits()
        {
            JLog.Log($"{name} Init Ad Units.", JLogTags.Advertisemnt, this);
            RewardedVideo = _data.GetRewardedVideo(Allower);
        }

        protected virtual async Task<bool> GetConsent()
        {
            if (_consent.CanRequestAds) { return true; }

            await _consent.GatherConsent();
            return _consent.CanRequestAds;
        }

        // --------------- OTHER COMMANDS --------------- //
        /// <summary>
        /// Opens the AdInspector.
        /// </summary>
        public void OpenAdInspector()
        {
            JLog.Log($"{name} Opening Ad Inspector.", JLogTags.Advertisemnt, this);

            MobileAds.OpenAdInspector((AdInspectorError error) =>
            {
                if (error != null)
                {
                    JLog.Error($"{name} Ad Inspector failed to open with error: {error}", JLogTags.Advertisemnt, this);
                    return;
                }

                JLog.Log($"{name} Ad Inspector opened successfully.", JLogTags.Advertisemnt, this);
            });
        }
    }
}
#endif
