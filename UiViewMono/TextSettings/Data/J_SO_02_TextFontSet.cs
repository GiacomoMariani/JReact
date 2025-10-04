using System;
using JReact;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

namespace Jreact.UiViewMono.TextSettings
{
    /// <summary>
    /// this is s single font with multiple size options, such as for having Arial Big and Arial Small
    /// </summary>
    [CreateAssetMenu(menuName = "Reactive/UiCustomization/Text Font Set", fileName = "J_SO_TextSet", order = 0)]
    public class J_SO_02_TextFontSet : ScriptableObject
    {
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private TMP_FontAsset _fontAsset;
        public TMP_FontAsset FontAsset => _fontAsset;

        [BoxGroup("Setup", true, true, 0), SerializeField] private J_SO_03_FontSizes[] _setsSizes;
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] public J_SO_03_FontSizes _default;

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private string FontIdentifier => FontAsset.name;
        public ushort DefaultIndex => (ushort)Array.IndexOf(_setsSizes, _default);

        public J_SO_03_FontSizes GetFontSet(int setIndex)
        {
            Assert.IsTrue(_setsSizes.ContainsIndex(setIndex), $"Font set index {setIndex} is out of bounds.");
            return _setsSizes[setIndex];
        }
        
        public J_SO_03_FontSizes GetDefaultFontSet() => GetFontSet(DefaultIndex);
    }
}
