using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Unity.Collections;
using UnityEngine;

namespace JReact.Tilemaps
{
    public class J_Mono_MainTileBoard : MonoBehaviour
    {
        // --------------- CONSTS --------------- //
        internal const int NoTile = -1;
        // --------------- EVENTS --------------- //
        public event Action<J_Mono_MainTileBoard> OnGeneration;

        // --------------- FIELDS AND PROPERTIES --------------- //
        [InfoBox("NULL => No border"), BoxGroup("Setup", true, true, 0), SerializeField, ChildGameObjectsOnly]
        private J_Mono_MapBoundary _boundary;
        [BoxGroup("Setup", true, true, 0), SerializeField] private int _zPos;
        public int ZPosision => _zPos;
        [BoxGroup("Setup - Assets", true, true, 0), SerializeField, AssetsOnly, Required]
        private J_SO_TileRepository _tileRepository;
        [BoxGroup("Setup - Assets", true, true, 0), SerializeField, AssetsOnly, Required]
        private J_Mono_TilemapLayer _layerPrefab;
        [BoxGroup("Setup", true, true, 0), SerializeField, ChildGameObjectsOnly, Required]
        private Transform _baseGridParent;
        [BoxGroup("Setup", true, true, 0), SerializeField, ChildGameObjectsOnly, Required]
        private J_Mono_TilemapLayer _ground;
        [BoxGroup("Setup", true, true, 0), SerializeField, ChildGameObjectsOnly, Required]
        private J_Mono_MapGrid _mapGrid;
        public J_Mono_MapGrid MapGrid => _mapGrid;
        [BoxGroup("Setup", true, true, 0), SerializeField, ChildGameObjectsOnly, Required]
        private List<J_Mono_TilemapLayer> _layers = new List<J_Mono_TilemapLayer>();
        [BoxGroup("Setup", true, true, 0), SerializeField] private Vector3Int _startPoint;
        public Vector3Int StartPoint => _startPoint;

        [FoldoutGroup("State", false, 5), Sirenix.OdinInspector.ReadOnly, ShowInInspector]
        public bool HasBorder => _boundary != default;
        [FoldoutGroup("State", false, 5), Sirenix.OdinInspector.ReadOnly, ShowInInspector]
        public int Length => _ground.Length;
        [FoldoutGroup("State", false, 5), Sirenix.OdinInspector.ReadOnly, ShowInInspector]
        public int Width => _ground.Width;
        [FoldoutGroup("State", false, 5), Sirenix.OdinInspector.ReadOnly, ShowInInspector]
        public int Height => _ground.Height;

        // --------------- DATA GENERATION --------------- //
        public void SetGroundData(int[] layerMap, int groundWidth) { _ground.Data(layerMap, groundWidth); }

        public void AddLayerData(int[] layerMap, int layerWidth)
        {
            J_Mono_TilemapLayer layer = Instantiate(_layerPrefab, _baseGridParent);
            layer.Data(layerMap, layerWidth);
            _layers.Add(layer);
        }

        public void ResetLayersData()
        {
            for (int i = 0; i < _layers.Count; i++) { _layers[i].gameObject.AutoDestroy(); }

            _layers.Clear();
        }
        
        // --------------- INITIALIZATION --------------- //
        public void GenerateTileViews()
        {
            ResetTileViews();

            if (HasBorder) { _boundary.DrawBoundaries(this, _ground); }

            NativeArray<JTile> allTiles = DrawAllTilesOnLayers(Allocator.TempJob);
            _mapGrid.InitiateMap(allTiles, Width);
            FinalizeAllTileMaps();
            allTiles.Dispose();
            OnGeneration?.Invoke(this);
        }

        private NativeArray<JTile> DrawAllTilesOnLayers(Allocator allocator)
        {
            var result = new NativeArray<JTile>(Length, allocator);
            for (int tileIndex = 0; tileIndex < Length; tileIndex++)
            {
                JTile tile = result[tileIndex] = CalculateTileProperties(tileIndex);
                DrawTileOnAllLayers(tile);
            }

            return result;
        }

        private JTile CalculateTileProperties(int index)
        {
            Vector3Int position = new Vector3Int(index % Width, index / Width, 0) + _startPoint;
            int        id       = CalculateTileId(index);
            return new JTile(position, id);
        }

        private void DrawTileOnAllLayers(JTile tile)
        {
            _ground.DrawTileOnLayer(tile, GetGroundTileInfo(tile));
            for (int layerIndex = 0; layerIndex < _layers.Count; layerIndex++)
            {
                _layers[layerIndex].DrawTileOnLayer(tile, GetLayerTileInfo(tile, layerIndex));
            }
        }

        private int CalculateTileId(int tileIndex) => _ground.GetIdAtIndex(tileIndex);

        private void FinalizeAllTileMaps()
        {
            _ground.FinalizeThis(this);
            for (int layerIndex = 0; layerIndex < _layers.Count; layerIndex++) { _layers[layerIndex].FinalizeThis(this); }
        }

        // --------------- PUBLIC QUERIES --------------- //
        public J_TileInfo GetGroundTileInfoFromWorld(Vector2 worldPosition)
        {
            JTile tile = MapGrid.GetTileFromWorld(worldPosition);
            return GetTileInfo(tile, _ground);
        }
        
        public J_TileInfo GetGroundTileInfo(int x, int y)
        {
            JTile tile = MapGrid.GetTile(x, y);
            return GetTileInfo(tile, _ground);
        }

        public (Vector2 bottomLeft, Vector2 topRight) GetGroundBorders()
            => (_ground.BottomLeftWorldPosition, _ground.TopRightWorldPosition);

        public Vector3 GetWorldPosition(Vector3Int position) => _ground.GetWorldPosition(position);

        public bool IsCompatible(J_Mono_TilemapLayer layer) => _ground.IsCompatible(layer);

        // --------------- PRIVATE QUERIES --------------- //
        private J_TileInfo GetGroundTileInfo(JTile tile) => GetTileInfo(tile, _ground);

        private J_TileInfo GetLayerTileInfo(JTile tile, int layerIndex)
        {
            J_Mono_TilemapLayer layer = _layers[layerIndex];
            return GetTileInfo(tile, layer);
        }

        private J_TileInfo GetTileInfo(JTile tile, J_Mono_TilemapLayer layer)
        {
            int tileIndex  = tile.ConvertToIndex(this);
            int tileInfoId = layer.GetIdAtIndex(tileIndex);
            //we have no tile if one of the layers above ground is of different size
            return tileInfoId == NoTile ? _tileRepository.EmptyTileInfo : _tileRepository.GetTileInfoSafe(tileInfoId);
        }

        // --------------- RESET --------------- //
        public void ResetTileViews() { ResetAllLayersView(); }

        private void ResetAllLayersView()
        {
            _ground.ResetVisuals(0);
            for (int i = 0; i < _layers.Count; i++) { _layers[i].ResetVisuals(i + 1); }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_baseGridParent == default) { _baseGridParent = _mapGrid.transform; }
        }
#endif
    }
}
