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

        /// <summary>
        /// place the transform on the specific placement data
        /// </summary>
        /// <param name="transform">the transfomr to place</param>
        /// <param name="data">the data to define the placement (position and rotation)</param>
        /// <returns>returns the transfrom</returns>
        public static Transform Place(this Transform transform, J_PlaceableData data)
        {
            transform.position = data.position;
            transform.rotation = data.QuaternionRotation;
            return transform;
        }

        /// <summary>
        /// convert the transform into placement data
        /// </summary>
        /// <param name="transform">the transform we want to convert</param>
        /// <returns>the placement data</returns>
        public static J_PlaceableData ToPlacementData(this Transform transform) => J_PlaceableData.FromTransform(transform);

        /// <summary>
        /// Sets the position of the transform to match the position of the target transform.
        /// </summary>
        /// <param name="transform">The current transform.</param>
        /// <param name="target">The target transform whose position will be used.</param>
        /// <returns>The updated transform with the new position.</returns>
        public static Transform SetOnPosition(this Transform transform, Transform target)
        {
            transform.position = target.position;
            return transform;
        }

        /// <summary>
        /// Sets the rotation of the transform to match the rotation of the target transform.
        /// </summary>
        /// <param name="transform">The current transform.</param>
        /// <param name="target">The target transform whose rotation will be used.</param>
        /// <returns>The updated transform with the new rotation.</returns>
        public static Transform SetOnRotation(this Transform transform, Transform target)
        {
            transform.rotation = target.rotation;
            return transform;
        }

        /// <summary>
        /// Sets the position and rotation of the transform to match the target transform.
        /// </summary>
        /// <param name="transform">The current transform.</param>
        /// <param name="target">The target transform whose position and rotation will be used.</param>
        /// <returns>The updated transform with the new position and rotation.</returns>
        public static Transform SetOn(this Transform transform, Transform target)
        {
            transform.position = target.position;
            transform.rotation = target.rotation;
            return transform;
        }

        /// <summary>
        /// Sets the position and rotation of the transform to match the target transform and sets the target transform as its parent.
        /// </summary>
        /// <param name="transform">The current transform.</param>
        /// <param name="target">The target transform whose position and rotation will be used, and it will become the parent of the current transform.</param>
        /// <returns>The updated transform with the new position, rotation, and parent.</returns>
        public static Transform SetOnParent(this Transform transform, Transform target)
        {
            transform.position = target.position;
            transform.rotation = target.rotation;
            transform.SetParent(target);
            return transform;
        }

        // --------------- PLACEMENT AND ROTATION --------------- //
        public static Transform AddX(this Transform transform, float value)
        {
            var pos = transform.position;
            pos.x              += value;
            transform.position =  pos;
            return transform;
        }

        public static Transform AddY(this Transform transform, float value)
        {
            var pos = transform.position;
            pos.y              += value;
            transform.position =  pos;
            return transform;
        }

        public static Transform AddZ(this Transform transform, float value)
        {
            var pos = transform.position;
            pos.z              += value;
            transform.position =  pos;
            return transform;
        }

        public static Transform SetX(this Transform transform, float value)
        {
            var pos = transform.position;
            pos.x              = value;
            transform.position = pos;
            return transform;
        }

        public static Transform SetY(this Transform transform, float value)
        {
            var pos = transform.position;
            pos.y              = value;
            transform.position = pos;
            return transform;
        }

        public static Transform SetZ(this Transform transform, float value)
        {
            var pos = transform.position;
            pos.z              = value;
            transform.position = pos;
            return transform;
        }

        public static Transform RotateOnX(this Transform transform, float xRotation)
        {
            var currentRotation = transform.rotation;
            var rotationDelta   = Quaternion.AngleAxis(xRotation, Vector3.right);
            transform.rotation = currentRotation * rotationDelta;
            return transform;
        }

        public static Transform RotateOnY(this Transform transform, float yRotation)
        {
            var currentRotation = transform.rotation;
            var rotationDelta   = Quaternion.AngleAxis(yRotation, Vector3.up);
            transform.rotation = currentRotation * rotationDelta;
            return transform;
        }

        /// <summary>
        /// set a specific rotation for the z coordinate
        /// </summary>
        /// <param name="transform">the transform to rotate</param>
        /// <param name="zRotation">the rotation to set</param>
        /// <returns>returns the same transform</returns>
        public static Transform RotateOnZ(this Transform transform, float zRotation)
        {
            var currentRotation = transform.rotation;
            var rotationDelta   = Quaternion.AngleAxis(zRotation, Vector3.up);
            transform.rotation = currentRotation * rotationDelta;
            return transform;
        }
        
        /// <summary>
        /// either returns the same cache or get the component from the transform
        /// </summary>
        /// <param name="transform">the transform were to get the component from</param>
        /// <param name="cache">the object used for caching</param>
        /// <typeparam name="T">the type of component to retrieve</typeparam>
        /// <returns>returns the instance of the component</returns>
        public static T GetOrCache<T>(this Transform transform, ref T cache) where T : Component
        {
            if (cache != default) { return cache; }

            cache = transform.GetComponent<T>();
            return cache;
        }

        /// <summary>
        /// either returns the same cache or get the component from the children transform
        /// </summary>
        /// <param name="transform">the transform were to get the component from</param>
        /// <param name="cache">the object used for caching</param>
        /// <param name="disabledToo">if we want to check also on disabled components</param>
        /// <typeparam name="T">the type of component to retrieve</typeparam>
        /// <returns>returns the instance of the component</returns>
        public static T GetOrCacheChildren<T>(this Transform transform, ref T cache, bool disabledToo) where T : Component
        {
            if (cache != default) { return cache; }

            cache = transform.GetComponentInChildren<T>(disabledToo);
            return cache;
        }
    }
}
