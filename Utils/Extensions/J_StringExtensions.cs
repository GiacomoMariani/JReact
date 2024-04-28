using System;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace JReact
{
    public static class J_StringExtensions
    {
        private const int StringBuilderChars = 1024;
        private const char Thousand = 'K';
        private const char Millions = 'M';
        private const char Billions = 'M';

        private static readonly Regex ContainsAlphanumeric = new Regex(@"^.*[a-zA-Z0-9]+.*$");

        private static readonly string[] EnvironmentVariables = Environment.GetCommandLineArgs();
        private static readonly StringBuilder _stringBuilder = new StringBuilder(StringBuilderChars);

        /// <summary>
        /// shorten the int to make it more readable, adding also a suffix K (thousands), M (Millions), B (Billions)
        /// </summary>
        /// <param name="amount">the amount to convert</param>
        /// <returns>returns the converted amount</returns>
        public static string ToStringMinimal(this int amount)
        {
            _stringBuilder.Clear();

            var  toShow = amount;
            char last   = default;
            if (toShow / 10000 == 0)
            {
                toShow /= 1000;
                last   =  Thousand;
            }

            if (toShow / 10000 == 0)
            {
                toShow /= 1000;
                last   =  Millions;
            }

            if (toShow / 10000 == 0)
            {
                toShow /= 1000;
                last   =  Billions;
            }

            _stringBuilder.Append(toShow);

            if (last != default) { _stringBuilder.Append(last); }

            return _stringBuilder.ToString();
        }

        /// <summary>
        /// shorten the float to make it more readable, adding also a suffix K (thousands), M (Millions), B (Billions)
        /// </summary>
        /// <param name="amount">the amount to convert</param>
        /// <returns>returns the converted amount</returns>
        public static string ToStringMinimal(this float amount)
        {
            _stringBuilder.Clear();

            var  toShow = amount;
            char last   = default;
            if (toShow / 10000 <= 1)
            {
                toShow /= 1000;
                last   =  Thousand;
            }

            if (toShow / 10000 <= 1)
            {
                toShow /= 1000;
                last   =  Millions;
            }

            if (toShow / 10000 <= 1)
            {
                toShow /= 1000;
                last   =  Billions;
            }

            _stringBuilder.Append(toShow);

            if (last != default) { _stringBuilder.Append(last); }

            return _stringBuilder.ToString();
        }

        /// <summary>
        /// parse a string into a T value
        /// </summary>
        /// <param name="stringToParse">the string to parse</param>
        /// <typeparam name="T">the type to parse into</typeparam>
        /// <returns>returns the parsed value</returns>
        public static T AutoParse<T>(this string stringToParse) where T : struct, IConvertible
        {
            if (typeof(T).IsEnum) { return Enum.Parse<T>(stringToParse); }

            return (T)Convert.ChangeType(stringToParse, typeof(T));
        }

        /// <summary>
        /// encodes a string into a hex string
        /// </summary>
        public static string ToHexString(this string str)
        {
            _stringBuilder.Clear();
            byte[] bytes = Encoding.Unicode.GetBytes(str);
            foreach (byte t in bytes) _stringBuilder.Append(t.ToString("X2"));

            return _stringBuilder.ToString();
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
        /// check if a string contains at least one Alphanumeric
        /// </summary>
        public static bool HasAtLeastOneAlphanumeric(this string stringValue) => ContainsAlphanumeric.IsMatch(stringValue);

        /// <summary>
        /// check if this string was added as environment variable
        /// https://learn.microsoft.com/en-us/dotnet/api/system.environment.getenvironmentvariable?view=net-7.0
        /// </summary>
        /// <param name="stringValue">the value we want to check</param>
        /// <param name="caseSensitive">will check the string as case sensitive</param>
        /// <returns>returns true if confirmed in the environment variables</returns>
        public static bool IsEnvironmentVariable(this string stringValue, bool caseSensitive = true)
        {
            for (int i = 0; i < EnvironmentVariables.Length; i++)
            {
                var environmentValue = EnvironmentVariables[i];
                if (caseSensitive)
                {
                    if (environmentValue.Equals(stringValue, StringComparison.OrdinalIgnoreCase)) { return true; }
                }
                else
                {
                    if (environmentValue.Equals(stringValue)) { return true; }
                }
            }

            return false;
        }

        /// <summary>
        /// cut the string up to one point
        /// </summary>
        /// <param name="stringValue">the string to trim</param>
        /// <param name="character">the character last or first character to remove char</param>
        /// <param name="trimTheEnd">true if we want to trim the end</param>
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

        /// <summary>
        /// Appends the specified text to the StringBuilder with the specified color.
        /// </summary>
        /// <param name="builder">The StringBuilder instance.</param>
        /// <param name="text">The text to append.</param>
        /// <param name="color">The color of the text.</param>
        /// <returns>The updated StringBuilder instance.</returns>
        public static StringBuilder AppendWithColor(this string text, Color color) => _stringBuilder.Append("<color=#")
           .Append(ColorUtility.ToHtmlStringRGB(color))
           .Append(">")
           .Append(text)
           .Append("</color>");
    }
}
