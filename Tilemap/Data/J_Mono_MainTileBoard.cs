using System.Collections.Generic;
using JReact.Singleton;
using Sirenix.OdinInspector;
using Unity.Collections;
using UnityEngine;

namespace JReact.Tilemaps
{
    public class J_Mono_MainTileBoard : J_MonoSingleton<J_Mono_MainTileBoard>
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        [InfoBox("NULL => No border"), BoxGroup("Setup", true, true, 0), SerializeField, ChildGameObjectsOnly]
        private J_Mono_MapBoundary _boundary;
        [BoxGroup("Setup", true, true, 0), SerializeField] private int _zPos;
        public int ZPosision => _zPos;
        [BoxGroup("Setup", true, true, 0), SerializeField, ChildGameObjectsOnly, Required]
        private J_Mono_TilemapLayer _ground;
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

        // --------------- QUERY --------------- //
        public Vector3 GetWorldPosition(Vector3Int position) => _ground.GetWorldPosition(position);

        public bool IsCompatible(J_Mono_TilemapLayer layer) => _ground.IsCompatible(layer);

        public J_TileInfo GetGroundTileInfo(JTile tile) => _ground.GetTileInfo(tile.ConvertToIndex(StartPoint.ConvertToInt2(), Width));

        public J_TileInfo GetLayerTileInfo(JTile tile, int layerIndex)
            => _layers[layerIndex].GetTileInfo(tile.ConvertToIndex(StartPoint.ConvertToInt2(), Width));

        // --------------- SETUP --------------- //
        internal void SetupThis()
        {
            ResetThis();
            if (HasBorder) { _boundary.DrawBoundaries(this, _ground); }

            NativeArray<JTile> allTiles = GenerateTiles(Allocator.Temp);
            DrawAllTileLayers(allTiles);
        }

        // --------------- TILE GENERATION --------------- //
        private NativeArray<JTile> GenerateTiles(Allocator allocator)
        {
            var result = new NativeArray<JTile>(Length, allocator);

            for (int i = 0; i < Length; i++)
            {
                var position = new Vector3Int(i / Width, i % Width, 0) + _startPoint;
                int id       = CalculateTileId(i);
                result[i] = new JTile(position, GetWorldPosition(position), id);
            }

            return result;
        }

        private int CalculateTileId(int tileIndex) => _ground.GetIdAtIndex(tileIndex);

        private void DrawAllTileLayers(NativeArray<JTile> allTiles)
        {
            for (int i = 0; i < allTiles.Length; i++)
            {
                _ground.DrawLayerTile(i, allTiles[i]);
                for (int j = 0; j < _layers.Count; j++) { _layers[i].DrawLayerTile(i, allTiles[i]); }
            }
        }

        // --------------- CLEAR --------------- //
        private void ResetThis() { ResetAllLayers(); }

        private void ResetAllLayers()
        {
            _ground.ResetThis(0);
            for (int i = 0; i < _layers.Count; i++) { _layers[i].ResetThis(i + 1); }
        }
    }
}
