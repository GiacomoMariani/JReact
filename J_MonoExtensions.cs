using System;
using UnityEngine;

namespace JReact
{
    public static class J_MonoExtensions
    {
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
    }
}
