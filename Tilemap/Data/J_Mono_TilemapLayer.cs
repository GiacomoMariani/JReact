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

        [Button] internal void FromText(TextAsset text) { _layerIds = text.ToIntArray(out _width, true, false); }

        public bool IsCompatible(J_Mono_TilemapLayer layer) => Width == layer.Width && Height == layer.Height;

        public Vector3 GetWorldPosition(Vector3Int cellPosition) => _tilemap.GetCellCenterWorld(cellPosition);

        public void ResetVisuals(int layerId)
        {
            _layerId = layerId;
            _tilemap.ClearAllTiles();

        }

        internal int GetIdAtIndex(int index) => _layerIds[index];

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
    }
}
