using System;
using JReact.Random;
using UnityEngine.Networking;

namespace JReact.JWebRequests
{
    /// <summary>
    /// General HTTP rules: network failures, 408, 429 and 5xx are retried with exponential backoff and jitter,
    /// honouring Retry-After; any other rejection is dropped.
    /// </summary>
    public class JWebRetryPolicy
    {
        // Not readonly: J_RandomPCG is a mutable struct that must advance in place. Each instance gets its own seed,
        // so clients that failed together do not retry in lockstep.
        private J_RandomPCG _jitter = new J_RandomPCG(BitConverter.ToUInt64(Guid.NewGuid().ToByteArray(), 0));

        public int MaxAttempts { get; }
        public double BaseDelaySeconds { get; }
        public double MaxDelaySeconds { get; }

        public JWebRetryPolicy(int maxAttempts = 3, double baseDelaySeconds = 1, double maxDelaySeconds = 30)
        {
            if (maxAttempts < 1) { throw new ArgumentOutOfRangeException(nameof(maxAttempts), maxAttempts, null); }

            if (baseDelaySeconds <= 0 || maxDelaySeconds < baseDelaySeconds)
            {
                throw new ArgumentOutOfRangeException(nameof(maxDelaySeconds), maxDelaySeconds,
                                                      "Use 0 < baseDelaySeconds <= maxDelaySeconds.");
            }

            MaxAttempts      = maxAttempts;
            BaseDelaySeconds = baseDelaySeconds;
            MaxDelaySeconds  = maxDelaySeconds;
        }

        public virtual JWebOutcome Classify(JWebResponse response)
        {
            if (response.IsSuccess) { return JWebOutcome.Success; }

            // Connection and data processing failures happen before the server could answer.
            if (response.Result != UnityWebRequest.Result.ProtocolError) { return JWebOutcome.Retry; }

            var status = response.StatusCode;
            return status == 408 || status == 429 || status >= 500 ? JWebOutcome.Retry : JWebOutcome.Drop;
        }

        /// <summary>
        /// Seconds to wait before the given attempt is repeated, or null when the server asks for a longer wait
        /// than this policy allows, in which case the caller should retry later.
        /// </summary>
        public double? NextDelaySeconds(int attempt, JWebResponse response)
        {
            if (response.RetryAfterSeconds is double requested) { return requested <= MaxDelaySeconds ? requested : (double?)null; }

            var exponential = Math.Min(MaxDelaySeconds, BaseDelaySeconds * Math.Pow(2, attempt - 1));
            // Jitter spreads out retries from many clients that failed at the same moment.
            return exponential * _jitter.NextFloat(0.5f, 1f);
        }
    }
}
