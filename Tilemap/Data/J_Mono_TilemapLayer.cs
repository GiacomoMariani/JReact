using System.Collections.Generic;
using JReact.Singleton;
using JReact.Tilemaps.Generator;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Tilemaps;

namespace JReact.Tilemaps
{
    public class J_Mono_TilemapLayer : MonoBehaviour
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField, ChildGameObjectsOnly, Required]
        private Tilemap _tilemap;

        [BoxGroup("Setup", true, true, 0), SerializeField] private int _width;
        [BoxGroup("Setup", true, true, 0), SerializeField] private int[] _layerIds;

        // --------------- STATE --------------- //
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private int _layerId;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public int Length => _layerIds?.Length ?? 0;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public int Width => _width;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public int Height
            => _layerIds == null ? 0 : _layerIds.Length / Width;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public Vector2 BottomLeftWorldPosition { get; private set; }
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public Vector2 TopRightWorldPosition { get; private set; }

        [Button]
        internal void Data(int[] data, int width)
        {
            _width    = width;
            _layerIds = data;
        }

        public bool IsCompatible(J_Mono_TilemapLayer layer) => Width == layer.Width && Height == layer.Height;

        public Vector3 GetWorldPosition(Vector3Int cellPosition) => _tilemap.GetCellCenterWorld(cellPosition);

        public void ResetVisuals(int layerId)
        {
            _layerId = layerId;
            _tilemap.ClearAllTiles();
        }

        internal int GetIdAtIndex(int index) => _layerIds.ValidIndex(index) ? _layerIds[index] : J_Mono_MainTileBoard.NoTile;

        /// <summary>
        /// the index may be different by the coordinates, because we have a starting point on main tile map
        /// </summary>
        /// <param name="jTile"></param>
        /// <param name="tileInfo"></param>
        internal void DrawTileOnLayer(JTile jTile, J_TileInfo tileInfo)
        {
            if (tileInfo.IsEmptyTile) { return; }

            _tilemap.SetTile(jTile.cellPosition.ToVector3Int(), tileInfo.UnityTile);
        }

        public void FinalizeThis(J_Mono_MainTileBoard mainBoard)
        {
            BoundsInt bounds     = _tilemap.cellBounds;
            Vector3   cellOffset = GetTileCellOffset(_tilemap);

            //bottom left
            Vector3Int bottomLeftCellPosition = new Vector3Int(bounds.xMin, bounds.yMin, 0);
            BottomLeftWorldPosition = _tilemap.CellToWorld(bottomLeftCellPosition);

            //top right
            Vector3Int topRightCellPosition = new Vector3Int(bounds.xMax, bounds.yMax, 0);
            TopRightWorldPosition = _tilemap.CellToWorld(topRightCellPosition);
        }

        private Vector2 GetTileCellOffset(Tilemap tilemap) => new(tilemap.cellSize.x / 2, tilemap.cellSize.y / 2);

        private void OnDrawGizmos()
        {
            // Ensure we have tilemap reference and the gizmo is within editor bounds
            if (_tilemap == null) { return; }

            BoundsInt bounds = _tilemap.cellBounds;

            //bottom left
            Vector3Int bottomLeftCellPosition  = new Vector3Int(bounds.xMin, bounds.yMin, bounds.zMin);
            Vector3    bottomLeftWorldPosition = _tilemap.CellToWorld(bottomLeftCellPosition);
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(bottomLeftWorldPosition, 0.5f);

            //top right
            Vector3Int topRightCellPosition  = new Vector3Int(bounds.xMax, bounds.yMax, bounds.zMax);
            Vector3    topRightWorldPosition = _tilemap.CellToWorld(topRightCellPosition);
            Gizmos.DrawSphere(topRightWorldPosition, 0.5f);
        }
    }
}
