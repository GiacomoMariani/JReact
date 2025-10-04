using System;
using Cysharp.Text;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace Jreact.UiViewMono.TextSettings
{
    /// <summary>
    /// this represent a type of text, such as a title, a description, a note, etc.
    /// each type has its own possible fonts
    /// and each font have different syzes
    /// FULL EXAMPLE OF SET:
    /// Type: Title
    /// Font: Arial
    /// SizeType: Big
    /// SizeFloat = 64
    /// </summary>
    [CreateAssetMenu(menuName = "Reactive/UiCustomization/Text Style", fileName = "J_SO_TextStyle", order = 0)]
    public sealed class J_SO_01_TextType : ScriptableObject
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        private static string TextSizeSettingsId_FormatPref = "Font_{0}";
        private string GetPlayerPrefName => ZString.Format(TextSizeSettingsId_FormatPref, TextType);

        [BoxGroup("Setup", true, true, 0), SerializeField] private string _textType = "Default";
        public string TextType => _textType;
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private J_SO_02_TextFontSet[] _sets;
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private J_SO_02_TextFontSet _default;

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public ushort DefaultIndex
            => (ushort)Array.IndexOf(_sets, _default);
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public bool HasSavedSize => PlayerPrefs.HasKey(GetPlayerPrefName);
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public JTextSetting SavedSizeId
        {
            get => HasSavedSize
                       ? new JTextSetting(PlayerPrefs.GetInt(GetPlayerPrefName, 0))
                       : new JTextSetting(DefaultIndex, _default.DefaultIndex);
            set => PlayerPrefs.SetInt(GetPlayerPrefName, value);
        }

        public TMP_FontAsset       GetFontAsset(JTextSetting setting) => _sets[setting.FontType].FontAsset;
        public TMP_FontAsset       GetDefaultFontAsset()              => _sets[DefaultIndex].FontAsset;
        public J_SO_02_TextFontSet GetDefaultFontSet()                => _sets[DefaultIndex];
        
        public float GetFontSize(JTextSetting setting, int fontSize)
            => _sets[setting.FontType].GetFontSet(setting.SizeSet).GetSizeFromIndex(fontSize);

        public float GetDefaultFontSize(int fontSize)
            => _sets[DefaultIndex].GetDefaultFontSet().GetSizeFromIndex(fontSize);
    }
}
