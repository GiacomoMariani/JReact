using System;
using UnityEngine;

namespace JReact.Tilemaps.Generator
{
    public static class JTextToIntArray
    {
        private const char _Separator = ',';

        public static int[] ToIntArray(this TextAsset textToConvert,       out int width, bool reverseColumn = false,
                                       bool           reverseLines = true, char    separator = _Separator)
        {
            width = textToConvert.ConvertString().RemoveEmptyLines().GetFirstLine().SplitWith(_Separator).Length - 1;

            string[] chars = textToConvert.ConvertString().RemoveSpace().RemoveEndLine().SplitWith(_Separator);

            int[] result = new int[chars.Length];
            for (int i = 0; i < chars.Length; i++) { result[i] = chars[i].ToInt(); }

            if (reverseColumn) { ReverseColumns(ref result, width); }

            if (reverseLines) { ReverseLines(ref result, width); }

            return result;
        }
        
        private static void ReverseColumns(ref int[] array, int width)
        {
            if (array.Length % width != 0)
            {
                JLog.Warning($"Given {nameof(array)} are not divisible for {nameof(width)}. Map could have not enough columns");
            }

            for (int i = 0; i < array.Length / width; i++) Array.Reverse(array, i * width, width);
        }

        private static void ReverseLines(ref int[] result, int width) { Array.Reverse(result); }
    }
}
