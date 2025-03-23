using System;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

namespace JReact
{
    public static class J_Text_Extensions
    {
        private static readonly string[] _EndLines = new[] { "\r", "\r\n", "\n" };

        public static void ShowTextOrNull(this TextMeshProUGUI textUi, string textStr)
        {
            if (!textStr.IsEmptyOrNull())
            {
                textUi.gameObject.SetActive(true);
                textUi.text = textStr;
            }
            else { textUi.gameObject.SetActive(false); }
        }

        public static string ConvertString(this TextAsset textToConvert) => textToConvert.text;

        public static string[] SplitLines(this string stringToConvert) => stringToConvert.Split(_EndLines, StringSplitOptions.RemoveEmptyEntries);
        public static string[] SplitWith(this string stringToConvert, char splitWith) => stringToConvert.Split(splitWith);

        public static string TrimSpace(this string stringToConvert) => stringToConvert.Replace(" ", "");

        public static string RemoveEmptyLines(this string stringToConvert)
            => Regex.Replace(stringToConvert, @"^\s+$[\r\n]*", string.Empty, RegexOptions.Multiline);

        public static string RemoveEndLine(this string stringToConvert)
        {
            for (int i = 0; i < _EndLines.Length; i++) stringToConvert = stringToConvert.Replace(_EndLines[i], "");

            return stringToConvert;
        }

        public static string GetLine(this string stringToConvert, int lineIndex) => GetAllLines(stringToConvert)[lineIndex];

        public static string GetFirstLine(this string stringToConvert) => GetLine(stringToConvert, 0);

        public static string[] GetAllLines(this string inputString)
            => inputString.Split(new[] { "\r", "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
    }
}
