using System;
using JReact;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace Jreact.UiViewMono.TextSettings
{
    /// <summary>
    /// This is a single size for a font. Each size might have multiple options, such as Big for Description and Small for Notes
    /// Then each Size has its own subtype
    /// </summary>
    [CreateAssetMenu(menuName = "Reactive/UiCustomization/Text Font Sizes", fileName = "J_SO_TextFontSizes", order = 0)]
    public sealed class J_SO_03_FontSizes : ScriptableObject
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField] private float[] _fontSizes = new[] { 32f };
        [BoxGroup("Setup", true, true, 0), SerializeField] private int _defaultSizeIndex = 0;

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public float DefaultSize => _fontSizes[_defaultSizeIndex];

        public float GetSizeFromIndex(int index)
        {
            Assert.IsTrue(_fontSizes.ContainsIndex(index));
            return _fontSizes[index];
        }
    }
}
