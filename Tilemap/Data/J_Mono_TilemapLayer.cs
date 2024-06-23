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
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private J_TileInfo _emptyTileInfo;
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private J_TileInfo[] _validTiles;

        [BoxGroup("Setup", true, true, 0), SerializeField] private int _width;
        [BoxGroup("Setup", true, true, 0), SerializeField] private int[] _layerIds;

        // --------------- STATE --------------- //
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private int _layerId;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public int Length => _layerIds?.Length ?? 0;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public int Width => _width;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public int Height
            => _layerIds == null ? 0 : _layerIds.Length / Width;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector]
        private Dictionary<int, J_TileInfo> _tileInfoMapping = new Dictionary<int, J_TileInfo>();

        [Button] internal void FromText(TextAsset text) { _layerIds = text.ToIntArray(out _width, true, true); }

        public bool IsCompatible(J_Mono_TilemapLayer layer) => Width == layer.Width && Height == layer.Height;

        public Vector3 GetWorldPosition(Vector3Int cellPosition) => _tilemap.GetCellCenterWorld(cellPosition);

        public void ResetThis(int layerId)
        {
            _layerId = layerId;
            _tilemap.ClearAllTiles();
            _tileInfoMapping.Clear();
            AddTileInfo(_emptyTileInfo);
            for (int i = 0; i < _validTiles.Length; i++)
            {
                var tileInfo = _validTiles[i];
                AddTileInfo(tileInfo);
            }
        }

        private void AddTileInfo(J_TileInfo tileInfo)
        {
            Assert.IsFalse(_tileInfoMapping.ContainsKey(tileInfo.TileInfoId));
            _tileInfoMapping.Add(tileInfo.TileInfoId, tileInfo);
        }

        internal int GetIdAtIndex(int index) => _layerIds[index];

        public J_TileInfo GetTileInfo(int index)
        {
            var tileInfoId = _layerIds[index];
            return _tileInfoMapping[tileInfoId];
        }

        /// <summary>
        /// the index may be different by the coordinates, because we have a starting point on main tile map
        /// </summary>
        /// <param name="tileInfoIndex"></param>
        /// <param name="tileData"></param>
        internal void DrawLayerTile(int tileInfoIndex, JTile tileData)
        {
            J_TileInfo tileInfo = GetTileInfo(tileInfoIndex);
            if (tileInfo.IsEmptyTile) { return; }

            DirectDrawTile(tileData, tileInfo);
        }

        internal void DirectDrawTile(JTile tileData, J_TileInfo tileInfo)
        {
            tileData.AddTileInfoIndexAtLayer(_layerId, tileInfo.TileInfoId);
            _tilemap.SetTile(tileData.cellPosition.ToVector3Int(), tileInfo.UnityTile);
        }
    }
}
