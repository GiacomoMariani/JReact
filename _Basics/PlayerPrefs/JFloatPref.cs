using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact
{
    [Serializable]
    public class JFloatPref
    {
        [BoxGroup("Setup", true, true), SerializeField, Required] private string _prefName;
        [BoxGroup("Setup", true, true), SerializeField, Required] private float _startValue;

        public JFloatPref(string prefName, float startValue)
        {
            _prefName   = prefName;
            _startValue = startValue;
        }

        public float Value => PlayerPrefs.GetFloat(_prefName, _startValue);
        public void SetValue(float value) { PlayerPrefs.SetFloat(_prefName, value); }
    }
}
