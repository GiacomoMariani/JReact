using Unity.Mathematics;
using UnityEngine;

namespace JReact
{
    public struct J_PlaceableData
    {
        public readonly float3 position;
        public readonly float4 rotation;

        public readonly Quaternion QuaternionRotation => new Quaternion(rotation.x, rotation.y, rotation.z, rotation.w);

        private J_PlaceableData(Vector3 position, Quaternion rotation)
        {
            this.position = position;
            this.rotation = new float4(rotation.x, rotation.y, rotation.z, rotation.w);
        }

        public static J_PlaceableData FromPositionRotation(Vector3 position, Quaternion rotation) => new(position, rotation);
        public static J_PlaceableData FromTransform(Transform      transform) => new(transform.position, transform.rotation);
        public static J_PlaceableData FromTransformLocal(Transform      transform) => new(transform.localPosition, transform.localRotation);
    }
}
