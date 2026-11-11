using System;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;
using Object = UnityEngine.Object;

namespace JReact.JWebRequests
{
    /// <summary>
    /// Sends JWebRequests. Every attempt uses a new UnityWebRequest that is always disposed. Continuations run
    /// on the main thread; cancellation aborts the request in flight and throws OperationCanceledException.
    /// </summary>
    public static class JWebClient
    {
        // --------------- SEND --------------- //
        /// <summary>
        /// Sends the request, repeating it while the policy says Retry and attempts remain.
        /// </summary>
        public static async Awaitable<JWebResult> SendAsync(JWebRequest request, JWebRetryPolicy policy,
                                                            CancellationToken cancellation = default,
                                                            Object context = null)
        {
            for (int attempt = 1;; attempt++)
            {
                var response = await SendOnceAsync(request, cancellation);
                var outcome  = policy.Classify(response);
                var delay    = outcome == JWebOutcome.Retry && attempt < policy.MaxAttempts
                                   ? policy.NextDelaySeconds(attempt, response)
                                   : null;

                if (delay == null)
                {
                    Report(request, response, outcome, attempt, context);
                    return new JWebResult(outcome, response, attempt);
                }

                JLog.Warning($"{request} attempt {attempt}: {Describe(response)}; retrying in {delay.Value:0.0} s",
                             JLogTags.Network, context);

                await DelayRealtimeAsync(delay.Value, cancellation);
            }
        }

        /// <summary>
        /// Sends the request once, without retries.
        /// </summary>
        public static async Awaitable<JWebResponse> SendOnceAsync(JWebRequest request,
                                                                  CancellationToken cancellation = default)
        {
            cancellation.ThrowIfCancellationRequested();
            using var unityRequest = Create(request);
            try { await Awaitable.FromAsyncOperation(unityRequest.SendWebRequest(), cancellation); }
            catch (OperationCanceledException)
            {
                unityRequest.Abort();
                throw;
            }

            return JWebResponse.From(unityRequest);
        }

        // --------------- HELPERS --------------- //
        private static UnityWebRequest Create(JWebRequest request)
        {
            DownloadHandler download = request.DownloadFilePath == null
                                           ? new DownloadHandlerBuffer()
                                           : new DownloadHandlerFile(request.DownloadFilePath) { removeFileOnAbort = true };

            UploadHandler upload = request.Body == null
                                       ? null
                                       : new UploadHandlerRaw(request.Body) { contentType = request.ContentType };

            var unityRequest = new UnityWebRequest(request.Url, Verb(request.Method), download, upload)
            {
                timeout = request.TimeoutSeconds,
                // Without this, uploads wait for a "100 Continue" round trip before sending the body.
                useHttpContinue = false
            };

            foreach (var header in request.Headers) { unityRequest.SetRequestHeader(header.Key, header.Value); }

            return unityRequest;
        }

        private static string Verb(JWebRequestMethod method)
        {
            switch (method)
            {
                case JWebRequestMethod.Get:  return UnityWebRequest.kHttpVerbGET;
                case JWebRequestMethod.Post: return UnityWebRequest.kHttpVerbPOST;
                default:                     throw new ArgumentOutOfRangeException(nameof(method), method, null);
            }
        }

        // Realtime on purpose: retries keep their timing while the game is paused (Time.timeScale = 0).
        private static async Awaitable DelayRealtimeAsync(double seconds, CancellationToken cancellation)
        {
            var resumeAt = Time.realtimeSinceStartupAsDouble + seconds;
            while (Time.realtimeSinceStartupAsDouble < resumeAt) { await Awaitable.NextFrameAsync(cancellation); }
        }

        // Status and outcome only: bodies and query strings can carry player data.
        private static void Report(JWebRequest request, JWebResponse response, JWebOutcome outcome, int attempts,
                                   Object context)
        {
            var message = $"{request} -> {outcome} after {attempts} attempt(s): {Describe(response)}";
            if (outcome == JWebOutcome.Success) { JLog.Trace(message, JLogTags.Network, context); }
            else { JLog.Warning(message, JLogTags.Network, context); }
        }

        private static string Describe(JWebResponse response) =>
            response.Result == UnityWebRequest.Result.ProtocolError || response.IsSuccess
                ? $"HTTP {response.StatusCode}"
                : $"{response.Result} ({response.Error})";
    }
}
