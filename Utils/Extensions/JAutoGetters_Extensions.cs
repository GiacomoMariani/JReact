using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace JReact
{
    public static class JAutoGetters_Extensions
    {
        // --------------- GENERICS --------------- //
        /// <summary>
        /// a method to get the requested item only if not already initiated
        /// it simulates the lazy initiation of c#
        /// </summary>
        /// <param name="value">the value we want to retrieve</param>
        /// <param name="initMethod">the method used to init the item</param>
        /// <returns>returns the item, making sure it is already initiated</returns>
        public static T AutoGet<T>(ref T value, Func<T> initMethod)
        {
            if (value == null) { value = initMethod(); }

            return value;
        }

        /// <summary>
        /// gets the requested item from the gameobject
        /// </summary>
        /// <param name="gameObject">the source where to start the getter</param>
        /// <param name="value">the desired item</param>
        /// <typeparam name="T">the type of the item to catch</typeparam>
        /// <returns>returns the item, making sure it is already initiated</returns>
        public static T AutoGetComponent<T>(this GameObject gameObject, ref T value)
            => JAutoGetters_Extensions.AutoGet(ref value, gameObject.GetComponent<T>);

        /// <summary>
        /// gets the requested item from the component
        /// </summary>
        /// <param name="component">the source where to start the getter</param>
        /// <param name="value">the desired item</param>
        /// <typeparam name="T">the type of the item to catch</typeparam>
        /// <returns>returns the item, making sure it is already initiated</returns>
        public static T AutoGetComponent<T>(this Component component, ref T value)
            => JAutoGetters_Extensions.AutoGet(ref value, component.gameObject.GetComponent<T>);

        /// <summary>
        /// gets the requested item from the scene
        /// </summary>
        /// <param name="gameObject">the source where to start the getter</param>
        /// <param name="value">the desired item</param>
        /// <typeparam name="T">the type of the item to catch</typeparam>
        /// <returns>returns the item, making sure it is already initiated</returns>
        public static T AutoGetSceneInstance<T>(this GameObject gameObject, ref T value) where T : Object
            => JAutoGetters_Extensions.AutoGet(ref value, Object.FindAnyObjectByType<T>);
    }
}
