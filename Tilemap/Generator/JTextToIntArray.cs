using System;
using UnityEngine;

namespace JReact.Tilemaps.Generator
{
    public static class JTextToIntArray
    {
        private const char _Separator = ',';

        public static int[] ToIntArray(this TextAsset textToConvert,        out int width, bool reverseRows = true,
                                       bool           reverseLines = false, char    separator = _Separator)
        {
            width = textToConvert.ConvertString().RemoveEmptyLines().GetFirstLine().SplitWith(_Separator).Length - 1;

            string[] chars = textToConvert.ConvertString().TrimSpace().RemoveEndLine().SplitWith(_Separator);

            int[] result = new int[chars.Length];
            for (int i = 0; i < chars.Length; i++) { result[i] = chars[i].ToInt(); }

            if (reverseRows) { ReverseRows(ref result, width); }

            if (reverseLines) { ReverseLines(ref result, width); }

            return result;
        }

        private static void ReverseRows(ref int[] array, int width)
        {
            if (array.Length % width != 0)
            {
                JLog.Warning($"Given {nameof(array)} of L {array.Length} is not divisible for {nameof(width)}({width}). Map could have not enough columns");
            }

            var numberOfRows = array.Length / width;
            for (int i = 0; i < numberOfRows / 2; i++)
            {
                var start = i                      * width;
                var end   = (numberOfRows - i - 1) * width;

                var temp = new int[width];
                Array.Copy(array, start, temp,  0,     width);
                Array.Copy(array, end,   array, start, width);
                Array.Copy(temp,  0,     array, end,   width);
            }
        }

        private static void ReverseLines(ref int[] result, int width) { Array.Reverse(result); }
    }
}
