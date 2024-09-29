using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JReact.Collections;
using JReact.StateControl;
using Sirenix.Utilities;
using Unity.Mathematics;
using UnityEngine.Assertions;

namespace JReact
{
    public static class J_CollectionExtensions
    {
        // --------------- ARRAYS --------------- //
        /// <summary>
        /// checks if an array contains a given item
        /// </summary>
        /// <param name="array">the array to check</param>
        /// <param name="itemToCheck">the item we want to find</param>
        /// <returns>returns true if the array contains the item</returns>
        public static bool ArrayContains<T>(this T[] array, T itemToCheck) => Array.IndexOf(array, itemToCheck) > -1;

        public static bool ArrayIsValid<T>(this T[] array) => array != null && array.Length > 0;

        /// <summary>
        /// iterates an enumerable using an action
        /// </summary>
        /// <param name="enumerable">the collection as enumerable</param>
        /// <param name="action">the action to process the items</param>
        /// <typeparam name="T">the type to process</typeparam>
        public static void Iterate<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            using var enumerator = enumerable.GetEnumerator();
            while (enumerator.MoveNext()) { action.Invoke(enumerator.Current); }
        }

        public static bool ValidIndex<T>(this T[] array, int index) => index >= 0 && index < array.Length;

        public static T[] SubArray<T>(this T[] data, int index, int length)
        {
            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }

        public static T[] ConverToArray<T>(this IEnumerable<T> data, T item)
        {
            List<T> tempList = data.ToList();
            tempList.Add(item);
            return tempList.ToArray();
        }

        public static int SafeLength<T>(this T[] array, T item) => array?.Length ?? 0;

        public static List<T> AddToList<T>(this List<T> list, T[] added)
        {
            int amountToAdd = added.Length;

            for (int i = 0; i < amountToAdd; i++) { list.Add(added[i]); }

            return list;
        }

        public static List<T> AddUniques<T>(this List<T> list, IList<T> added)
        {
            Assert.IsNotNull(list, $"requires a {nameof(list)}");
            if (added == null) { return list; }

            int amountToAdd = added.Count;
            for (int i = 0; i < amountToAdd; i++)
            {
                if (!list.Contains(added[i])) { list.Add(added[i]); }
            }

            return list;
        }

        public static int SafeCount<T>(this IList<T> list) => list?.Count ?? 0;

        public static void RemoveSafe<T>(this IList<T> list, T item)
        {
            if (list.Contains(item)) { list.Remove(item); }
        }

        public static void Shuffle<T>(this IList<T> list, uint seed = 1)
        {
            var random = new Unity.Mathematics.Random(seed);
            int totals = list.Count;
            for (int i = 0; i < totals; i++)
            {
                int j = random.NextInt(0, i + 1);
                (list[j], list[i]) = (list[i], list[j]);
            }
        }

        public static void RemoveSafe<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey item)
        {
            if (dict.ContainsKey(item)) { dict.Remove(item); }
        }

        public static int SafeCount<TKey, TValue>(this Dictionary<TKey, TValue> dict) => dict?.Count ?? 0;

        /// <summary>
        /// converts an int2 coordinates with the index of an linear 2d array
        /// </summary>
        /// <param name="array">the linear 2d array</param>
        /// <param name="coord">the coordinates of the array</param>
        /// <param name="width">the width of the array</param>
        /// <returns>returns the index of the linear array</returns>
        public static T GetAtCoordinate<T>(this T[] array, int2 coord, int width) => array.GetAtCoordinate(coord.x, coord.y, width);

        /// <summary>
        /// gets the item of a 2d linear array from its coordinates
        /// </summary>
        /// <param name="array">the linear 2d array</param>
        /// /// <param name="x">the x coordinate of the array</param>
        /// <param name="y">the y coordinate of the array</param>
        /// <param name="width">the width of the array</param>
        /// <returns>returns the index of the linear array</returns>
        public static T GetAtCoordinate<T>(this T[] array, int x, int y, int width)
        {
            var index = y * width + x;
            Assert.IsTrue(array.ContainsIndex(index), $"Index {index} not valid for array[{array.Length}]\n" +
                                                      $"Coordinates {x}, {y} - width {width}");

            return array[index];
        }

        /// <summary>
        /// cycle the index of an array given a value
        /// </summary>
        /// <param name="array">the array we want to index</param>
        /// <param name="rawIndex">the raw index we want to check</param>
        /// <returns>returns a valid index in the array given its raw index</returns>
        public static int CycleIndex<T>(this T[] array, int rawIndex)
        {
            if (rawIndex < 0)
            {
                rawIndex %= array.Length;
                return array.Length + rawIndex;
            }
            else { return rawIndex % array.Length; }
        }

        // --------------- COLLECTIONS --------------- //
        public static bool ContainsIndex(this ICollection collection, int index) => index >= 0 && index < collection.Count;

        public static bool ContainsIndex<T>(this iReactiveIndexCollection<T> collection, int index)
            => index >= 0 && index < collection.Length;

        public static void SubscribeToAll<T>(this IEnumerable<T> collection, Action actionToPerform)
            where T : jObservable
        {
            foreach (T element in collection) element.Subscribe(actionToPerform);
        }

        public static void UnSubscribeToAll<T>(this IEnumerable<T> collection, Action actionToPerform)
            where T : jObservable
        {
            foreach (T element in collection) element.UnSubscribe(actionToPerform);
        }

        public static void SubscribeToAllEnd<T>(this IEnumerable<T> collection, Action action)
            where T : J_State
        {
            foreach (T element in collection) element.SubscribeToEnd(action);
        }

        public static void UnSubscribeToAllEnd<T>(this IEnumerable<T> collection, Action action)
            where T : J_State
        {
            foreach (T element in collection) element.UnSubscribeToEnd(action);
        }

        public static void SubscribeToAll<T>(this IEnumerable<jObservable<T>> collection, Action<T> actionToPerform)
        {
            foreach (jObservable<T> element in collection) element.Subscribe(actionToPerform);
        }

        public static void UnSubscribeToAll<T>(this IEnumerable<jObservable<T>> collection, Action<T> actionToPerform)
        {
            foreach (jObservable<T> element in collection) element.UnSubscribe(actionToPerform);
        }

        public static void ActivateAll(this IEnumerable<J_Service> services)
        {
            foreach (J_Service element in services) element.Activate();
        }

        public static void UnSubscribeToAll<T>(this IEnumerable<J_Service> services)
        {
            foreach (J_Service element in services) element.End();
        }

        public static void ResetAll(IEnumerable<iResettable> collection)
        {
            foreach (iResettable element in collection) element.ResetThis();
        }
    }
}
