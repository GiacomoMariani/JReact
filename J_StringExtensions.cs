using System.Text;
using System.Text.RegularExpressions;

namespace JReact
{
    public static class J_StringExtensions
    {
        /// <summary>
        /// encodes a string into a hex string
        /// </summary>
        public static string ToHexString(this string str)
        {
            var sb = new StringBuilder();

            byte[] bytes = Encoding.Unicode.GetBytes(str);
            foreach (byte t in bytes) sb.Append(t.ToString("X2"));

            return sb.ToString();
        }

        /// <summary>
        /// removes all non alpha numeric elements.
        /// </summary>
        public static string ToAlphaNumeric(this string str, bool includeSpace = true)
        {
            Regex rgx;
            rgx = includeSpace
                      ? new Regex("[^a-zA-Z0-9 -]")
                      : new Regex("[^a-zA-Z0-9-]");

            str = rgx.Replace(str, "");

            return str;
        }

        /// <summary>
        /// check if a string is null or empty
        /// </summary>
        public static bool IsEmptyOrNull(this string stringValue) => string.IsNullOrEmpty(stringValue);

        /// <summary>
        /// cut the string up to one point
        /// </summary>
        /// <param name="stringValue">the string to trim</param>
        /// <param name="character">the character last or first character to remove char</param>
        /// <param name="forward">true if we want to trim the end</param>
        /// <returns></returns>
        public static string TrimString(this string stringValue, char character, bool trimTheEnd = true)
        {
            int index = stringValue.LastIndexOf(character);
            if (index > 0)
            {
                return trimTheEnd ? stringValue.Substring(0, index) : stringValue.Substring(index, stringValue.Length - 1);
            }
            else { return stringValue; }
        }
    }
}
