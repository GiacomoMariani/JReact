#if PLAYFAB_INTEGRATION
using System;
using Cysharp.Threading.Tasks;
using PlayFab;
using PlayFab.SharedModels;
using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.Playfab_Integration
{
    public sealed class J_PlayfabResult<TResult>
        where TResult : PlayFabResultCommon
    {
        // --------------- FIELDS --------------- //
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public bool TimedOutRequest;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public bool TimedOutForTraffic;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public bool TimedOut => TimedOutRequest || TimedOutForTraffic;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public readonly TResult Result;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public readonly PlayFabError Error;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public bool IsSuccessfull => Result != null;

        // --------------- CONSTRUCTORS --------------- //
        public J_PlayfabResult(TResult      result) => Result = result;
        public J_PlayfabResult(PlayFabError error) => Error = error;
        private J_PlayfabResult() {}
        public static readonly J_PlayfabResult<TResult> TrafficTimeOutResult =
            new J_PlayfabResult<TResult>() { TimedOutForTraffic = true };
        public static readonly J_PlayfabResult<TResult> RequestTimeOutResult =
            new J_PlayfabResult<TResult>() { TimedOutRequest = true };
    }

    public abstract class J_PlayfabRequest<TRequest, TResult>
        where TRequest : PlayFabRequestCommon
        where TResult : PlayFabResultCommon
    {
        /// <summary>
        /// we generate a result at callback, both in case of success or error
        /// </summary>
        [ShowInInspector, ReadOnly] public bool IsProcessing { get; private set; }
        [ShowInInspector, ReadOnly] private GameObject _requestor;

        //requires a parameterless constructor
        [ShowInInspector, ReadOnly] private readonly TRequest _request = Activator.CreateInstance<TRequest>();
        [ShowInInspector, ReadOnly] private J_PlayfabResult<TResult> LastResult;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public int CurrentRequest { get; private set; }

        [ShowInInspector, ReadOnly]
        private static readonly string _WaitTrafficTimeout = $"PlayfabRequestTraffic-{typeof(TRequest).FullName}";
        [ShowInInspector, ReadOnly]
        private static readonly string _WaitServerTimeOut = $"PlayfabRequestTimeout-{typeof(TRequest).FullName}";

        /// <summary>
        /// Sends a specific request on playfab API
        /// </summary>
        public async UniTask<J_PlayfabResult<TResult>> Process(GameObject requestor = default, int timeOutMs = 5000, int delayMs = 100)
        {
            // --------------- WAIT IF ANOTHER REQUEST IS RUNNING --------------- //
            var canRun = await J_Async_Utils.WaitUntilReady(IsReady, _WaitTrafficTimeout, timeOutMs, delayMs, requestor);

            if (!canRun) { return J_PlayfabResult<TResult>.TrafficTimeOutResult; }

            // --------------- SETUP THE REQUEST --------------- //
            IsProcessing = true;
            _requestor   = requestor;
            LastResult   = default;

            UpdateRequest(_request);
            JLog.Log($"{typeof(TRequest).FullName} sending request {CurrentRequest}", JLogTags.Playfab, _requestor);

            // --------------- WAIT FOR RESPONSE --------------- //
            try
            {
                SendRequest(_request, OnSuccess, OnError);
                var hasReceivedAnswer = await J_Async_Utils.WaitUntilReady(IsReady, _WaitServerTimeOut, timeOutMs, delayMs, requestor);

                if (!hasReceivedAnswer) { return J_PlayfabResult<TResult>.RequestTimeOutResult; }
            }
            finally
            {
                IsProcessing = false;
                _requestor   = default;
                ResetRequest(_request);
                CurrentRequest++;
            }

            return LastResult;
        }

        /// <summary>
        /// Resets the given request object to its default state.
        /// </summary>
        /// <typeparam name="TRequest">The type of the request object.</typeparam>
        /// <param name="request">The request object to be reset.</param>
        protected abstract void ResetRequest(TRequest request);

        /// <summary>
        /// Updates the request before sending it.
        /// </summary>
        /// <param name="request">The request to update.</param>
        /// <returns>The updated request.</returns>
        protected abstract TRequest UpdateRequest(TRequest request);

        /// <summary>
        /// Sends a request to the PlayFab server.
        /// </summary>
        /// <typeparam name="TRequest">The type of request being sent.</typeparam>
        /// <typeparam name="TResult">The type of result expected from the server.</typeparam>
        /// <param name="request">The request object to be sent.</param>
        /// <param name="successCallback">The callback to be executed when the request is successful.</param>
        /// <param name="errorCallback">The callback to be executed when an error occurs.</param>
        protected abstract void SendRequest(TRequest request, Action<TResult> successCallback, Action<PlayFabError> errorCallback);

        private bool IsReady() => !IsProcessing;

        private void OnSuccess(TResult result) { LastResult = new J_PlayfabResult<TResult>(result); }

        private void OnError(PlayFabError error)
        {
            LastResult = new J_PlayfabResult<TResult>(error);
            JLog.Error($"{this.GetType().Name}:{error.GenerateErrorReport()}", JLogTags.Playfab);
        }
    }
}
#endif
