using System;
using System.Collections.Generic;
using JReact.Singleton;
using Sirenix.OdinInspector;
using Unity.Collections;
using UnityEngine;

namespace JReact.Tilemaps
{
    public class J_Mono_MainTileBoard : J_MonoSingleton<J_Mono_MainTileBoard>
    {
        // --------------- EVENTS --------------- //
        public event Action<J_Mono_MainTileBoard> OnGeneration;

        // --------------- FIELDS AND PROPERTIES --------------- //
        [InfoBox("NULL => No border"), BoxGroup("Setup", true, true, 0), SerializeField, ChildGameObjectsOnly]
        private J_Mono_MapBoundary _boundary;
        [BoxGroup("Setup", true, true, 0), SerializeField] private int _zPos;
        public int ZPosision => _zPos;
        [BoxGroup("Setup", true, true, 0), SerializeField, ChildGameObjectsOnly, Required]
        private Transform _baseGridParent;
        [BoxGroup("Setup", true, true, 0), SerializeField, ChildGameObjectsOnly, Required]
        private J_Repo_AllTileInfo _tileRepository;
        [BoxGroup("Setup", true, true, 0), SerializeField, ChildGameObjectsOnly, Required]
        private J_Mono_TilemapLayer _ground;
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private J_Mono_TilemapLayer _layerPrefab;
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

        // --------------- INIT --------------- //
        protected internal override void InitThis()
        {
            _tileRepository.InitRepository();
            base.InitThis();
        }

        // --------------- QUERY --------------- //
        public (Vector2 bottomLeft, Vector2 topRight) GetGroundBorders()
            => (_ground.BottomLeftWorldPosition, _ground.TopRightWorldPosition);

        public Vector3 GetWorldPosition(Vector3Int position) => _ground.GetWorldPosition(position);

        public bool IsCompatible(J_Mono_TilemapLayer layer) => _ground.IsCompatible(layer);

        public J_TileInfo GetGroundTileInfo(JTile tile)
        {
            int tileInfoId = _ground.GetIdAtIndex(tile.ConvertToIndex(this));
            return _tileRepository.GetTileInfo(tileInfoId);
        }

        private J_TileInfo GetLayerTileInfo(JTile tile, int layerIndex)
        {
            J_Mono_TilemapLayer layer      = _layers[layerIndex];
            int                 tileIndex  = tile.ConvertToIndex(this);
            int                 tileInfoId = layer.GetIdAtIndex(tileIndex);
            return _tileRepository.GetTileInfo(tileInfoId);
        }

        // --------------- DATA GENERATION --------------- //
        public void SetGroundData(TextAsset layerMap) { _ground.FromText(layerMap); }

        public void AddLayerData(TextAsset layerMap)
        {
            J_Mono_TilemapLayer layer = Instantiate(_layerPrefab, _baseGridParent);
            layer.FromText(layerMap);
            _layers.Add(layer);
        }

        public void ResetLayersData()
        {
            for (int i = 0; i < _layers.Count; i++) { _layers[i].gameObject.AutoDestroy(); }

            _layers.Clear();
        }

        // --------------- VIEW SETUP --------------- //
        public void GenerateTileViews()
        {
            ResetTileViews();
            if (HasBorder) { _boundary.DrawBoundaries(this, _ground); }

            var allTiles = DrawAllTileLayers(Allocator.TempJob);
            _mapGrid.InitiateMap(allTiles, Width);
            FinalizeAllTileMaps();
            allTiles.Dispose();
            OnGeneration?.Invoke(this);
        }

        private NativeArray<JTile> DrawAllTileLayers(Allocator allocator)
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
            var position = new Vector3Int(index % Width, index / Width, 0) + _startPoint;
            int id       = CalculateTileId(index);
            return new JTile(position, GetWorldPosition(position), id);
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
