#if PLAYFAB_INTEGRATION
using Cysharp.Threading.Tasks;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.SharedModels;
using Sirenix.OdinInspector;

namespace JReact.Playfab_Integration
{
    public class J_PlayfabResult<TResult>
        where TResult : PlayFabResultCommon
    {
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public readonly TResult Result;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public readonly PlayFabError Error;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public bool IsSuccessfull => Error == null;

        public J_PlayfabResult(TResult      result) => Result = result;
        public J_PlayfabResult(PlayFabError error) => Error = error;
    }

    public abstract class J_PlayfabRequest<TRequest, TResult>
        where TRequest : PlayFabRequestCommon
        where TResult : PlayFabResultCommon
    {
        /// <summary>
        /// we generate a result at callback, both in case of success or error
        /// </summary>
        private bool IsProcessing => Result != default;

        //used only to pass the value between the request and the callback
        private J_PlayfabResult<TResult> Result;

        /// <summary>
        /// Sends a specific request on playfab API
        /// </summary>
        public async UniTask<J_PlayfabResult<TResult>> Process()
        {
            // --------------- SETUP THE LOGIN --------------- //
            Result = default;

            var request = CreateRequest();

            SendRequest(request);

            //wait until the call is ready
            await UniTask.WaitUntil(IsReady);

            //store a reference to reset the field in this static class. Required to implement async
            var result = Result;
            Result = null;

            return result;
        }

        /// <summary>
        /// used to generate the request based on the api, IE LoginRequest
        /// </summary>
        /// <returns>returns the generated request we will be using in the API</returns>
        protected abstract TRequest CreateRequest();

        /// <summary>
        /// send the request to playfab.
        /// IMPORTANT: OnSuccess and OnError need to be passed inside the request
        /// </summary>
        /// <param name="request">the request is pre generated</param>
        protected abstract void SendRequest(TRequest request);

        private bool IsReady() => !IsProcessing;

        /// <summary>
        /// Success callback, generating the result to stop processing (IsProcessiong => Result != default)
        /// </summary>
        private void OnSuccess(TResult result) { Result = new J_PlayfabResult<TResult>(result); }

        /// <summary>
        /// Error Callback, generating the result to stop processing (IsProcessiong => Result != default)
        /// </summary>
        private void OnError(PlayFabError error)
        {
            Result = new J_PlayfabResult<TResult>(error);
            JLog.Error($"{this.GetType().Name}:{error.GenerateErrorReport()}", JLogTags.Playfab);
        }
    }
}
#endif
