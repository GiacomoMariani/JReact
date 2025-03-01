using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.Tilemaps.Split15
{
    [CreateAssetMenu(menuName = "Reactive/Tilemap/GroundTile15Split", fileName = "F_GroundTile15Split",         order = 0)]
    public sealed class J_SO_15SplitTileLibrary : ScriptableObject
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField] private int _tileId;
        public int TileId => _tileId;

        [BoxGroup("Setup", true, true, 0), SerializeField] private float _verticalDisplacement = .5f;
        public float VerticalDisplacement => _verticalDisplacement;

        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private J_SO_GroundTile[] _tiles;

        public J_SO_GroundTile GetTile(Vector3Int position, J_Mono_MainTileBoard board)
        {
            int index = CalculateIndex(position, board);
            return _tiles[index];
        }

        private int CalculateIndex(Vector3Int position, J_Mono_MainTileBoard board)
        {
            Vector3Int coords = position;
            // 4 neighbours
            bool topRight = IsSameKind(coords - JTileNeighbourMap.Neighbours[0], board);
            bool topLeft  = IsSameKind(coords - JTileNeighbourMap.Neighbours[1], board);
            bool botRight = IsSameKind(coords - JTileNeighbourMap.Neighbours[2], board);
            bool botLeft  = IsSameKind(coords - JTileNeighbourMap.Neighbours[3], board);

            Tuple<bool, bool, bool, bool> neighbourTuple = new(topLeft, topRight, botLeft, botRight);
            return JTileNeighbourMap.NeighbourTupleToTile[neighbourTuple];
        }

        private bool IsSameKind(Vector3Int neighbour, J_Mono_MainTileBoard board)
        {
            if (!board.IsInsideBorders(neighbour)) { return true; }

            JTile tile = board.MapGrid.GetTile(neighbour);
            if (tile.IsDefault()) { return true; }

            return tile.id == _tileId;
        }
    }
}
