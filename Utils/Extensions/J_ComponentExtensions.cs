using UnityEngine;
using UnityEngine.Assertions;

namespace JReact
{
    public static class J_ComponentExtensions
    {
        /// <summary>
        /// auto destroy one game object
        /// </summary>
        /// <param name="component">the item to be destroyed</param>
        public static void AutoDestroy(this Component component)
        {
            Assert.IsNotNull(component, $"Requires a {nameof(component)}");
#if UNITY_EDITOR
            if (Application.isPlaying) { Object.Destroy(component); }
            else { Object.DestroyImmediate(component); }
#else
            Object.Destroy(component);
#endif
        }

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

        /// <summary>
        /// gets the component without allocating
        /// </summary>
        /// <param name="c">the component where we start the search</param>
        /// <typeparam name="T">the desired type</typeparam>
        /// <returns>returns the first component found or default if none is found</returns>
        public static T GetComponentNoAlloc<T>(this Component c) where T : class
            => J_GameObject_Extensions<T>.GetComponentNoAlloc(c.gameObject);

        /// <summary>
        /// either returns the same cache or get the component from the gameobject
        /// </summary>
        /// <param name="component">the component were to get the component from</param>
        /// <param name="cache">the object used for caching</param>
        /// <typeparam name="T">the type of component to retrieve</typeparam>
        /// <returns>returns the instance of the component</returns>
        public static T GetOrCache<T>(this Component component, ref T cache) where T : Component
        {
            if (cache != default) { return cache; }

            cache = component.GetComponent<T>();
            return cache;
        }

        /// <summary>
        /// either returns the same cache or get the component from the children component
        /// </summary>
        /// <param name="component">the component were to get the component from</param>
        /// <param name="cache">the object used for caching</param>
        /// <param name="disabledToo">if we want to check also on disabled components</param>
        /// <typeparam name="T">the type of component to retrieve</typeparam>
        /// <returns>returns the instance of the component</returns>
        public static T GetOrCacheChildren<T>(this Component component, ref T cache, bool disabledToo) where T : Component
        {
            if (cache != default) { return cache; }

            cache = component.GetComponentInChildren<T>(disabledToo);
            return cache;
        }

        public static void EnsureComponent<T>(this GameObject parent, ref T component) where T : Component
        {
            if (component != null) { return; }

            GameObject gameObject = new GameObject(typeof(T).Name);
            gameObject.transform.SetParent(parent.transform);
            component = gameObject.AddComponent<T>();
        }
    }
}
