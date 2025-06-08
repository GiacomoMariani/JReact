using System;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace JReact.CheatConsole
{
    /// <summary>
    /// used to load a method directly into the valid cheats
    /// an example of usage: [JCheat("This is a description.", "{method_name} {userIndex}(requires int)"]
    /// auto cheats require to be static
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class JStaticCheatAttribute : Attribute
    {
        public readonly string Description;
        public readonly string Format;

        public JStaticCheatAttribute(string description, string format)
        {
            this.Description = description;
            this.Format      = format;
        }
    }

    public sealed class JAutoCheat : JCheat
    {
        private readonly MethodInfo _methodCall;

        private JAutoCheat(JStaticCheatAttribute attribute, MethodInfo methodCall)
            : base(methodCall.Name, attribute.Description, attribute.Format) => _methodCall = methodCall;

        public override void Invoke(J_Mono_CheatConsole console)
        {
            base.Invoke(console);

            var methodParameters = _methodCall.GetParameters();
            var parametersAmount = methodParameters.Length;
            if (console.Parameters        == null ||
                console.Parameters.Length != parametersAmount + 1)
            {
                JLog.Error($"Cheat {CommandId} requires {parametersAmount} parameters.", JLogTags.Cheats, console.gameObject);
                return;
            }

            var loadedParameters = new object[parametersAmount];

            for (int i = 0; i < loadedParameters.Length; i++)
                //the first parameter on the console, is the method name, so we load from i+1
                loadedParameters[i] = Convert.ChangeType(console.Parameters[i + 1], methodParameters[i].ParameterType);

            _methodCall.Invoke(null, loadedParameters);
        }

        public static void LoadReflectedCheats(J_Mono_CheatConsole console)
        {
            // manual search for method calls inside all domain assemblies
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
#if UNITY_EDITOR
            // Type cache is faster, but only available in UnityEditor.
            TypeCache.MethodCollection cachedMethods = TypeCache.GetMethodsWithAttribute<JStaticCheatAttribute>();
            foreach (var method in cachedMethods)
            {
                var autoCheat = method.GetCustomAttribute<JStaticCheatAttribute>();
                console.AddCommand(new JAutoCheat(autoCheat, method));
            }
#else
            for (int assemplyIndex = 0; assemplyIndex < assemblies.Length; assemplyIndex++)
            {
                Type[] types = assemblies[assemplyIndex].GetTypes();
                for (int typeIndex = 0; typeIndex < types.Length; typeIndex++)
                {
                    var methods = types[typeIndex].GetMethods(BindingFlags.Static | BindingFlags.Public);
                    for (int methodIndex = 0; methodIndex < methods.Length; methodIndex++)
                    {
                        MethodInfo            method         = methods[methodIndex];
                        JStaticCheatAttribute cheatAttribute = method.GetCustomAttribute<JStaticCheatAttribute>();
                        if (cheatAttribute != null) { console.AddCommand(new JAutoCheat(cheatAttribute, method)); }
                    }
                }
            }
#endif
        }
    }
}
