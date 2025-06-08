#if NX_BITBUFFER
using System;
using System.Collections.Generic;
using JReact.Localization.LocalizationText;
using JReact.SaveSystem;
using JReact.Singleton;
using NetStack.Serialization;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.Localization
{
    public class J_St_Localization : J_MonoSingleton<J_St_Localization>, jSerializable
    {
        private static string SavedLanguageId_Pref = "SavedLanguageId";
        // --------------- FIELDS AND PROPERTIES --------------- //
        public List<Action<J_St_Localization>> OnLocalizationChange = new List<Action<J_St_Localization>>();

        public static SystemLanguage Language => GetInstanceSafe().CurrentLanguage;
        public static int LanguageId => GetInstanceSafe().CurrentLanguageId;
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private J_SO_LocalizationLibrary _library;

        // --------------- STATE --------------- //
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public SystemLanguage CurrentLanguage { get; private set; }
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public int CurrentLanguageId
            => _library.LanguageToId(CurrentLanguage);

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public bool HasSavedLanguage
            => PlayerPrefs.HasKey(nameof(SavedLanguageId_Pref));

        // --------------- SAVED DATA --------------- //
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public int SavedLanguageId
        {
            get => PlayerPrefs.GetInt(nameof(SavedLanguageId_Pref), 0);
            set => PlayerPrefs.SetInt(nameof(SavedLanguageId_Pref), value);
        }
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public SystemLanguage SavedLanguage
            => _library.IdToLanguage(SavedLanguageId);

        public SystemLanguage GetSystemLanguage() => Application.systemLanguage;

        public SystemLanguage[] GetSystemLanguages() => _library.Languages;

        protected internal override void InitThis()
        {
            base.InitThis();
            if (HasSavedLanguage) { SetLanguage(SavedLanguage); }
            else { SetDefault(); }
        }

        public void SetDefault()
        {
            SystemLanguage   language       = GetSystemLanguage();
            SystemLanguage[] validLanguages = GetSystemLanguages();
            SetLanguage(validLanguages.ArrayContains(language) ? language : _library.DefaultLanguage);
        }

        [Button]
        public void SetLanguage(SystemLanguage language)
        {
            Assert.IsTrue(_library.Languages.ArrayContains(language), $"Language {language} not found");
            CurrentLanguage = language;
            SavedLanguageId = Array.IndexOf(_library.Languages, language);
            SendChangeEvent();
        }

        public string LanguageIds()
        {
            SystemLanguage[] availableLanguages = GetSystemLanguages();
            string           result             = "";
            for (int i = 0; i < availableLanguages.Length; i++) { result += $"{i} => {availableLanguages[i]},"; }

            return result;
        }

        private void SendChangeEvent()
        {
            foreach (Action<J_St_Localization> action in OnLocalizationChange) { action?.Invoke(this); }
        }

        public void Subscribe(Action<J_St_Localization> action)
        {
            if (!OnLocalizationChange.Contains(action)) { OnLocalizationChange.Add(action); }
        }

        public void Unsubscribe(Action<J_St_Localization> action)
        {
            if (OnLocalizationChange.Contains(action)) { OnLocalizationChange.Remove(action); }
        }

        // --------------- SERIALIZATION --------------- //
        public void Serialize(BitBuffer serializer)
        {
            serializer.AddByte((byte)CurrentLanguageId);
        }

        public void DeSerialize(BitBuffer serializer)
        {
            int languageId = serializer.ReadByte();
            SetLanguage(_library.IdToLanguage(languageId));
        }
    }
}
#endif
