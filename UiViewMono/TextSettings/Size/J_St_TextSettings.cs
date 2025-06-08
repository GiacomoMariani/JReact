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
        private static string TextSizeSettingsId_Pref = "TextSizeSettingsId";
        // --------------- FIELDS AND PROPERTIES --------------- //
        public List<Action<J_St_TextSettings>> OnTextSizeChange = new List<Action<J_St_TextSettings>>();

        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private J_SO_TextSize[] _textSizes;
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private int _defaultId;

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public J_SO_TextSize CurrentSize { get; private set; }

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public bool HasSavedSize
            => PlayerPrefs.HasKey(nameof(TextSizeSettingsId_Pref));
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public int SavedSizeId
        {
            get => PlayerPrefs.GetInt(nameof(TextSizeSettingsId_Pref), 0);
            set => PlayerPrefs.SetInt(nameof(TextSizeSettingsId_Pref), value);
        }
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public J_SO_TextSize SavedSize => _textSizes[SavedSizeId];

        protected internal override void InitThis()
        {
            base.InitThis();
            SanityCheck();
            if (HasSavedSize) { SetSize(SavedSize); }
            else { SetDefault(); }
        }

        private void SanityCheck()
        {
            for (int i = 0; i < _textSizes.Length; i++)
            {
                bool isValid = _textSizes[i].SanityCheck();
                if(!isValid) { JLog.Error($"{name} found an invalid text size at index {i}", JLogTags.Settings, this); }
            }
        }

        public void SetDefault()
        {
            Assert.IsTrue(_defaultId < _textSizes.Length, $"Default size {_defaultId} not found");
            SetSize(_textSizes[_defaultId]);
        }

        public J_SO_TextSize GetSizeById(JeTextSizeType size)
        {
            for (int i = 0; i < _textSizes.Length; i++)
            {
                if (_textSizes[i].SizeType == size) { return _textSizes[i]; }
            }
            JLog.Error($"{name} could not find a size with {size}", JLogTags.Settings, this);
            return null;
        }
        
        [Button] public void SetSize(int textIndex) { SetSize(_textSizes[textIndex]); }

        [Button]
        public void SetSize(J_SO_TextSize textSize)
        {
            Assert.IsTrue(_textSizes.ArrayContains(textSize), $"Language {textSize} not found");
            CurrentSize = textSize;
            SavedSizeId = (byte) textSize.SizeType;
            SendChangeEvent();
        }

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
        public void Serialize(BitBuffer serializer) { serializer.AddByte((byte)CurrentSize.SizeType); }

        public void DeSerialize(BitBuffer serializer)
        {
            int sizeId = serializer.ReadByte();
            J_SO_TextSize size   = GetSizeById((JeTextSizeType)sizeId);
            SetSize(size);
        }
    }
}
#endif
