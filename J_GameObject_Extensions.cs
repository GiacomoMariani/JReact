using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Object = UnityEngine.Object;

namespace JReact
{
    public static class J_GameObject_Extensions
    {
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

        /// <summary>
        /// gets the component without allocating
        /// </summary>
        /// <param name="go">the gameobject where we start the search</param>
        /// <typeparam name="T">the desired type</typeparam>
        /// <returns>returns the first component found or default if none is found</returns>
        public static T GetComponentNoAlloc<T>(this GameObject go) where T : class
            => J_GameObject_Extensions<T>.GetComponentNoAlloc(go);

        /// <summary>
        /// set a gameobject enabled or disabled
        /// make sure to change the state of the game only if required
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="enableGameObject"></param>
        public static void SetActiveSafe(this GameObject gameObject, bool enableGameObject)
        {
            if (gameObject.activeSelf == enableGameObject) { return; }
            else { gameObject.SetActive(enableGameObject); }
        }
    }

    public static class J_GameObject_Extensions<T> where T : class
    {
        private static readonly List<T> _Reusable = new List<T>();

        /// <summary>
        /// gets a component without allocating
        /// </summary>
        /// <param name="go">the gameobject to get it from</param>
        /// <typeparam name="T">the desired type</typeparam>
        /// <returns>returns the first component found or default if none is found</returns>
        public static T GetComponentNoAlloc(GameObject go)
        {
            _Reusable.Clear();

            go.GetComponents(_Reusable);

            if (_Reusable.Count <= 0) { return default; }

            T component = _Reusable[0];

            return component;
        }
    }
}
