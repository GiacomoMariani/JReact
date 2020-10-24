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
    }
}
