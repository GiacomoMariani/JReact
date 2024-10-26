using System;
using System.Collections.Generic;
using System.Globalization;
using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.Analytics
{
    [Serializable]
    public sealed class JAnalyticsTags
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField] private string ProjectId = "Project";
        [BoxGroup("Setup", true, true, 0), SerializeField] private string PlatformId = "PlatformId";
        [BoxGroup("Setup", true, true, 0), SerializeField] private string DeviceId = "Device";
        [BoxGroup("Setup", true, true, 0), SerializeField] private string LanguageId = "Language";
        [BoxGroup("Setup", true, true, 0), SerializeField] private string CountryIso = "CountryIso";
        [BoxGroup("Setup", true, true, 0), SerializeField] private string AppVersion = "AppVersion";
        [BoxGroup("Setup", true, true, 0), SerializeField] private string SessionID = "SessionID";

        private const int _MaxCustomTags = 10;
        internal Dictionary<string, string> Tags = new(_MaxCustomTags);

        // --------------- INIT --------------- //
        public JAnalyticsTags()
        {
            Tags[ProjectId]  = GetProjectId();
            Tags[PlatformId] = GetCurrentPlatform();
            Tags[DeviceId]   = GetDevice();
            Tags[LanguageId] = GetLanguage();
            Tags[CountryIso] = GetCountryIso();
            Tags[AppVersion] = GetVersion();
            Tags[SessionID]  = GetSessionId();
        }

        public void SetTag(string tag, string value) => Tags[tag] = value;

        private string GetProjectId()       => Application.productName;
        private string GetCurrentPlatform() => Application.platform.ToString();
        private string GetDevice()          => SystemInfo.deviceModel;
        private string GetLanguage()        => CultureInfo.CurrentCulture.Name;
        private string GetCountryIso()      => new RegionInfo(CultureInfo.CurrentCulture.LCID).Name;
        private string GetVersion()         => Application.version;
        private string GetSessionId()       => Guid.NewGuid().ToString();
    }
}
