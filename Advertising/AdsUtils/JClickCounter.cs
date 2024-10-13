using UnityEngine;

namespace JReact.Advertisting
{
    public sealed class JClickCounter
    {
        public readonly string identifier;

        public int ShowTotal { get => PlayerPrefs.GetInt(identifier, 0); private set => PlayerPrefs.SetInt(identifier, value); }

        public int ClickTotal { get => PlayerPrefs.GetInt(identifier, 0); private set => PlayerPrefs.SetInt(identifier, value); }

        public int CounterShow { get; private set; }
        public int CounterClick { get; private set; }

        public JClickCounter(string unitName, JAdTypes atType, string prefix = "", string postfix = "")
        {
            identifier   = GetAdUnitPrefsIdentifier(unitName, atType, prefix, postfix);
            CounterShow  = 0;
            CounterClick = 0;
        }

        private string GetAdUnitPrefsIdentifier(string unitName, JAdTypes atType, string prefix, string postfix)
            => $"{prefix}_{unitName}_{atType}_{postfix}";

        internal void AddShow()
        {
            CounterShow++;
            ShowTotal++;
        }

        internal void AddClick()
        {
            CounterClick++;
            ClickTotal++;
        }

        public void ResetTotals()  { CounterShow = ShowTotal    = ClickTotal = 0; }
        public void ResetCurrent() { CounterShow = CounterClick = ClickTotal = 0; }
    }
}
