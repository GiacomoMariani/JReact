#if PLAYFAB_INTEGRATION
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

namespace JReact.Playfab_Integration
{
    public sealed class J_PlayfabDataGetter : J_PlayfabRequest<GetUserDataRequest, GetUserDataResult>
    {
        public const int MaxRequests = 10;
        // --------------- FIELDS AND PROPERTIES --------------- //
        private readonly List<IJPlayfabData> _requested = new List<IJPlayfabData>();
        public bool HasQueue => _requested.Count > 0;

        private readonly List<string> _currentBatchKeys = new List<string>();
        private readonly List<IJPlayfabData> _currentBatchData = new List<IJPlayfabData>();
        public bool IsInProcess => _currentBatchKeys.Count > 0;
        private static readonly J_PlayfabDataGetter _getterInstance = new J_PlayfabDataGetter();
        private J_PlayfabDataGetter() {}

        // --------------- MAIN CONTROLS --------------- //
        public static bool IsLoading(IJPlayfabData data)
            => _getterInstance._requested.Contains(data) || _getterInstance._currentBatchData.Contains(data);

        public static J_PlayfabDataGetter Add(IJPlayfabData data)
        {
            if (IsLoading(data)) { return _getterInstance; }

            _getterInstance._requested.Add(data);
            return _getterInstance;
        }

        public static async Task<J_PlayfabResult<GetUserDataResult>> LoadAllStatic() => await _getterInstance.LoadAll();

        public async Task<J_PlayfabResult<GetUserDataResult>> LoadAll()
        {
            J_PlayfabResult<GetUserDataResult> lastResult = default;
            while (HasQueue) { lastResult = await _getterInstance.Process(); }

            return lastResult;
        }

        // --------------- REQUEST IMPLEMENTATION --------------- //
        protected override void ResetRequest(GetUserDataRequest request) { request.Keys.Clear(); }

        protected override GetUserDataRequest UpdateRequest(GetUserDataRequest request)
        {
            int currentBatchAmount = Mathf.Min(_requested.Count, MaxRequests);
            for (int i = 0; i < currentBatchAmount; i++)
            {
                _currentBatchKeys.Add(_requested[i].Key);
                _currentBatchData.Add(_requested[i]);
            }

            if (currentBatchAmount >= _requested.Count) { _requested.Clear(); }
            else { _requested.RemoveRange(0, currentBatchAmount); }

            request.PlayFabId = GetPlayfabId();
            request.Keys      = _currentBatchKeys;
            return request;
        }

        private string GetPlayfabId() => default;

        protected override void SendRequest(GetUserDataRequest   request, Action<GetUserDataResult> successCallback,
                                            Action<PlayFabError> errorCallback)
        {
            if (!IsInProcess)
            {
                JLog.Warning("No data to load. Operation skipped.", JLogTags.Playfab);
                return;
            }

            successCallback += KeysReceived;
            errorCallback   += Fail;
            PlayFabClientAPI.GetUserData(request, successCallback, errorCallback);
        }

        // --------------- RESULT HANDLING --------------- //
        private void KeysReceived(GetUserDataResult result)
        {
            Dictionary<string, UserDataRecord> resultData = result.Data;
            for (int i = 0; i < _currentBatchKeys.Count; i++)
            {
                if (resultData != null &&
                    resultData.TryGetValue(_currentBatchKeys[i], out UserDataRecord data)) { _currentBatchData[i].ReceiveData(data); }
                else { _currentBatchData[i].EmptyDataRetrieved(); }
            }

            FinishLoad();
        }

        private void Fail(PlayFabError error)
        {
            for (int i = 0; i < _currentBatchData.Count; i++) { _requested[i].LoadError(error); }

            FinishLoad();
        }

        private void FinishLoad()
        {
            _currentBatchKeys.Clear();
            _currentBatchData.Clear();
        }
    }
}
#endif
