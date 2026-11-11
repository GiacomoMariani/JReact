using System;
using System.Collections.Generic;
using System.Text;

namespace JReact.JWebRequests
{
    /// <summary>
    /// Describes a request: URL, method, headers, timeout and body. It holds no live UnityWebRequest, so
    /// JWebClient can send it any number of times with a fresh request per attempt.
    /// </summary>
    public sealed class JWebRequest
    {
        public const int DefaultTimeoutSeconds = 30;
        private const string ContentEncodingHeader = "Content-Encoding";
        private const char Slash = '/';

        private readonly List<KeyValuePair<string, string>> _parameters = new List<KeyValuePair<string, string>>();
        private readonly Dictionary<string, string> _headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public string BaseUrl { get; }
        public JWebRequestMethod Method { get; }
        public string Endpoint { get; private set; } = string.Empty;
        public int TimeoutSeconds { get; private set; } = DefaultTimeoutSeconds;
        public byte[] Body { get; private set; }
        public string ContentType { get; private set; }
        public string DownloadFilePath { get; private set; }
        public IReadOnlyDictionary<string, string> Headers => _headers;
        public string Url => BuildUrl(true);

        // --------------- BASE --------------- //
        public JWebRequest(string baseUrl, JWebRequestMethod method = JWebRequestMethod.Get)
        {
            // Query values belong in AddParameter, so they are escaped and the endpoint can follow the base URL.
            if (string.IsNullOrEmpty(baseUrl) || baseUrl.IndexOf('?') >= 0)
            {
                throw new ArgumentException("Use an absolute URL without a query string.", nameof(baseUrl));
            }

            if (method == JWebRequestMethod.NotSet) { throw new ArgumentOutOfRangeException(nameof(method), method, null); }

            BaseUrl = baseUrl;
            Method  = method;
        }

        public JWebRequest SetEndpoint(string endpoint)
        {
            Endpoint = endpoint ?? string.Empty;
            return this;
        }

        public JWebRequest SetTimeout(int seconds)
        {
            if (seconds <= 0) { throw new ArgumentOutOfRangeException(nameof(seconds), seconds, "Use a positive timeout."); }

            TimeoutSeconds = seconds;
            return this;
        }

        // --------------- PARAMETERS --------------- //
        public JWebRequest AddParameter(string key, string value)
        {
            if (string.IsNullOrEmpty(key)) { throw new ArgumentException("A parameter key is required.", nameof(key)); }

            _parameters.Add(new KeyValuePair<string, string>(key, value ?? string.Empty));
            return this;
        }

        public JWebRequest AddParameters(IEnumerable<KeyValuePair<string, string>> parameters)
        {
            foreach (var parameter in parameters) { AddParameter(parameter.Key, parameter.Value); }

            return this;
        }

        // --------------- HEADERS --------------- //
        public JWebRequest AddHeader(string name, string value)
        {
            if (string.IsNullOrEmpty(name)) { throw new ArgumentException("A header name is required.", nameof(name)); }

            _headers[name] = value ?? string.Empty;
            return this;
        }

        // --------------- BODY AND DOWNLOAD --------------- //
        /// <summary>
        /// Sets the bytes to upload. Formatting (JSON, gzip) happens before this call; pass the encoding of
        /// compressed bodies, for example "gzip".
        /// </summary>
        public JWebRequest SetBody(byte[] body, string contentType, string contentEncoding = null)
        {
            if (string.IsNullOrEmpty(contentType)) { throw new ArgumentException("A content type is required.", nameof(contentType)); }

            Body        = body ?? throw new ArgumentNullException(nameof(body));
            ContentType = contentType;
            if (string.IsNullOrEmpty(contentEncoding)) { _headers.Remove(ContentEncodingHeader); }
            else { _headers[ContentEncodingHeader] = contentEncoding; }

            return this;
        }

        /// <summary>
        /// Writes the response to a file instead of keeping it in memory; JWebResponse.Body is then empty.
        /// </summary>
        public JWebRequest SetDownloadFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) { throw new ArgumentException("A file path is required.", nameof(filePath)); }

            DownloadFilePath = filePath;
            return this;
        }

        // --------------- HELPERS --------------- //
        private string BuildUrl(bool includeParameters)
        {
            var url = new StringBuilder(BaseUrl.TrimEnd(Slash));
            if (Endpoint.Length > 0) { url.Append(Slash).Append(Endpoint.Trim(Slash)); }

            if (!includeParameters) { return url.ToString(); }

            for (int i = 0; i < _parameters.Count; i++)
            {
                url.Append(i == 0 ? '?' : '&')
                   .Append(Uri.EscapeDataString(_parameters[i].Key))
                   .Append('=')
                   .Append(Uri.EscapeDataString(_parameters[i].Value));
            }

            return url.ToString();
        }

        // Query values stay out of logs: they can carry identifiers or tokens.
        public override string ToString() => $"{Method} {BuildUrl(false)}";
    }
}
