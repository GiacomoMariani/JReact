using UnityEngine;

namespace JReact
{
    public static class J_ComponentExtensions
    {
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
    }
}
