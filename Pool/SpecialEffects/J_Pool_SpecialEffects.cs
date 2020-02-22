using UnityEngine;

namespace JReact.Pool.SpecialEffect
{
    /// <summary>
    /// a pool of special effects
    /// </summary>
    [CreateAssetMenu(menuName = "Reactive/Special Effects/Pool")]
    public class J_Pool_SpecialEffects : J_Pool<J_PoolItem_SpecialEffect>
    {
        /// <summary>
        /// triggers the effect on a specific location
        /// </summary>
        public virtual void TriggerEffectOnPosition(Vector3 position, Quaternion rotation)
        {
            //get the next effect
            J_PoolItem_SpecialEffect specialEffect = Spawn();
            //set its location
            Transform transform = specialEffect.transform;
            transform.position = position;
            transform.rotation = rotation;
            //play the effect
            specialEffect.Activate();
        }

        /// <summary>
        /// plays the effect on a transform
        /// </summary>
        /// <param name="transform">the transform where this will appear</param>
        public void TriggerEffectOnTransform(Transform transform) { TriggerEffectOnPosition(transform.position, transform.rotation); }
    }
}
