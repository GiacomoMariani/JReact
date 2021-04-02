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

        public static bool ValidIndex<T>(this T[] array, int index) => array != null && index < array.Length && index > 0;

        public static T[] SubArray<T>(this T[] data, int index, int length)
        {
            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }

        public static T[] AddItemToList<T>(this IEnumerable<T> data, T item)
        {
            List<T> tempList = data.ToList();
            tempList.Add(item);
            return tempList.ToArray();
        }

        public static List<T> AddUniques<T>(this List<T> list, IList<T> added)
        {
            Assert.IsFalse(list.IsNullOrEmpty(),  $"Not valid list {nameof(list)}");
            Assert.IsFalse(added.IsNullOrEmpty(), $"Not valid added {nameof(added)}");
            int amountToAdd = added.Count;
            for (int i = 0; i < amountToAdd; i++)
            {
                if (!list.Contains(added[i])) { list.Add(added[i]); }
            }

            return list;
        }

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
