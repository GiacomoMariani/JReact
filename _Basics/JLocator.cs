namespace JReact
{
    /// <summary>
    /// A static class used to locate instances of a generic type.
    /// </summary>
    /// <typeparam name="T">The type of instance to locate.</typeparam>
    public static class JLocator<T>
    {
        /// <summary>
        /// A static class used to locate instances of a generic type.
        /// </summary>
        /// <typeparam name="T">The type of instance to locate.</typeparam>
        private static T Instance;
        private static string TypeName = typeof(T).Name;

        /// <summary>
        /// Registers an instance of a generic type.
        /// </summary>
        /// <typeparam name="T">The type of instance to register.</typeparam>
        /// <param name="instance">The instance to register.</param>
        public static void RegisterInstance(T instance)
        {
            if (!Instance.DefaultEqual(default)) { LogWarning("Replacement", Instance, instance); }

            Instance = instance;
        }

        /// <summary>
        /// Unregisters an instance of a generic type from the JLocator.
        /// </summary>
        /// <typeparam name="T">The type of instance to unregister.</typeparam>
        /// <param name="instance">The instance to unregister.</param>
        public static void UnRegisterInstance(T instance)
        {
            if (!Instance.DefaultEqual(default))
            {
                LogWarning("Comparision", Instance, instance);
                return;
            }

            Instance = default;
        }

        /// <summary>
        /// Retrieves the registered instance of a generic type from JLocator.
        /// </summary>
        /// <typeparam name="T">The type of instance to retrieve.</typeparam>
        /// <returns>The registered instance of the generic type.</returns>
        public static T GetInstance() => Instance;

        /// <summary>
        /// Checks if an instance of a generic type is ready.
        /// </summary>
        /// <typeparam name="T">The type of the instance.</typeparam>
        /// <returns>
        /// <c>true</c> if an instance of the generic type is ready; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsReady() => !Instance.DefaultEqual(default);

        /// <summary>
        /// Checks if an instance is registered in a locator of a generic type.
        /// </summary>
        /// <typeparam name="T">The type of instance to check.</typeparam>
        /// <param name="instance">The instance to check.</param>
        /// <returns>True if the instance is registered, otherwise false.</returns>
        public static bool IsRegistered(T instance) => instance.DefaultEqual(Instance);

        private static void LogWarning(string operationType, T oldInstance, T newInstance)
        {
            JLog.Warning($"{operationType} Operation: {TypeName} => {oldInstance} != {newInstance}", JLogTags.Infrastructure);
        }
    }
}
