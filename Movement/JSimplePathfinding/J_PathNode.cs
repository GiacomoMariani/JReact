using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.Pathfinding
{
    /// <summary>
    /// the base path node
    /// </summary>
    public class J_PathNode : MonoBehaviour
    {
        // --------------- Logic --------------- //
        //the weight to be spent when walking on this node
        [BoxGroup("Setup", true, true, 10), SerializeField] internal int baseWeight = 10;

        // --------------- STATE --------------- //
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] internal Vector2Int Coordinates { get; private set; }
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public virtual Vector2 NodePosition => transform.position;

        /// <summary>
        /// manually sets the coordinates, if required
        /// </summary>
        /// <param name="coordinates"></param>
        public void SetNodeCoordinates(Vector2Int coordinates) { Coordinates = coordinates; }

        /// <summary>
        /// counts the weight of this current node. Might be overriden
        /// </summary>
        /// <returns>returns the weight of the node</returns>
        public virtual int GetWeight() => baseWeight;
    }
}
