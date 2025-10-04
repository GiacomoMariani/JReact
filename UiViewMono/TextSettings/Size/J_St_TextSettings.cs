#if NX_BITBUFFER
using System;
using System.Collections.Generic;
using JReact;
using JReact.Singleton;
using NetStack.Serialization;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace Jreact.UiViewMono.TextSettings
{
    public sealed class J_St_TextSettings : J_MonoSingleton<J_St_TextSettings>
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        public List<Action<J_St_TextSettings>> OnTextSizeChange = new List<Action<J_St_TextSettings>>();

        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private J_SO_01_TextType[] _textTypes;

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector]
        private Dictionary<J_SO_01_TextType, JTextSetting> _currentSets = new Dictionary<J_SO_01_TextType, JTextSetting>();

        protected internal override void InitThis()
        {
            base.InitThis();

            _currentSets.Clear();
            for (int i = 0; i < _textTypes.Length; i++) { SetTextSetting(_textTypes[i], _textTypes[i].SavedSizeId); }
        }

        public void SetTextSetting(J_SO_01_TextType text, JTextSetting setting)
        {
            JLog.Log($"{text.TextType} => {setting.FontType} {setting.SizeSet}", JLogTags.Settings, this);
            _currentSets[text] = setting;
            SendChangeEvent();
        }

        public JTextSetting GetTextSetting(J_SO_01_TextType text) => _currentSets[text];

        private void SendChangeEvent()
        {
            foreach (Action<J_St_TextSettings> action in OnTextSizeChange) { action?.Invoke(this); }
        }

        public void Subscribe(Action<J_St_TextSettings> action)
        {
            if (!OnTextSizeChange.Contains(action)) { OnTextSizeChange.Add(action); }
        }

        public void Unsubscribe(Action<J_St_TextSettings> action)
        {
            if (OnTextSizeChange.Contains(action)) { OnTextSizeChange.Remove(action); }
        }

        // --------------- SERIALIZATION --------------- //
        public void Serialize(BitBuffer serializer)
        {
            for (int i = 0; i < _textTypes.Length; i++) { serializer.AddInt(_currentSets[_textTypes[i]]); }
        }

        public void DeSerialize(BitBuffer serializer)
        {
            for (int i = 0; i < _textTypes.Length; i++)
            {
                int sizeId = serializer.ReadInt();
                SetTextSetting(_textTypes[i], new JTextSetting(sizeId));
            }
        }
    }
}
#endif
