using JReact.Environment.SkyLight;
using JReact.Environment.Wind;
using JReact.Singleton;
using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.Environment
{
    public sealed class J_St_Environment : J_MonoSingleton<J_St_Environment>
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField, ChildGameObjectsOnly, Required] private J_SkyLight _skyLight;
        public J_SkyLight SkyLight => _skyLight;
        [BoxGroup("Setup", true, true, 0), SerializeField, ChildGameObjectsOnly, Required] private J_Wind _wind;
        public J_Wind Wind => _wind;

        // --------------- SKY LIGHT --------------- //
        public static float SkyLightBrightness => GetInstanceSafe().SkyLight.Brightness;
        public static Vector2 SkyLightDirection => GetInstanceSafe().SkyLight.LightDirection;
        public static float SkyLightShadowMultiplier => GetInstanceSafe().SkyLight.ShadowMultiplier;
        public static SkyLightKind SkyLightKind => GetInstanceSafe().SkyLight.SkyLightKind;
        public static SkyLightStrength SkyLightStrength => GetInstanceSafe().SkyLight.SkyLightStrength;

        // --------------- WIND --------------- //
        public static WindKind WindKind => GetInstanceSafe().Wind.WindKind;
        public static Vector2 WindFlow => GetInstanceSafe().Wind.WindFlow;
        public static float WindStrength => GetInstanceSafe().Wind.WindStrength;
    }
}
