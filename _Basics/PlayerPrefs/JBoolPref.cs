using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact
{
    [Serializable]
    public class JBoolPref
    {
        [BoxGroup("Setup", true, true), SerializeField, Required] private string _prefName;
        [BoxGroup("Setup", true, true), SerializeField, Required] private bool _startValue;

        public JBoolPref(string prefName, bool startValue)
        {
            _prefName   = prefName;
            _startValue = startValue;
        }

        public bool Value => PlayerPrefs.GetInt(_prefName, _startValue ? 1 : 0) == 1;

        public void SetValue(bool value) { PlayerPrefs.SetInt(_prefName, value ? 1 : 0); }
    }
}
