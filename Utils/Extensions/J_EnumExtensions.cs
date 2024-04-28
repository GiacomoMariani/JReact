using System;

namespace JReact
{
    public static class J_EnumExtensions
    {
        // --------------- ENUMS --------------- //
        /// <summary>
        /// retrieves all the values of a given enumerator
        /// </summary>
        /// <returns>all the possible enumerator, as an array</returns>
        public static TEnum[] GetValues<TEnum>() where TEnum : struct => (TEnum[])Enum.GetValues(typeof(TEnum));

        public static int CountValues<TEnum>() where TEnum : struct => Enum.GetValues(typeof(TEnum)).Length;

        /// <summary>
        /// converts a string into an enum
        /// </summary>
        /// <param name="enumString">the string to convert</param>
        /// <param name="caseSensitive">check if we want case sensitive adjustments</param>
        /// <typeparam name="TEnum">the enum we want to parse into</typeparam>
        /// <returns>the string parsed into an enum</returns>
        public static TEnum ToEnum<TEnum>(this string enumString, bool caseSensitive = false)
            => (TEnum)Enum.Parse(typeof(TEnum), enumString, caseSensitive);

        public static TEnum LoopNext<TEnum>(this TEnum enumValue) where TEnum : unmanaged, Enum
        {
            var id   = Convert.ToInt32(enumValue);
            var max  = CountValues<TEnum>();
            var next = (id + 1) % max;
            return ToEnum<TEnum>(next.ToString());

        }
    }
}
