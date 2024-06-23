using System;
using UnityEngine;

namespace JReact
{
    public static class J_StringUtils
    {
        public const string AlphaNumeric = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

        public static void CopyToClipboard(this string value) => GUIUtility.systemCopyBuffer = value;
        public static string FromToClipboard(ref string value) => value = GUIUtility.systemCopyBuffer;

        /// <summary>
        /// generates a random string of the desired length from a set of valid characters
        /// </summary>
        /// <param name="characterAmount">the lenght required for this string</param>
        /// <param name="validCharacters">the valid characters to create the string</param>
        /// <returns>a random string of the requested length using the requested characters</returns>
        public static string GenerateRandomString(int characterAmount, string validCharacters = AlphaNumeric)
        {
            var stringResult          = new char[characterAmount];
            var validCharactersLength = validCharacters.Length;
            var random                = new System.Random();

            for (int i = 0; i < characterAmount; i++) { stringResult[i] = validCharacters[random.Next(validCharactersLength)]; }

            return new string(stringResult);
        }
        
    }
}
