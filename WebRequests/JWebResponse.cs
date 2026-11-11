using System;
using System.Globalization;
using System.Text;
using UnityEngine.Networking;

namespace JReact.JWebRequests
{
    /// <summary>
    /// The result of one attempt, copied out of the UnityWebRequest before it is disposed.
    /// </summary>
    public readonly struct JWebResponse
    {
        private const string ProblemDetailsType = "application/problem+json";

        public long StatusCode { get; }
        public UnityWebRequest.Result Result { get; }
        public string Error { get; }
        public byte[] Body { get; }
        public string ContentType { get; }
        public double? RetryAfterSeconds { get; }

        public bool IsSuccess => Result == UnityWebRequest.Result.Success;
        public string Text => Body == null ? string.Empty : Encoding.UTF8.GetString(Body);

        // RFC 7807 problem details: the server explained the rejection.
        public bool IsProblemDetails =>
            ContentType != null && ContentType.StartsWith(ProblemDetailsType, StringComparison.OrdinalIgnoreCase);

        public JWebResponse(long statusCode, UnityWebRequest.Result result, string error, byte[] body, string contentType,
                            double? retryAfterSeconds)
        {
            StatusCode        = statusCode;
            Result            = result;
            Error             = error;
            Body              = body;
            ContentType       = contentType;
            RetryAfterSeconds = retryAfterSeconds;
        }

        internal static JWebResponse From(UnityWebRequest request) =>
            new JWebResponse(request.responseCode, request.result, request.error,
                             request.downloadHandler is DownloadHandlerBuffer buffer ? buffer.data : null,
                             request.GetResponseHeader("Content-Type"),
                             ParseRetryAfter(request.GetResponseHeader("Retry-After")));

        // Retry-After is either delay seconds or an HTTP date (RFC 9110).
        private static double? ParseRetryAfter(string value)
        {
            if (string.IsNullOrEmpty(value)) { return null; }

            if (int.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out var seconds)) { return seconds; }

            if (DateTimeOffset.TryParseExact(value, "r", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal,
                                             out var date)) { return Math.Max(0, (date - DateTimeOffset.UtcNow).TotalSeconds); }

            return null;
        }
    }
}
