using System;
using System.IO;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using Object = UnityEngine.Object;

namespace JReact
{
    public static class JExtensions
    {
        // --------------- CONSTANT VALUES --------------- //
        private const string ScriptableObjectSuffix = "_ScriptableObject";

        // --------------- GENERAL --------------- //
        // [StructLayout(LayoutKind.Explicit)]
        // private struct Converter<TFrom, KTo>
        //     where TFrom : unmanaged
        //     where KTo : unmanaged
        // {
        //     [FieldOffset(0)] private TFrom from;
        //     [FieldOffset(0)] private KTo to;
        //     public static KTo Convert(TFrom value) { return new Converter<TFrom, KTo> { from = value }.to; }
        // }
        
        // /// <summary>
        // /// convert a unmanaged value type into another using just the bits
        // /// </summary>
        // /// <param name="value">the value to convert</param>
        // /// <typeparam name="TFrom">the expected source type</typeparam>
        // /// <typeparam name="KTo">the desired out type</typeparam>
        // /// <returns>returns the source into a new type converting just the bits</returns>
        // public static KTo Convert<TFrom, KTo>(TFrom value)
        //     where TFrom : unmanaged
        //     where KTo : unmanaged => Converter<TFrom, KTo>.Convert(value);

        // --------------- FLOAT --------------- //
        /// <summary>
        /// check if the value is within 2 numbers, equality is considered inside by default
        /// </summary>
        /// <param name="value">the value the check</param>
        /// <param name="minBoundary">the minimum threshold</param>
        /// <param name="maxBoundary">the max threshold</param>
        /// <param name="acceptEquals">this is used to accept equals as a valid</param>
        /// <returns>returns true if the value is within the min and max</returns>
        public static bool IsWithin(this float value, float minBoundary, float maxBoundary, bool acceptEquals = true) => acceptEquals
            ? value >= minBoundary && value <= maxBoundary
            : value > minBoundary  && value < maxBoundary;

        /// <summary>
        /// checks if a fiven float is NaN
        /// </summary>
        /// <param name="value">the float we want to check</param>
        /// <returns>true if the value is NaN</returns>
        public static bool IsNaN(this float value) => value != value;

        // --------------- PERCENTAGE --------------- //
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
            if (axisFloat >= 0) { return (byte)(axisFloat * 100); }

            //negative
            return (byte)(201 + axisFloat * 100);
        }

        /// <summary>
        /// converts a byte to axis
        /// </summary>
        /// <param name="axisByte">the byte to deconvert</param>
        /// <returns>returns the axis</returns>
        public static float ToAxis(this byte axisByte)
        {
            if (axisByte <= 100) { return axisByte * 0.01f; }

            return axisByte * 0.01f - 2.01f;
        }

        // --------------- INT --------------- //
        /// <summary>
        /// sums an integer and make sure it circles between some values 
        /// </summary>
        /// <param name="element">the element to be changed</param>
        /// <param name="toAdd">the element we want to add</param>
        /// <param name="roundMax">the max</param>
        public static int SumRound(this int element, int toAdd, int roundMax) => (element + toAdd) % roundMax;

        /// <summary>
        /// check if the value is within 2 numbers, equality is considered inside by default
        /// </summary>
        /// <param name="value">the value the check</param>
        /// <param name="minBoundary">the minimum threshold</param>
        /// <param name="maxBoundary">the max threshold</param>
        /// <param name="acceptEquals">this is used to accept equals as a valid</param>
        /// <returns>returns true if the value is within the min and max</returns>
        public static bool IsWithin(this int value, int minBoundary, int maxBoundary, bool acceptEquals = true) => acceptEquals
            ? value >= minBoundary && value <= maxBoundary
            : value > minBoundary  && value < maxBoundary;

        // --------------- ENUMS --------------- //
        /// <summary>
        /// retrieves all the values of a given enumerator
        /// </summary>
        /// <returns>all the possible enumerator, as an array</returns>
        public static TEnum[] GetValues<TEnum>() where TEnum : struct => (TEnum[])Enum.GetValues(typeof(TEnum));

        public static int CountValues<TEnum>() where TEnum : struct => Enum.GetValues(typeof(TEnum)).Length;

        /// <summary>
        /// converts a string into an enum
        public static T ToEnum<T>(this string enumString, bool caseSensitive = false)
            => (T)Enum.Parse(typeof(T), enumString, caseSensitive);

        // --------------- SCRIPTABLE OBJECTS --------------- //
        //a way to set the names of scriptable object
        public static void SetName(this ScriptableObject item, string newName) => item.name = newName + ScriptableObjectSuffix;

#if UNITY_EDITOR
        public static T GetOrCreateAtPath<T>(string folder, string assetName, string assetType = ".asset",
                                             bool   createPathIfMissing = false)
            where T : ScriptableObject
        {
            var path = Path.Combine(folder, assetName + assetType);
            if (createPathIfMissing) { Directory.CreateDirectory(folder); }

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

        // --------------- TRANSFORMS --------------- //
        /// <summary>
        /// removes all children of a transform
        /// </summary>
        public static void ClearTransform(this Transform transform)
        {
            while (transform.childCount != 0) { transform.GetChild(0).gameObject.AutoDestroy(); }
        }

        /// <summary>
        /// find a component from parent, until reaching root component
        /// </summary>
        public static T RetrieveFromParent<T>(this Transform parentTransform)
        {
            if (parentTransform.parent == null) throw new MissingComponentException($"Not such component found: {nameof(T)}");

            var item = parentTransform.GetComponent<T>();
            return item ?? parentTransform.RetrieveFromParent<T>();
        }

        /// <summary>
        /// moves the transform towards a given direction
        /// </summary>
        /// <param name="thisTransform">the transform to move</param>
        /// <param name="target">the position target to reach</param>
        /// <param name="distance">the distance of movement</param>
        /// <returns>returns the same transform for fluent syntax</returns>
        public static Transform MoveTowards(this Transform thisTransform, Vector3 target, float distance)
        {
            var startPosition = thisTransform.position;

            var direction = (target - startPosition).normalized;
            direction *= distance;
            var position = startPosition + direction;
            thisTransform.position = position;
            return thisTransform;
        }

        // --------------- MONOBEHAVIOURS --------------- //
        /// <summary>
        /// checks if a monobehaviour was destroyed
        /// </summary>
        public static bool IsAlive(this MonoBehaviour monoBehaviour) => monoBehaviour != null;

        /// <summary>
        /// checks if a monobehaviour was destroyed, making sure we catch the null reference exception
        /// </summary>
        public static bool IsValid(this MonoBehaviour monoBehaviour)
        {
            try
            {
                if (monoBehaviour.gameObject == null) { return false; }
            }
            catch (Exception) { return false; }

            return true;
        }

        // --------------- COMPONENT --------------- //
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
            for (int i = 0; i < elementThatRequireThis.Length; i++) { elementThatRequireThis[i].InjectThis(element); }
        }

        /// <summary>
        /// to really check if a component is null also after scene change
        /// Unity does different equality checks and simple == null might not work. Check also
        /// https://forum.unity.com/threads/null-check-inconsistency-c.220649/
        /// https://blog.unity.com/technology/custom-operator-should-we-keep-it
        /// https://github.com/Unity-Technologies/UnityCsReference/blob/61f92bd79ae862c4465d35270f9d1d57befd1761/Runtime/Export/Scripting/UnityEngineObject.bindings.cs#L103
        /// </summary>
        public static bool IsNull<T>(this T component) where T : Component => component == null || component.gameObject == null;

        /// <summary>
        /// faster way to check an object against null,
        /// IMPORTANT: it miss the game destroy during scene change, use IsNull in that case 
        /// </summary>
        public static bool QuickIsNull<T>(this T item) where T : Object => ReferenceEquals(item, null);

        /// <summary>
        /// quicker way to catch the component
        /// </summary>
        public static T QuickGetComponent<T>(this Component parent) where T : Component => (T)parent.GetComponent(nameof(T));

        // --------------- GAME OBJECT --------------- //
        public static void ActivateAll(this GameObject[] gameObjects, bool activation)
        {
            for (int i = 0; i < gameObjects.Length; i++) { gameObjects[i].SetActive(activation); }
        }

        /// <summary>
        /// auto destroy one game object
        /// </summary>
        /// <param name="item">the item to be destroyed</param>
        public static void AutoDestroy(this GameObject item)
        {
            Assert.IsNotNull(item, $"Requires a {nameof(item)}");
#if UNITY_EDITOR
            if (Application.isPlaying) { Object.Destroy(item); }
            else { Object.DestroyImmediate(item); }
#else
            Object.Destroy(item);
#endif
        }

        /// <summary>
        /// checks if the elements is a prefab or a scene game object
        /// </summary>
        /// <param name="item">the element to check</param>
        /// <returns>true if this is a prefab, false if this is a gameobject</returns>
        public static bool IsPrefab(this GameObject item) => item.scene.rootCount == 0;

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

        // --------------- VECTORS --------------- //
        public static Direction GetDirection(this Vector2 force)
        {
            // --------------- STOPPED WIND --------------- //
            //if the wind is at 0 it is stopped
            if (Math.Abs(force.x) < JConstants.GeneralFloatTolerance &&
                Math.Abs(force.y) < JConstants.GeneralFloatTolerance) { return Direction.None; }

            //find if the top most intensity is vertical or horizontal
            // --------------- HORIZONTAL --------------- //
            if (Mathf.Abs(force.x) > Mathf.Abs(force.y))
            {
                return force.x >= 0
                           ? Direction.Right
                           : Direction.Left;
            }

            // --------------- VERTICAL --------------- //
            return force.y >= 0
                       ? Direction.Up
                       : Direction.Down;
        }

        /// <summary>
        /// Changes the X value of a Vector3
        /// </summary>
        public static Vector3 WithX(this Vector3 v, float xValue) => new Vector3(xValue, v.y, v.z);

        /// <summary>
        /// Changes the Y value of a Vector3
        /// </summary>
        public static Vector3 WithY(this Vector3 v, float yValue) => new Vector3(v.x, yValue, v.z);

        /// <summary>
        /// Changes the Z value of a Vector3
        /// </summary>
        public static Vector3 WithZ(this Vector3 v, float zValue) => new Vector3(v.x, v.y, zValue);

        // --------------- STRING --------------- //
        public static int ToInt(this string stringToConvert)
        {
            if (int.TryParse(stringToConvert, out int valueToReturn)) { return valueToReturn; }

            Debug.LogWarning($"The string '{stringToConvert}' cannot be converted into integer. Returning 0.");
            return 0;
        }

        public static float ToFloat(this string stringToConvert)
        {
            if (float.TryParse(stringToConvert, out float valueToReturn)) { return valueToReturn; }

            Debug.LogWarning($"String '{stringToConvert}' cannot be converted to float. Returning 0f.");
            return 0f;
        }

        // --------------- DATE TIME and DATE SPAN --------------- //
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
        public static long GetUnixTimeStamp(this DateTime dateTime)
        {
            var  epochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            long unixTime   = (long)(dateTime - epochStart).TotalSeconds;
            return unixTime;
        }

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

        /// <summary>
        /// given a span it returns a string with the 2 highest values
        /// </summary>
        public static string SpanToStringTwo(this TimeSpan span,      string day       = "d", string hour = "h", string min = "m",
                                             string        sec = "s", string separator = ":")
        {
            if (span.Days > 0) { return $"{span:%d}{day}{separator}{span:%h}{hour}"; }

            if (span.Hours > 0) { return $"{span:%h}{hour}{separator}{span:%m}{min}"; }

            return $"{span:%m}{min}{separator}{span:%s}{sec}";
        }

        // --------------- LINE RENDERER --------------- //
        /// <summary>
        /// resets the position of the line render to 0
        /// </summary>
        public static LineRenderer ResetLine(this LineRenderer lr)
        {
            lr.positionCount = 0;
            return lr;
        }

        // --------------- COLOR --------------- //
        /// <summary>
        /// used to add transparency to a given color
        /// </summary>
        /// <param name="spriteRenderer">the color to change</param>
        /// <param name="transparency">the transparency we want to set</param>
        public static Color SetTransparency(this Color color, float transparency)
        {
            Assert.IsTrue(transparency >= 0f && transparency <= 1.0f,
                          $"The transparency to be set should be between 0 and 1. Received value: {transparency}");

            transparency = Mathf.Clamp(transparency, 0f, 1f);
            color        = new Color(color.r, color.g, color.b, transparency);
            return color;
        }
    }
}
