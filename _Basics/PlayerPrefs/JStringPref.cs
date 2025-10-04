using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact
{
    [Serializable]
    public class JStringPref
    {
        [BoxGroup("Setup", true, true), SerializeField, Required] private string _prefName;
        [BoxGroup("Setup", true, true), SerializeField, Required] private string _startValue;

        public JStringPref(string prefName, string startValue)
        {
            _prefName   = prefName;
            _startValue = startValue;
        }

        public string Value => PlayerPrefs.GetString(_prefName, _startValue);
        public void SetValue(string value) { PlayerPrefs.SetString(_prefName, value); }
    }
}
