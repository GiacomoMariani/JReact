#if PLAYFAB_INTEGRATION

using System;
using PlayFab;
using PlayFab.ClientModels;
using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.Analytics.Implementation
{
    public sealed class J_St_PlayfabAnalytics : J_St_Analytics
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField] private bool _logEvents;

        private static readonly WriteClientPlayerEventRequest EventRequest = new WriteClientPlayerEventRequest();

        internal override void SendEvent(JAnalyticsEvent eventToSend)
        {
            EventRequest.EventName  = eventToSend.eventName;
            EventRequest.CustomTags = _tags.Tags;
            EventRequest.Timestamp  = DateTime.UtcNow;
            EventRequest.Body       = JAnalyticsEvent.Values;
            PlayFabClientAPI.WritePlayerEvent(EventRequest, OnSuccess, OnError);
        }

        private void OnSuccess(WriteEventResponse eventSent)
        {
            if (_logEvents) { JLog.Log($"Event sent: {eventSent.ToJson()}"); }
        }

        private void OnError(PlayFabError error)
        {
            JLog.Error($"{this.GetType().Name}:{error.GenerateErrorReport()}", JLogTags.Playfab, this);
        }
    }
}
#endif
