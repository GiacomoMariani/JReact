using System;
using System.Collections.Generic;
using Cysharp.Text;
using JReact.TimeProgress;
using Sirenix.OdinInspector;
using UnityEngine.Assertions;

namespace JReact.Analytics
{
    public class JAnalyticsEvent
    {
        //please note we need just 6 digits on ms
        //the format is this with just 6 f instead of 7: https://learn.microsoft.com/en-us/dotnet/standard/base-types/standard-date-and-time-format-strings#Roundtrip
        private readonly string LocalDateFormat = "{0:yyyy-MM-ddTHH:mm:ss.ffffffK}{1}";
        private readonly string LocalDateSuffix = "__Local";

        private readonly string CurrentSent = "CurrentSent";
        private readonly string TotalSent = "TotalSent";
        private readonly string SessionTimeSinceStartup = "SessionTimeSec";
        private readonly string LocalTime = "LocalTime";

        // --------------- EVENT DATA --------------- //
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public readonly string eventName;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private static int _totalEventsSent = 0;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private int _totalOfThis = 0;

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public DateTime UserTimeStamp { get; private set; }
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public float SessionTimeValue { get; private set; }
        
        // --------------- CACHE AND BOOK KEEPING --------------- //
        internal static readonly Dictionary<string, object> Values = new Dictionary<string, object>();
        private static readonly Dictionary<string, JAnalyticsEvent> _Registered =
            new Dictionary<string, JAnalyticsEvent>();

        // --------------- INIT --------------- //
        public static JAnalyticsEvent GetOrGenerate(string eventName)
            => _Registered.TryGetValue(eventName, out JAnalyticsEvent @event)
                   ? @event
                   : new JAnalyticsEvent(eventName);

        private JAnalyticsEvent(string eventName)
        {
            Assert.IsFalse(_Registered.ContainsKey(eventName), $"{eventName} is already registered");
            this.eventName = eventName;
            _Registered.Add(eventName, this);
        }

        // --------------- COMMANDS --------------- //
        public JAnalyticsEvent Prepare()
        {
            Values.Clear();
            UpdateBaseValues();
            return this;
        }

        private void UpdateBaseValues(bool storeTimeAsValue = true)
        {
            Values[TotalSent]   = _totalEventsSent;
            Values[CurrentSent] = _totalOfThis;
            SessionTimeValue    = JTime.RealtimeSinceStartup;
            UserTimeStamp       = DateTime.Now;
            if (!storeTimeAsValue) { return; }

            Values[SessionTimeSinceStartup] = SessionTimeValue;
            Values[LocalTime]               = ZString.Format(LocalDateFormat, UserTimeStamp, LocalDateSuffix);
        }

        public JAnalyticsEvent AddValue(string valueName, object value)
        {
            Values[valueName] = value;
            return this;
        }

        public void Send()
        {
            J_St_Analytics.GetInstanceSafe().SendEvent(this);
            _totalEventsSent++;
            _totalOfThis++;
        }
    }

#if UNITY_EDITOR
    public static class JAnalyticsEventExample
    {
        public static void RunExample()
        {
            // Generate a new event with the name "UserLogin"
            var userLoginEvent = JAnalyticsEvent.GetOrGenerate("Test");

            // Prepare the event (this will clear previous values and update base values)
            userLoginEvent.Prepare()
                          .AddValue("UserId",      12345)
                          .AddValue("LoginMethod", "Email")
                          .Send();
        }
    }
#endif
}
