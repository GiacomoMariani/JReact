using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JReact.Collections;

namespace JReact
{
    public static class J_CollectionExtensions
    {
        public static bool ContainsIndex(this ICollection collection, int index) => index >= 0 && index < collection.Count;
        public static bool ContainsIndex<T>(this iReactiveIndexCollection<T> collection, int index) => index >= 0 && index < collection.Length;

        public static string PrintAll<T>(this ICollection<T> collection)
            => collection.Aggregate("Elements: - ", (current, element) => current + (element + " - "));
    }
}
