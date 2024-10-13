#if JADVERTISING
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GoogleMobileAds.Ump.Api;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace JReact.Advertisting
{
    public class J_UserConsentController : MonoBehaviour
    {
        [BoxGroup("Setup", true, true, 0), SerializeField] private bool _underAge;
        [BoxGroup("Setup", true, true, 0), SerializeField] private DebugGeography _debugGeography = DebugGeography.Disabled;

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public bool IsRequesting { get; private set; }
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private List<Action> _onSuccess = new List<Action>();
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private List<Action> _onFail = new List<Action>();

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public bool CanRequestAds => ConsentInformation.CanRequestAds();
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public bool IsConsentRequired
            => ConsentInformation.PrivacyOptionsRequirementStatus == PrivacyOptionsRequirementStatus.Required;

        [Button]
        public async UniTask GatherConsent(Action onSuccess = default, Action onFail = default)
        {
            JLog.Log($"{name} Gathering Consent -- START", JLogTags.Advertisemnt, this);

            if (!IsRequesting)
            {
                JLog.Log($"{name} Gathering Consent - Request Sent.", JLogTags.Advertisemnt, this);
                ConsentRequestParameters requestParameters = SetupConsentRequest();
                IsRequesting = true;
                _onSuccess.Clear();
                _onSuccess.Add(onSuccess);
                _onFail.Clear();
                _onFail.Add(onFail);
                ConsentInformation.Update(requestParameters, ConsentResult);
            }
            else
            {
                _onSuccess.Add(onSuccess);
                _onFail.Add(onFail);
            }

            await UniTask.WaitWhile(() => IsRequesting);
            JLog.Log($"{name} LGathering Consent -- END.", JLogTags.Advertisemnt, this);
        }

        private void ConsentResult(FormError error)
        {
            JLog.Log($"{name} Loading Consent - Completed. Errors? {error}.", JLogTags.Advertisemnt, this);
            if (error != null)
            {
                ConsentFail(error);
                return;
            }

            if (CanRequestAds)
            {
                ConsentSuccess();
                return;
            }

            // Consent not obtained and is required.
            // Load the initial consent request form for the user.
            JLog.Log($"{name} Loading Consent Form.", JLogTags.Advertisemnt, this);
            ConsentForm.LoadAndShowConsentFormIfRequired(ConsentGetFormAnswer);
        }

        private void ConsentGetFormAnswer(FormError error)
        {
            JLog.Log($"{name} Consent Form Completed - Completed. Errors? {error}.", JLogTags.Advertisemnt, this);
            if (error != null) { ConsentFail(error); }
            else { ConsentSuccess(); }
        }

        private ConsentRequestParameters SetupConsentRequest() => new()
        {
            TagForUnderAgeOfConsent = _underAge,
            ConsentDebugSettings = new ConsentDebugSettings
            {
                DebugGeography = _debugGeography, TestDeviceHashedIds = J_St_Advertising.TestDeviceIds,
            }
        };

        public void ShowPrivacyOptionsForm(Action<string> onComplete)
        {
            Debug.Log("Showing privacy options form.");

            ConsentForm.ShowPrivacyOptionsForm((FormError showError) =>
            {
                if (showError != null)
                {
                    // Form showing failed.
                    if (onComplete != null)
                    {
                        onComplete(showError.Message);
                    }
                }
                // Form showing succeeded.
                else if (onComplete != null)
                {
                    onComplete(null);
                }
            });
        }

        // --------------- SUCCESS/FAIL --------------- //
        private void ConsentSuccess()
        {
            JLog.Log($"Consent Request Complete. Consent; {CanRequestAds}", JLogTags.Advertisemnt, this);
            for (int i = 0; i < _onSuccess.Count; i++) { _onSuccess[i]?.Invoke(); }

            IsRequesting = false;
        }

        private void ConsentFail(FormError error)
        {
            JLog.Error($"Consent Request Error {error.ErrorCode}: {error.Message}", JLogTags.Advertisemnt, this);
            for (int i = 0; i < _onFail.Count; i++) { _onFail[i]?.Invoke(); }

            IsRequesting = false;
        }

        // --------------- REQUEST --------------- //
        public void ResetConsentInformation() { ConsentInformation.Reset(); }
    }
}
#endif
