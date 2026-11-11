using UnityEngine.Networking;

namespace JReact.JWebRequests
{
    /// <summary>
    /// Rules of the Wayrex backend contract on top of the general ones: 413 asks the sender to split the
    /// payload, and only rejections the backend explains with problem details are dropped.
    /// </summary>
    public sealed class JWebBackendRetryPolicy : JWebRetryPolicy
    {
        public JWebBackendRetryPolicy(int maxAttempts = 3, double baseDelaySeconds = 1, double maxDelaySeconds = 30)
            : base(maxAttempts, baseDelaySeconds, maxDelaySeconds) {}

        public override JWebOutcome Classify(JWebResponse response)
        {
            if (response.Result != UnityWebRequest.Result.ProtocolError) { return base.Classify(response); }

            if (response.StatusCode == 413) { return JWebOutcome.Split; }

            // A 400 without problem details comes from infrastructure in front of the backend (for example a
            // proxy or HTTPS rejection), not from validation, so it can recover.
            if (response.StatusCode == 400 && !response.IsProblemDetails) { return JWebOutcome.Retry; }

            return base.Classify(response);
        }
    }
}
