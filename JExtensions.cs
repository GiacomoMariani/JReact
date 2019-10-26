using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using JReact.StateControl;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace JReact
{
    public static class JExtensions
    {
        #region CONSTANT VALUES
        private const string ScriptableObjectSuffix = "_ScriptableObject";
        #endregion

        #region FLOAT
        /// <summary>
        /// converts a float value into time string
        /// </summary>
        /// <param name="seconds">the time in seconds</param>
        public static string SecondsToString(this float seconds)
        {
            TimeSpan time = TimeSpan.FromSeconds(seconds);
            //backslash tells that colon is not the part of format, it just a character that we want in output
            return time.SpanToStringTwo();
        }
        #endregion FLOAT

        #region PERCENTAGE
        /// <summary>
        /// the float will be used as a chance
        /// </summary>
        /// <param name="chance">the desired float should be between 0f and 1f</param>
        /// <returns>returns true if the chance happens</returns>
        public static bool ChanceHit(this float chance)
        {
            Assert.IsTrue(chance > 0,  $"{chance} is lower or equal to 0. So it will always be false");
            Assert.IsTrue(chance < 1f, $"{chance} is higher or equal to 1. So it will always be  true");
            return Random.Range(0, 1f) <= chance;
        }

        /// <summary>
        /// converts an axis (-1f to 1f) to a byte
        /// </summary>
        /// <param name="axisFloat">the value to compress</param>
        /// <returns>the byte</returns>
        public static byte ToByte(this float axisFloat)
        {
            // --------------- OUTER CASE --------------- //
            if (axisFloat > 1.0f)
            {
                JLog.Warning($"Percentage {axisFloat} is higher than 1. Setting to 1");
                return 100;
            }

            if (axisFloat < -1.0f)
            {
                JLog.Warning($"Percentage {axisFloat} is lower than -1 Setting to -1");
                return 101;
            }

            //positive
            if (axisFloat >= 0) return (byte) (axisFloat * 100);
            //negative
            return (byte) (201 + axisFloat * 100);
        }

        /// <summary>
        /// deconverts a byt to axis
        /// </summary>
        /// <param name="axisByte">the byte to deconvert</param>
        /// <returns>returns the axis</returns>
        public static float ToAxis(this byte axisByte)
        {
            if (axisByte <= 100) return axisByte * 0.01f;
            return axisByte                      * 0.01f - 2.01f;
        }
        #endregion PERCENTAGE

        #region INT
        /// <summary>
        /// sums an integer and make sure it circles between some values 
        /// </summary>
        /// <param name="element">the element to be changed</param>
        /// <param name="toAdd">the element we want to add</param>
        /// <param name="roundMax">the max</param>
        public static int SumRound(this int element, int toAdd, int roundMax) => (element + toAdd) % roundMax;
        #endregion INT

        #region ENUMS
        /// <summary>
        /// retrieves all the values of a given enumerator
        /// </summary>
        /// <returns>all the possible enumerator, as an array</returns>
        public static TEnum[] GetValues<TEnum>() where TEnum : struct => (TEnum[]) Enum.GetValues(typeof(TEnum));

        public static int CountValues<TEnum>() where TEnum : struct => Enum.GetValues(typeof(TEnum)).Length;
        
        /// <summary>
        /// converts a string into an enum
        public static T ToEnum<T>(this string enumString, bool caseSensitive = false)
            => (T) Enum.Parse(typeof(T), enumString, caseSensitive);
        #endregion

        #region ARRAYS
        /// <summary>
        /// checks if an array contains a given item
        /// </summary>
        /// <param name="array">the array to check</param>
        /// <param name="itemToCheck">the item we want to find</param>
        /// <returns>returns true if the array contains the item</returns>
        public static bool ArrayContains<T>(this T[] array, T itemToCheck) => Array.IndexOf(array, itemToCheck) > -1;

        public static bool ArrayIsValid<T>(this T[] array) => array != null && array.Length > 0;
        
        public static T[] SubArray<T>(this T[] data, int index, int length)
        {
            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }

        public static T[] AddItemToArray<T>(this IEnumerable<T> data, T item)
        {
            List<T> tempList = data.ToList();
            tempList.Add(item);
            return tempList.ToArray();
        }
        #endregion ARRAYS

        #region COLLECTIONS
        /// <summary>
        /// get a random element in the collection
        /// </summary>
        public static T GetRandomElement<T>(this ICollection<T> collection) => collection.ElementAt(Random.Range(0, collection.Count));

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
        #endregion COLLECTIONS

        #region SCRIPTABLE OBJECTS
        //a way to set the names of scriptable object
        public static void SetName(this ScriptableObject item, string newName) =>item.name = newName + ScriptableObjectSuffix;
        
        #if UNITY_EDITOR
        public static T GetOrCreateAtPath<T>(string folder, string assetName, string assetType = ".asset", bool createPathIfMissing = false)
            where T : ScriptableObject
        {
            var path  = Path.Combine(folder, assetName + assetType);
            if (createPathIfMissing) Directory.CreateDirectory(folder);

            var asset = AssetDatabase.LoadAssetAtPath<T>(path);
            //create ingredient asset at path
            if (asset == null)
            {
                JLog.Log($"Creating asset {assetName} at path {path}");
                asset = ScriptableObject.CreateInstance<T>();
                AssetDatabase.CreateAsset(asset, path);
                JLog.Log($"Asset created {assetName}");
            }

            return asset;
        }
        #endif
        #endregion SCRIPTABLE OBJECTS

        #region TRANSFORMS
        /// <summary>
        /// removes all children of a transform
        /// </summary>
        public static void ClearTransform(this Transform transform)
        {
            foreach (Transform child in transform) Object.Destroy(child.gameObject);
        }
        
        public static void ClearTransformImmediate(this Transform transform)
        {
            foreach (Transform child in transform) Object.DestroyImmediate(child.gameObject);
        }

        /// <summary>
        /// print all transform up to its root, for debug purposes
        /// </summary>
        public static string PrintAllParents(this Transform transform)
        {
            string transformNames = "";
            while (transform.root != transform)
            {
                transformNames = transformNames + " -> " + transform.gameObject.name;
                transform      = transform.parent;
            }

            transformNames = transformNames + transform.gameObject.name;
            return transformNames;
        }
        #endregion TRANSFORMS

        #region RECT TRANSFORM
        /// <summary>
        /// make this transform as large as the parent
        /// </summary>
        public static void FitParent(this RectTransform rectTransform)
        {
            Assert.IsTrue(rectTransform.GetComponentInParent<RectTransform>(),
                          $"{rectTransform.name} parent ({rectTransform.parent.name}) is not a valid");

            rectTransform.anchorMin = JConstants.VectorZero;
            rectTransform.anchorMax = JConstants.VectorOne;
            rectTransform.offsetMin = JConstants.VectorZero;
            rectTransform.offsetMax = JConstants.VectorOne;
        }

        /// <summary>
        /// returns the screen position of the given rect
        /// </summary>
        public static Vector2 ToScreenPosition(this RectTransform rectTransform, Camera camera)
            => RectTransformUtility.WorldToScreenPoint(camera, rectTransform.transform.position);
        #endregion RECT TRANSFORM

        #region COMPONENT
        /// <summary>
        /// inject directly the element
        /// </summary>
        /// <param name="component">must be a component to inject the element</param>
        /// <param name="alsoDisabled">injects also in disabled children</param>
        public static void InjectToChildren<T>(this T component, bool alsoDisabled = true)
            where T : Component
        {
            component.InjectElementToChildren(component, alsoDisabled);
        }

        /// <summary>
        /// inject an element into all children
        /// </summary>
        /// <param name="component">the component with children requiring injection</param>
        /// <param name="element">the element to inject</param>
        /// <param name="alsoDisabled">injects also in disabled children</param>
        public static void InjectElementToChildren<T>(this Component component, T element, bool alsoDisabled = true)
        {
            iInitiator<T>[] elementThatRequireThis = component.GetComponentsInChildren<iInitiator<T>>(alsoDisabled);
            for (int i = 0; i < elementThatRequireThis.Length; i++) elementThatRequireThis[i].InjectThis(element);
        }
        #endregion COMPONENT

        #region GAMEOBJECT
        public static void ActivateAll(this GameObject[] gameObjects, bool activation)
        {
            for (int i = 0; i < gameObjects.Length; i++) gameObjects[i].SetActive(activation);
        }

        /// <summary>
        /// checks if the elements is a prefab or a scene game object
        /// </summary>
        /// <param name="a_Object">the element to check</param>
        /// <returns>true if this is a prefab, false if this is a gameobject</returns>
        public static bool IsPrefab(this GameObject a_Object) => a_Object.scene.rootCount == 0;

        /// <summary>
        /// a method to check if a gameobject has a component
        /// </summary>
        /// <param name="component"></param>
        /// <param name="gameObjectToCheck"></param>
        public static void CheckComponent(this GameObject gameObjectToCheck, Type component)
        {
            //check one component ont he weapon
            Component[] elementSearched = gameObjectToCheck.GetComponents(component);

            //check if we have at least a component
            Assert.IsFalse(elementSearched.Length == 0,
                           $"There is no {component} components on {gameObjectToCheck.name}");

            //check that we have just one component
            Assert.IsFalse(elementSearched.Length > 1,
                           $"There are too many components of {component} on {gameObjectToCheck.name}");

            //check that the component is of the specified class
            if (elementSearched.Length > 0)
                Assert.IsTrue(elementSearched[0].GetType() == component.GetElementType(),
                              $"The class requested is of a parent class. Weapon {gameObjectToCheck}, class found {elementSearched[0].GetType()}, class requested {component.GetElementType()}. Player {gameObjectToCheck.transform.root.gameObject}");
        }
        #endregion GAMEOBJECT

        #region VECTORS
        /// <summary>
        /// used to have a random float value between 2 data given in a Vector2
        /// </summary>
        /// <param name="range">x is the min and y is the max</param>
        /// <returns></returns>
        public static float GetRandomValue(this Vector2 range)
        {
            Assert.IsTrue(range.x <= range.y,
                          $"Y (max) = {range.y} needs to be be higher than X(MIN) = {range.x}, ");

            return Random.Range(range.x, range.y);
        }

        /// <summary>
        /// Gets a random value between vector.x and vector.y
        /// </summary>
        /// <param name="rangeInt">the vector with the min and max</param>
        public static int GetRandomValue(this Vector2Int rangeInt)
        {
            Assert.IsTrue(rangeInt.x <= rangeInt.y,
                          $"Y (max) = {rangeInt.y} needs to be be higher than X(MIN) = {rangeInt.x}, ");

            return Random.Range(rangeInt.x, rangeInt.y);
        }

        public static Direction GetDirection(this Vector2 force)
        {
            // --------------- STOPPED WIND --------------- //
            //if the wind is at 0 it is stopped
            if (Math.Abs(force.x) < JConstants.GeneralFloatTolerance &&
                Math.Abs(force.y) < JConstants.GeneralFloatTolerance) return Direction.None;

            //find if the top most intensity is vertical or horizontal
            // --------------- HORIZONTAL --------------- //
            if (Mathf.Abs(force.x) > Mathf.Abs(force.y))
                return force.x >= 0
                           ? Direction.Right
                           : Direction.Left;

            // --------------- VERTICAL --------------- //
            return force.y >= 0
                       ? Direction.Up
                       : Direction.Down;
        }
        #endregion VECTORS

        #region STRING
        public static int ToInt(this string stringToConvert)
        {
            if (int.TryParse(stringToConvert, out int valueToReturn)) return valueToReturn;

            Debug.LogWarning($"The string '{stringToConvert}' cannot be converted into integer. Returning 0.");
            return 0;
        }

        public static float ToFloat(this string stringToConvert)
        {
            if (float.TryParse(stringToConvert, out float valueToReturn)) return valueToReturn;

            Debug.LogWarning($"The string '{stringToConvert}' cannot be converted into float. Returning 0f.");
            return 0f;
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
        #endregion STRING

        #region DATE TIME and DATE SPAN
        /// <summary>
        /// this is used to calculate the seconds passed between 2 date times
        /// </summary>
        /// <param name="currentTime">the current date time</param>
        /// <param name="previousTime">the time passed from the previous time</param>
        /// <returns>returns the seconds passed in this time interval</returns>
        public static double CalculateSecondsFrom(this DateTime currentTime, DateTime previousTime)
        {
            //calculate the time passed
            TimeSpan passedTime = currentTime.Subtract(previousTime);
            //return the seconds passed
            return passedTime.TotalSeconds;
        }

        /// <summary>
        /// converts a date time to unix time
        /// </summary>
        /// <param name="dateTime">the date time to convert</param>
        /// <returns>the unix time, converted</returns>
        public static int GetUnixTimeStamp(this DateTime dateTime)
        {
            var epochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            int unixTime   = (int) (dateTime - epochStart).TotalSeconds;
            JLog.Log($"Current time unix = {unixTime}");
            return unixTime;
        }

        /// <summary>
        /// given a span it returns a string with the 2 highest values
        /// </summary>
        public static string SpanToStringTwo(this TimeSpan span,      string day       = "d", string hour = "h", string min = "m",
                                             string        sec = "s", string separator = ":")
        {
            if (span.Days  > 0) return $"{span:%d}{day}{separator}{span:%h}{hour}";
            if (span.Hours > 0) return $"{span:%h}{hour}{separator}{span:%m}{min}";
            return $"{span:%m}{min}{separator}{span:%s}{sec}";
        }
        #endregion

        #region 2D
        /// <summary>
        /// used to set a transparency on a given sprite renderer
        /// </summary>
        /// <param name="spriteRenderer">the sprite renderer to adjust</param>
        /// <param name="transparency">the transparency we want to set</param>
        public static void SetTransparency(this SpriteRenderer spriteRenderer, float transparency)
        {
            //sanity check
            Assert.IsTrue(transparency >= 0f && transparency <= 1.0f,
                          $"The transparency to be set on {spriteRenderer.gameObject.name} should be between 0 and 1. Received value: {transparency}");

            //clamp the value
            transparency = Mathf.Clamp(transparency, 0f, 1f);
            //get the color
            Color fullColor = spriteRenderer.color;
            //set the color with transparency
            spriteRenderer.color = new Color(fullColor.r, fullColor.g, fullColor.b, transparency);
        }

        /// <summary>
        /// used to set a transparency on a given image
        /// </summary>
        /// <param name="image">the image to adjust</param>
        /// <param name="transparency">the transparency we want to set</param>
        public static void SetTransparency(this Image image, float transparency)
        {
            //sanity check
            Assert.IsTrue(transparency >= 0f && transparency <= 1.0f,
                          $"The transparency to be set on {image.gameObject.name} should be between 0 and 1. Received value: {transparency}");

            //clamp the value
            transparency = Mathf.Clamp(transparency, 0f, 1f);
            //get the color
            Color fullColor = image.color;
            //set the color with transparency
            image.color = new Color(fullColor.r, fullColor.g, fullColor.b, transparency);
        }
        #endregion
    }
}
