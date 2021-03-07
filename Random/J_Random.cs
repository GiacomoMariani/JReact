using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact
{
    public static class J_Random
    {
        private static double? _storedRandom;

        public static double NextGaussian(this System.Random random, float median = 0f, float sDeviation = 1f)
        {
            if (median <= 0) throw new ArgumentOutOfRangeException(nameof(sDeviation), "Must be >= 0.");

            if (_storedRandom.HasValue)
            {
                double stored = _storedRandom.Value;
                _storedRandom = null;
                return stored;
            }

            double r1, r2, s;
            do
            {
                r1 = 2d * random.NextDouble() - 1d;
                r2 = 2d * random.NextDouble() - 1d;
                s  = r1 * r1                  + r2 * r2;
            }
            while (s >= 1d ||
                   s == 0d);

            double polar = Math.Sqrt(-2 * Math.Log(s) / s);
            _storedRandom = r2 * polar;
            return r1          * polar * sDeviation + median;
        }

        public static void Shuffle<T>(this System.Random random, IList<T> list)
        {
            int totals = list.Count;
            for (int i = 0; i < totals; i++)
            {
                int j = random.Next(0, i + 1);

                T temp = list[j];
                list[j] = list[i];
                list[i] = temp;
            }
        }

        public static T GetRandomElement<T>(this IList<T> list) => list.ElementAt(UnityEngine.Random.Range(0, list.Count));

        public static T GetRandomElement<T>(this T[] array) => array.ElementAt(UnityEngine.Random.Range(0, array.Length));

        /// <summary>
        /// used to have a random float value between 2 data given in a Vector2
        /// </summary>
        /// <param name="range">x is the min and y is the max</param>
        /// <returns></returns>
        public static float GetRandomValue(this Vector2 range)
        {
            Assert.IsTrue(range.x <= range.y,
                          $"Y (max) = {range.y} needs to be be higher than X(MIN) = {range.x}, ");

            return UnityEngine.Random.Range(range.x, range.y);
        }

        /// <summary>
        /// Gets a random value between vector.x and vector.y
        /// </summary>
        /// <param name="rangeInt">the vector with the min and max</param>
        public static int GetRandomValue(this Vector2Int rangeInt)
        {
            Assert.IsTrue(rangeInt.x <= rangeInt.y,
                          $"Y (max) = {rangeInt.y} needs to be be higher than X(MIN) = {rangeInt.x}, ");

            return UnityEngine.Random.Range(rangeInt.x, rangeInt.y);
        }

        /// <summary>
        /// the float will be used as a chance
        /// </summary>
        /// <param name="chance">the desired float should be between 0f and 1f</param>
        /// <returns>returns true if the chance happens</returns>
        public static bool ChanceSuccess(this float chance)
        {
            Assert.IsTrue(chance >= 0, $"{chance} is lower or equal to 0. So it will always be false");
            Assert.IsTrue(chance < 1f, $"{chance} is higher or equal to 1. So it will always be  true");
            return UnityEngine.Random.Range(0, 1f) <= chance;
        }

        /// <summary>
        /// the int will be used as a chance
        /// </summary>
        /// <param name="chance">the desired int should be between 0 and 100</param>
        /// <returns>returns true if the chance happens</returns>
        public static bool ChanceSuccess(this int chance)
        {
            Assert.IsTrue(chance >= 0,   $"{chance} is lower to 0. So it will always be false");
            Assert.IsTrue(chance <= 100, $"{chance} is higher to 100. So it will always be  true");
            return UnityEngine.Random.Range(0, 101) <= chance;
        }

        /// <summary>
        /// gives a random color, with full alpha
        /// </summary>
        /// <returns>returns a random color</returns>
        public static Color GetRandomColor() => new Color(UnityEngine.Random.Range(0f, 1f),
                                                          UnityEngine.Random.Range(0f, 1f),
                                                          UnityEngine.Random.Range(0f, 1f), 1f);
    }
}
