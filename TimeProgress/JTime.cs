using UnityEngine;

namespace JReact.TimeProgress
{
    public static class JTime
    {
        public static float RealtimeSinceStartup => Time.realtimeSinceStartup;
        public static float UnscaledTime => Time.unscaledTime;
        public static float DeltaTime => Time.deltaTime;
    }
}
