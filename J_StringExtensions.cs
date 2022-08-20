using System;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace JReact
{
    public static class J_StringExtensions
    {
        private const char Thousand = 'K';
        private const char Millions = 'M';
        private const char Billions = 'M';

        private static readonly Regex ContainsAlphanumeric = new Regex(@"^.*[a-zA-Z0-9]+.*$");

        /// <summary>
        /// shorten the int to make it more readable, adding also a suffix K (thousands), M (Millions), B (Billions)
        /// </summary>
        /// <param name="amount">the amount to convert</param>
        /// <returns>returns the converted amount</returns>
        public static string ToStringMinimal(this int amount)
        {
            var sb = new StringBuilder();

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

            sb.Append(toShow);

            if (last != default) { sb.Append(last); }

            return sb.ToString();
        }

        /// <summary>
        /// shorten the float to make it more readable, adding also a suffix K (thousands), M (Millions), B (Billions)
        /// </summary>
        /// <param name="amount">the amount to convert</param>
        /// <returns>returns the converted amount</returns>
        public static string ToStringMinimal(this float amount)
        {
            var sb = new StringBuilder();

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

            sb.Append(toShow);

            if (last != default) { sb.Append(last); }

            return sb.ToString();
        }

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
        /// check if a string contains at least one Alphanumeric
        /// </summary>
        public static bool HasAtLeastOneAlphanumeric(this string stringValue) => ContainsAlphanumeric.IsMatch(stringValue);

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
        /// prints the full name of the game object with all the hierarchy in the scene
        /// </summary>
        /// <param name="gameObject">the game object we want to record</param>
        /// <returns>the full hierarchy of the game object</returns>
        public static string FullName(this GameObject gameObject)
        {
            var name = gameObject.name;
            while (gameObject.transform.root != gameObject.transform)
            {
                gameObject = gameObject.transform.parent.gameObject;
                name       = gameObject.name + "=>" + name;
            }

            return name;
        }
    }
}
