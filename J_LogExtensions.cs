using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JReact
{
    public static class J_LogExtensions
    {
        /// <summary>
        /// print all transform up to its root, for debug purposes
        /// </summary>
        public static string PrintAllParents(this Transform transform, string separator = " -> ")
        {
            string transformNames = "";
            while (transform.root != transform)
            {
                transformNames = $"{transformNames}{separator}{transform.gameObject.name}";
                transform      = transform.parent;
            }

            transformNames += $"{separator}{transform.gameObject.name}";
            return transformNames;
        }

        /// <summary>
        /// returns all the items of a collection
        /// </summary>
        public static string PrintAll<T>(this ICollection<T> collection)
            => collection.Aggregate("Elements: - ", (current, element) => current + (element + " - "));

        /// <summary>
        /// returns
        /// </summary>
        public static string PrintAll<T, U>(this Dictionary<T, U> dict, string separator = ", ")
            => dict.Aggregate("", (current, kvp) => current + $"[{kvp.Key}:{kvp.Value}]" + separator);
    }
}
