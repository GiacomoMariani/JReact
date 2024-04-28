using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact
{
    public static class J_2DExtensions
    {
        // --------------- 2D --------------- //
        /// <summary>
        /// rotates a 2d transform to look at the given position
        /// </summary>
        /// <param name="transform">the transform to rotate</param>
        /// <param name="position">the position to look at</param>
        /// <param name="forward">the forward position of the transform</param>
        /// <returns>returns the same transform</returns>
        public static Transform LookAt2D(this Transform transform, Vector3 position, Direction forward)
        {
            var positionToLook = position - transform.position;
            switch (forward)
            {
                case Direction.Up:
                    transform.up = positionToLook;
                    break;
                case Direction.Right:
                    transform.right = positionToLook;
                    break;
                case Direction.Down:
                    transform.up = -positionToLook;
                    break;
                case Direction.Left:
                    transform.right = -positionToLook;
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(forward), forward, null);
            }

            return transform;
        }

        /// <summary>
        /// moves the transform in a specific direction for a given distance
        /// </summary>
        /// <param name="transform">the transform to move</param>
        /// <param name="positionToLook">position we want to look at</param>
        /// <param name="distance">the distance we want to move</param>
        /// <returns>returns the transform itself</returns>
        public static Transform Move2D(this Transform transform, Vector3 positionToLook, float distance)
        {
            var     start     = transform.position;
            Vector2 direction = (positionToLook - start).normalized;
            direction *= distance;
            Vector2 positionToReach = start;
            positionToReach    += direction;
            transform.position =  positionToReach;
            return transform;
        }

        /// <summary>
        /// calculates the time to move between to points, based on a given speed
        /// </summary>
        /// <param name="start">the point where to start</param>
        /// <param name="end">the point to rwach</param>
        /// <param name="unitsPerSecond">the units per second</param>
        /// <returns>the time to reach the end point</returns>
        public static float GetTimeToReach2D(this Transform transform, Vector2 end, float unitsPerSecond)
        {
            var distanceInUnits = math.distance(end, (Vector2)transform.position);
            return distanceInUnits / unitsPerSecond;
        }

        /// <summary>
        /// used to set a transparency on a given sprite renderer
        /// </summary>
        /// <param name="spriteRenderer">the sprite renderer to adjust</param>
        /// <param name="transparency">the transparency we want to set</param>
        public static SpriteRenderer SetTransparency(this SpriteRenderer spriteRenderer, float transparency)
        {
            Assert.IsTrue(transparency >= 0f && transparency <= 1.0f,
                          $"The transparency to be set on {spriteRenderer.gameObject.name} should be between 0 and 1. Received value: {transparency}");

            transparency = Mathf.Clamp(transparency, 0f, 1f);
            Color fullColor = spriteRenderer.color;
            spriteRenderer.color = new Color(fullColor.r, fullColor.g, fullColor.b, transparency);
            return spriteRenderer;
        }
    }
}
