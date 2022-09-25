using UnityEngine;

namespace JReact
{
    public static class J_TransformExtensions
    {
        /// <summary>
        /// removes all children of a transform
        /// </summary>
        public static Transform ClearTransform(this Transform transform)
        {
            while (transform.childCount != 0) { transform.GetChild(0).gameObject.AutoDestroy(); }

            return transform;
        }

        /// <summary>
        /// removes all children of a transform
        /// </summary>
        public static Transform PlaceOnParent(this Transform transformChild, Transform transformParent, bool worldPositionStays = true)
        {
            transformChild.SetParent(transformParent, worldPositionStays);
            transformChild.localPosition = JConstants.Vector3Zero;
            transformChild.rotation      = JConstants.quarterionIdentity;
            transformChild.localScale    = JConstants.Vector3One;
            return transformChild;
        }

        /// <summary>
        /// find a component from parent, until reaching root component
        /// </summary>
        public static T RetrieveFromParent<T>(this Transform parentTransform)
        {
            if (parentTransform.parent == null) throw new MissingComponentException($"Not such component found: {nameof(T)}");

            var item = parentTransform.GetComponent<T>();
            return item ?? parentTransform.RetrieveFromParent<T>();
        }

        /// <summary>
        /// moves the transform towards a given direction
        /// </summary>
        /// <param name="thisTransform">the transform to move</param>
        /// <param name="target">the position target to reach</param>
        /// <param name="distance">the distance of movement</param>
        /// <returns>returns the same transform for fluent syntax</returns>
        public static Transform MoveTowards(this Transform thisTransform, Vector3 target, float distance)
        {
            var startPosition = thisTransform.position;

            var direction = (target - startPosition).normalized;
            direction *= distance;
            var position = startPosition + direction;
            thisTransform.position = position;
            return thisTransform;
        }
    }
}
