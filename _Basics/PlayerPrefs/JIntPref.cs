using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact
{
    [Serializable]
    public class JIntPref
    {
        [BoxGroup("Setup", true, true), SerializeField, Required] private string _prefName;
        [BoxGroup("Setup", true, true), SerializeField, Required] private int _startValue;

        public JIntPref(string prefName, int startValue)
        {
            _prefName   = prefName;
            _startValue = startValue;
        }

        public int Value => PlayerPrefs.GetInt(_prefName, _startValue);
        public void SetValue(int value) { PlayerPrefs.SetInt(_prefName, value); }
    }
}
