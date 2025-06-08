using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Networking;
using Object = UnityEngine.Object;

namespace JReact.JWebRequests
{
    public class JWebRequest
    {
        private static readonly StringBuilder _stringBuilder = new StringBuilder(1024);
        private readonly char AndSeparator = '&';
        private readonly char ParameterSeparator = '?';
        private readonly char BackSlash = '/';
        private readonly string InvalidResponse = string.Empty;
        private readonly string JsonContent = "application/json";
        private const string GetMethod = "GET";
        private const string PostMethod = "POST";

        public JWebRequestMethod Method { get; private set; }

        private UnityWebRequest _request;
        private Dictionary<string, string> _baseParameters = new Dictionary<string, string>();

        public bool HasParameters { get; private set; }
        public string BaseUrl { get; private set; }
        public string EndPoint { get; private set; }
        public string CurrentUrl => _request.url;

        public bool HasEndpoint => !string.IsNullOrEmpty(EndPoint);

        // --------------- BASE --------------- //
        public JWebRequest(string url, JWebRequestMethod method = JWebRequestMethod.Get)
        {
            BaseUrl = url;
            Method  = method;
            string methodString = "";
            switch (method)
            {
                case JWebRequestMethod.Get:  methodString = GetMethod; break;
                case JWebRequestMethod.Post: methodString = PostMethod; break;
                case JWebRequestMethod.NotSet:
                default: throw new ArgumentOutOfRangeException(nameof(method), method, null);
            }

            _request = new UnityWebRequest(url, methodString);
        }

        public JWebRequest SetEndpoint(string endPoint)
        {
            Assert.IsFalse(HasParameters, $"{this} cannot add endpoint after parameters");
            Assert.IsFalse(HasEndpoint,   $"{this} already has an endpoint");
            EndPoint = endPoint;
            _stringBuilder.Clear();
            _stringBuilder.Append(_request.url).Append(BackSlash).Append(endPoint);

            _request.url = _stringBuilder.ToString();
            _stringBuilder.Clear();
            return this;
        }

        public JWebRequest SetTimeout(int timeout)
        {
            _request.timeout = timeout;
            return this;
        }

        // --------------- PARAMETERS --------------- //
        public JWebRequest AddParameter(string key, string value, bool isBaseParameter = false)
        {
            if (isBaseParameter) { _baseParameters[key] = value; }

            _stringBuilder.Clear();
            _stringBuilder.Append(_request.url);
            if (!HasEndpoint) { _stringBuilder.Append(BackSlash); }
            else { _stringBuilder.Append(!HasParameters ? ParameterSeparator : AndSeparator); }

            _stringBuilder.AppendFormat("{0}={1}", Uri.EscapeDataString(key), Uri.EscapeDataString(value));

            _request.url  = _stringBuilder.ToString();
            HasParameters = true;
            return this;
        }

        public JWebRequest ResetParameters(bool alsoBaseParameters = false)
        {
            _request.url = BaseUrl;
            if (HasEndpoint) { SetEndpoint(EndPoint); }

            HasParameters = false;
            if (alsoBaseParameters) { _baseParameters.Clear(); }
            else { AddParameters(_baseParameters, false); }

            return this;
        }

        public JWebRequest AddParameters(Dictionary<string, string> parameters, bool isBaseParameter = false)
        {
            foreach (var parameter in parameters) { AddParameter(parameter.Key, parameter.Value); }

            return this;
        }

        // --------------- HEADER --------------- //
        public JWebRequest AddHeader(string header, string value)
        {
            _request.SetRequestHeader(header, value);
            return this;
        }

        // --------------- DATA UPLOAD --------------- //
        public JWebRequest SetJsonContent<T>(T data)
        {
            string           jsonString = JsonUtility.ToJson(data);
            byte[]           bytes      = Encoding.UTF8.GetBytes(jsonString);
            UploadHandlerRaw raw        = new UploadHandlerRaw(bytes);
            _request.uploadHandler             = raw;
            _request.uploadHandler.contentType = JsonContent;
            return this;
        }

        // --------------- SET DOWNLOADER --------------- //
        public JWebRequest SetFileDownloader(string filePath)
        {
            _request.downloadHandler = new DownloadHandlerFile(filePath);
            return this;
        }

        // --------------- EXECUTION --------------- //
        public async UniTask<string> Execute(int maxTryCount, Object sender = default)
        {
            LogImpl($"Request: {this} - Max Retry: {maxTryCount}", sender);

            await _request.SendWebRequest();

            switch (_request.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    LogImpl($"({_request.result}-{_request.responseCode}) Error on client: {_request.error}", sender); break;

                case UnityWebRequest.Result.ProtocolError:
                    LogImpl($"{_request.result}-{_request.responseCode} Error on server: {_request.error}", sender); break;

                case UnityWebRequest.Result.Success:
                    LogImpl($"{_request.result}-Success: {_request.responseCode} - {_request.downloadHandler.text}", sender);
                    return _request.downloadHandler.text;
            }

            switch (_request.responseCode)
            {
                case 400:
                case 404:
                    LogImpl($"Error on client: {_request.responseCode}: {_request.error}, retry aborted", sender);
                    return InvalidResponse;
            }

            if (maxTryCount <= 0) { return InvalidResponse; }

            LogImpl($"Retrying {this} => ({maxTryCount} attempts left)", sender);
            await Task.Delay(500);
            return await Execute(maxTryCount - 1, sender);
        }

        // --------------- HELPERS --------------- //
        private void LogImpl(string msg, Object sender) { JLog.Log(msg, JLogTags.Network, sender); }

        public override string ToString()
        {
            return $"WebRequest[Method: {Method}, URL: {_request.url}]"                 +
                   $"\nDownloadHandler: {_request.downloadHandler?.text     ?? "none"}" +
                   $"\nUploadHandler: {_request.uploadHandler?.data?.Length ?? 0} bytes";
        }
    }
}
