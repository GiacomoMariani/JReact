using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.Tilemaps
{
    public sealed class J_Mono_MapBoundary : MonoBehaviour
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private J_TileInfo _boundaryTileInfo;

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private List<JTile> _boundaryTiles = new List<JTile>();
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private J_Mono_MapGrid _mapGrid;

        internal void DrawBoundaries(J_Mono_MainTileBoard board, J_Mono_TilemapLayer ground)
        {
            // the ring is the board's bounds grown by one cell on every side
            JGridBounds ring = board.Bounds.Expand(1);

            _mapGrid = board.MapGrid;
            _boundaryTiles.Clear();

            // --------------- VERTICAL (left & right edges, interior rows) --------------- //
            for (int y = ring.Min.Y + 1; y < ring.Max.Y; y++)
            {
                _boundaryTiles.Add(CreateBoundary(new JCoord(ring.Min.X, y), ground));
                _boundaryTiles.Add(CreateBoundary(new JCoord(ring.Max.X, y), ground));
            }

            // --------------- HORIZONTAL (top & bottom edges, full width) --------------- //
            for (int x = ring.Min.X; x <= ring.Max.X; x++)
            {
                _boundaryTiles.Add(CreateBoundary(new JCoord(x, ring.Min.Y), ground));
                _boundaryTiles.Add(CreateBoundary(new JCoord(x, ring.Max.Y), ground));
            }
        }

        private JTile CreateBoundary(JCoord position, J_Mono_TilemapLayer ground)
        {
            var tile = new JTile(position, _boundaryTileInfo);
            ground.DrawTileOnLayer(tile, _boundaryTileInfo);
            return tile;
        }

        private void OnDrawGizmosSelected()
        {
            if (_mapGrid             == null ||
                _boundaryTiles       == null ||
                _boundaryTiles.Count == 0) return;

            Gizmos.color = Color.black;
            foreach (JTile tile in _boundaryTiles)
            {
                Vector3 worldPos = _mapGrid.GetWorldPosition(tile);
                Gizmos.DrawCube(worldPos, Vector3.one * 0.2f);
            }
        }
    }
}
