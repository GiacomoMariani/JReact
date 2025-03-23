using System;
using JReact;
using TMPro;
using UnityEngine;

namespace Jreact.UiViewMono.TextSettings
{
    [CreateAssetMenu(menuName = "Reactive/UiCustomization/Text Size", fileName = "J_SO_TextSize", order = 0)]
    public class J_SO_TextSize : ScriptableObject
    {
        [SerializeField] private JeTextSizeType _sizeType;
        public JeTextSizeType SizeType => _sizeType;
        public J_SO_TextFont[] Fonts;
        public float[] FontSizes;

        public bool SanityCheck()
        {
            if (!Fonts.ArrayIsValid()) { return false; }

            if (!FontSizes.ArrayIsValid()) { return false; }

            if (Fonts.Length != FontSizes.Length) { return false; }

            return true;
        }

        public bool HasFontSize(J_SO_TextFont fontType, out float fontSize)
        {
            int index = Array.IndexOf(Fonts, fontType);
            if (index == -1)
            {
                fontSize = 0;
                return false;
            }
            else
            {
                fontSize = FontSizes[index];
                return true;
            }
        }
    }
}
