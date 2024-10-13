#if JADVERTISING
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GoogleMobileAds.Api;
using Sirenix.OdinInspector;
using UnityEngine.Assertions;

namespace JReact.Advertisting.AdUnit
{
    [Serializable]
    public class JAdUnit_RewardedVideo : IDisposable
    {
        // --------------- EVENTS --------------- //
        public event Action<JAdUnit_RewardedVideo> OnAdOpenedEvent;
        public event Action<JAdUnit_RewardedVideo> OnAdClosedEvent;
        public event Action<JAdUnit_RewardedVideo> OnAdClicked;
        public event Action<JAdUnit_RewardedVideo> OnAdAvailableEvent;
        public event Action<AdError, JAdUnit_RewardedVideo> OnAdShowFailedEvent;
        public event Action<Reward, JAdUnit_RewardedVideo> OnAdRewardedEvent;

        // --------------- CONTROLS --------------- //
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private IAllower _allower;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private RewardedAd _rewardedAd;
        public RewardedAd RewardedAd => _rewardedAd;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private JClickCounter _counter;

        // --------------- GENERAL LOGIC --------------- //
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private bool _keepLoadingUntilReady;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public bool IsLoading { get; private set; }
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public bool IsWaitingToShow { get; private set; }
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public bool IsShowing { get; private set; }
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public bool IsLoaded => _rewardedAd != null;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public bool IsRewardedVideoReady
            => IsLoaded && _rewardedAd.CanShowAd();

        // --------------- STATE --------------- //
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private string _adUnit;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private List<Action> _onSuccess = new List<Action>();
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private List<Action> _onFail = new List<Action>();

        public JAdUnit_RewardedVideo(IAllower allower, string adUnit = null, bool keepLoadingUntilReady = true)
        {
            Assert.IsNotNull(allower, $"Requires a {nameof(allower)}");
            _allower               = allower;
            _counter               = new JClickCounter(adUnit, JAdTypes.RewardedVideo);
            _keepLoadingUntilReady = keepLoadingUntilReady;
            LoadAd(adUnit);
        }

        // --------------- LOADING --------------- //
        public async void LoadAd(string adUnit, bool forceLoadNew = false)
        {
            if (IsLoading) { return; }

            if (IsLoaded)
            {
                if (forceLoadNew) { Dispose(); }
                else { return; }
            }

            IsLoading = true;
            _adUnit   = adUnit;

            var adRequest = new AdRequest();
            RewardedAd.Load(_adUnit, adRequest, RewardVideoLoaded);
            while (!IsRewardedVideoReady) { await UniTask.DelayFrame(1); }

            OnAdAvailableEvent?.Invoke(this);
        }
        
        private void RewardVideoLoaded(RewardedAd ad, LoadAdError error)
        {
            IsLoading = false;
            if (error != null ||
                ad    == null)
            {
                JLog.Error($"Ad Load Failed: {error}");
                if (_keepLoadingUntilReady) { ReloadSame(); }

                return;
            }

            JLog.Log($"Loaded rewarded video: {ad.GetResponseInfo()}", JLogTags.Advertisemnt);
            AssignAd(ad);
        }

        private void ReloadSame() => LoadAd(_adUnit);

        // --------------- QUERIES --------------- //
        private bool CanShow()
        {
            if (!IsRewardedVideoReady) { return false; }

            if (IsShowing) { return false; }

            return _allower == default || _allower.IsAllowed();
        }

        // --------------- OPEN COMMANDS --------------- //
        [Button]
        public void ShowRewardedVideo(Action onReward = default, Action onFail = default)
        {
            if (!IsRewardedVideoReady) { return; }

            _onSuccess.Clear();
            _onFail.Clear();
            if (onReward != null) { _onSuccess.Add(onReward); }

            if (onFail != null) { _onFail.Add(onFail); }

            IsShowing = true;
            _rewardedAd.Show(ShowResult);
        }

        [Button]
        public async UniTask ShowWhenReady(Action onReward = default, Action onFail = default)
        {
            if (IsWaitingToShow) { return; }

            IsWaitingToShow = true;
            while (!CanShow()) { await UniTask.DelayFrame(1); }

            ShowRewardedVideo(onReward, onFail);
            IsWaitingToShow = false;
        }

        // --------------- EVENTS LISTENER --------------- //
        private void ShowResult(Reward result)
        {
            JLog.Log($"{_adUnit} Show Result: {result}", JLogTags.Advertisemnt, J_St_Advertising.Instance);
            for (int i = 0; i < _onSuccess.Count; i++) { _onSuccess[i]?.Invoke(); }

            OnAdRewardedEvent?.Invoke(result, this);
        }

        private void OnAdPaidHandler(AdValue adValue)
        {
            JLog.Log($"{_adUnit} Ad Paid: {adValue.Value} {adValue.CurrencyCode}", JLogTags.Advertisemnt,
                     J_St_Advertising.Instance);
        }

        private void OnAdImpressionRecordedHandler()
        {
            JLog.Log($"{_adUnit} Ad Impression Recorded", JLogTags.Advertisemnt, J_St_Advertising.Instance);
        }

        private void OnAdClickedHandler()
        {
            JLog.Log($"{_adUnit} Ad Clicked", JLogTags.Advertisemnt, J_St_Advertising.Instance);
            _counter.AddClick();
        }

        private void OnAdFullScreenContentOpenedHandler()
        {
            JLog.Log($"{_adUnit} Full Screen Content Opened", JLogTags.Advertisemnt, J_St_Advertising.Instance);
            _counter.AddShow();
            OnAdOpenedEvent?.Invoke(this);
        }

        private void OnAdFullScreenContentClosedHandler()
        {
            JLog.Log($"{_adUnit} Full Screen Content Closed", JLogTags.Advertisemnt, J_St_Advertising.Instance);
            IsShowing = false;
            ReloadSame();
            OnAdClosedEvent?.Invoke(this);
        }

        private void OnAdFullScreenContentFailedHandler(AdError error)
        {
            JLog.Warning($"{_adUnit} Full Screen Content Failed: {error.GetMessage()}", JLogTags.Advertisemnt,
                         J_St_Advertising.Instance);

            IsShowing = false;
            for (int i = 0; i < _onFail.Count; i++) { _onFail[i]?.Invoke(); }

            ReloadSame();
            OnAdShowFailedEvent?.Invoke(error, this);
        }

        public void CounterFullReset()
        {
            _counter.ResetCurrent();
            _counter.ResetTotals();
        }

        // --------------- ASSIGN AND DISPOSE --------------- //
        private void AssignAd(RewardedAd ad)
        {
            _rewardedAd                             =  ad;
            _rewardedAd.OnAdPaid                    += OnAdPaidHandler;
            _rewardedAd.OnAdImpressionRecorded      += OnAdImpressionRecordedHandler;
            _rewardedAd.OnAdClicked                 += OnAdClickedHandler;
            _rewardedAd.OnAdFullScreenContentOpened += OnAdFullScreenContentOpenedHandler;
            _rewardedAd.OnAdFullScreenContentClosed += OnAdFullScreenContentClosedHandler;
            _rewardedAd.OnAdFullScreenContentFailed += OnAdFullScreenContentFailedHandler;
        }
        
        public void Dispose()
        {
            if (IsLoaded) { _rewardedAd.Destroy(); }

            _rewardedAd.OnAdPaid                    -= OnAdPaidHandler;
            _rewardedAd.OnAdImpressionRecorded      -= OnAdImpressionRecordedHandler;
            _rewardedAd.OnAdClicked                 -= OnAdClickedHandler;
            _rewardedAd.OnAdFullScreenContentOpened -= OnAdFullScreenContentOpenedHandler;
            _rewardedAd.OnAdFullScreenContentClosed -= OnAdFullScreenContentClosedHandler;
            _rewardedAd.OnAdFullScreenContentFailed -= OnAdFullScreenContentFailedHandler;
        }
    }
}
#endif
