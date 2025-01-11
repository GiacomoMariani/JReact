#if PLAYFAB_INTEGRATION
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

namespace JReact.Playfab_Integration
{
    public class J_PlayfabDataSetter : J_PlayfabRequest<UpdateUserDataRequest, UpdateUserDataResult>
    {
        public const int MaxRequests = 10;
        // --------------- FIELDS AND PROPERTIES --------------- //
        private List<IJPlayfabData> _requested = new List<IJPlayfabData>();
        public bool HasQueue => _requested.Count > 0;

        private Dictionary<string, string> _currentBatch = new Dictionary<string, string>();
        public bool IsInProcess => _currentBatch.Count > 0;
        private static J_PlayfabDataSetter _setterInstance = new J_PlayfabDataSetter();

        private J_PlayfabDataSetter() {}

        public static bool IsSaving(IJPlayfabData data)
            => _setterInstance._requested.Contains(data) || _setterInstance._currentBatch.ContainsKey(data.Key);

        // --------------- MAIN CONTROLS --------------- //
        public static J_PlayfabDataSetter Add(IJPlayfabData data)
        {
            if (IsSaving(data)) { return _setterInstance; }

            _setterInstance._requested.Add(data);
            return _setterInstance;
        }

        public static async UniTask<J_PlayfabResult<UpdateUserDataResult>> SaveAllStatic() => await _setterInstance.SaveAll();

        public async UniTask<J_PlayfabResult<UpdateUserDataResult>> SaveAll()
        {
            J_PlayfabResult<UpdateUserDataResult> lastResult = default;
            while (HasQueue) { lastResult = await _setterInstance.Process(); }

            return lastResult;
        }

        // --------------- REQUEST IMPLEMENTATION --------------- //
        protected override void ResetRequest(UpdateUserDataRequest request) { request.Data.Clear(); }

        protected override UpdateUserDataRequest UpdateRequest(UpdateUserDataRequest request)
        {
            int currentBatchAmount = Mathf.Min(_requested.Count, MaxRequests);
            for (int i = 0; i < currentBatchAmount; i++)
            {
                IJPlayfabData data = _requested[i];
                _currentBatch[data.Key] = data.ConvertToString();
            }

            if (currentBatchAmount >= _requested.Count) { _requested.Clear(); }
            else { _requested.RemoveRange(0, currentBatchAmount); }

            request.Permission = UserDataPermission.Public;
            request.Data       = _currentBatch;
            return request;
        }

        protected override void SendRequest(UpdateUserDataRequest request, Action<UpdateUserDataResult> successCallback,
                                            Action<PlayFabError>  errorCallback)
        {
            if (!IsInProcess)
            {
                JLog.Warning("No data to save. Operation skipped.", JLogTags.Playfab);
                return;
            }

            successCallback += KeysReceived;
            errorCallback   += CallFail;
            PlayFabClientAPI.UpdateUserData(request, successCallback, errorCallback);
        }

        // --------------- RESULT HANDLING --------------- //
        private void CallFail(PlayFabError error) { ResetThis(); }

        private void KeysReceived(UpdateUserDataResult result) { ResetThis(); }

        private void ResetThis() { _currentBatch.Clear(); }
    }
}
#endif
