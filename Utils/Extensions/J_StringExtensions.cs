using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Text.RegularExpressions;
using Cysharp.Text;
using JReact.SaveSystem;
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
            if (index > 0) { return trimTheEnd ? stringValue[..index] : stringValue.Substring(index, stringValue.Length - 1); }
            else { return stringValue; }
        }

        /// <summary>
        /// Appends the specified text to the StringBuilder with the specified color.
        /// </summary>
        /// <param name="builder">The StringBuilder instance.</param>
        /// <param name="text">The text to append.</param>
        /// <param name="color">The color of the text.</param>
        /// <returns>The updated StringBuilder instance.</returns>
        public static StringBuilder AppendWithColor(this string text, Color color) => _stringBuilder.Append("<color=#").
            Append(ColorUtility.ToHtmlStringRGB(color)).
            Append(">").
            Append(text).
            Append("</color>");

        /// <summary>
        /// Replaces all occurrences of the specified key in the given string value with a new key.
        /// </summary>
        /// <param name="value">The string value to perform the replacement on.</param>
        /// <param name="oldKey">The key to replace.</param>
        /// <param name="newKey">The new key to replace with.</param>
        /// <returns>The resulting string with all occurrences of the old key replaced with the new key.</returns>
        public static string ReplaceKeys(this string value, string oldKey, string newKey)
        {
            const string FormatPattern = "\"{0}\"";
            oldKey = ZString.Format(FormatPattern, oldKey);
            newKey = ZString.Format(FormatPattern, newKey);
            return value.Replace(oldKey, newKey);
        }

        // --------------- STRING COMPRESS --------------- //
        /// <summary>
        /// Compresses a string using byte compression and encryption.
        /// </summary>
        /// <param name="source">The string to compress.</param>
        /// <param name="password">The password used for encryption.</param>
        /// <returns>The compressed and encrypted string as a base64-encoded string.</returns>
        public static string CompressString(this string source, string password = default)
        {
            byte[] serializedBytes = source.ConvertToBinary();
            byte[] compressedBytes = JByteCompression.Compress(serializedBytes);
            if (password == default) return Convert.ToBase64String(compressedBytes);
            byte[] encryptedBytes = JByteEncryption.DefaultEncrypt(compressedBytes, password);
            return Convert.ToBase64String(encryptedBytes);
        }

        /// <summary>
        /// Decompresses a string using base64 decoding, AES decryption, and byte decompression.
        /// </summary>
        /// <param name="rawData">The base64-encoded, AES-encrypted, and compressed string to decompress.</param>
        /// <param name="password">The password used for AES decryption.</param>
        /// <returns>The decompressed string.</returns>
        private static string DeCompressString(string rawData, string password = default)
        {
            byte[] encryptedBytes  = Convert.FromBase64String(rawData);
            byte[] compressedBytes = password != default ? JByteEncryption.DefaultDecrypt(encryptedBytes, password) : encryptedBytes;
            byte[] serializedBytes = JByteCompression.Decompress(compressedBytes);
            return serializedBytes.ConvertToString();
        }

        // --------------- BINARY STRINGS --------------- //
        /// <summary>
        /// Converts a string to binary representation.
        /// </summary>
        /// <param name="source">The string to convert.</param>
        /// <returns>The binary representation of the string as a byte array.</returns>
        public static byte[] ConvertToBinary(this string source) => Encoding.UTF8.GetBytes(source);

        /// <summary>
        /// Converts an integer value to a string representation with a shortened format, adding suffixes for thousands (K), millions (M), and billions (B).
        /// </summary>
        /// <returns>The string representation of the converted amount.</returns>
        public static string ConvertToString(this byte[] data) => Encoding.UTF8.GetString(data);
    }
}
