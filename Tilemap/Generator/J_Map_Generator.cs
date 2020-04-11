using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Tilemaps;

namespace JReact.Tilemaps.Generator
{
    public abstract class J_Map_Generator<T> : MonoBehaviour
        where T : J_Tile
    {
        private const int zPos = 0;
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] protected J_TileRetriever _tileInfoGetter;
        // --------------- UNITY TILEMAPS --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField, Required] protected Grid _unityMapGrid;
        [BoxGroup("Setup", true, true, 0), SerializeField, Required] protected J_Mono_TileDrawer _tileDrawer;

        // --------------- THE MAP GRID --------------- //
        [InfoBox("NULL => No Boundaries"), BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly]
        protected J_TileInfo _boundary;
        [BoxGroup("Setup", true, true, 0), SerializeField] protected Vector3Int _startPoint;

        // --------------- ABSTRACT --------------- //
        protected abstract JMapGrid<T> _MapGrid { get; }

        // --------------- GENERATION --------------- //
        [BoxGroup("Test", true, true, 100), Button(ButtonSizes.Medium)]
        public void Generate(J_MapData map)
        {
            // --------------- INITIATION --------------- //
            Assert.IsNotNull(map, $"{gameObject.name} requires a {nameof(map)}");

            BeforeGeneration(map);
            Initiate(map);

            // --------------- BOUNDARIES  --------------- //
            if (_boundary != null) DrawBoundaries(_boundary, map.Width, map.Height);

            // --------------- CREATION --------------- //
            var allTiles = FillMapWithTiles(map);

            // --------------- CONFIRM --------------- //
            _MapGrid.InitiateMap(_unityMapGrid, allTiles, map.Width);
            AfterGeneration(map);
        }

        // --------------- INITIATION --------------- //
        protected virtual void Initiate(J_MapData map)
        {
            _tileInfoGetter.ClearTileList();
            _tileDrawer.Setup();
            map.Setup();
        }

        // --------------- CREATION --------------- //
        private void DrawBoundaries(J_TileInfo boundary, int width, int height)
        {
            // --------------- CALCULATE EDGES --------------- //
            int westEdge  = _startPoint.x - 1;
            int eastEdge  = _startPoint.x + width;
            int southEdge = _startPoint.y - 1;
            int northEdge = _startPoint.y + height;

            // --------------- VERTICAL --------------- //
            for (int i = southEdge + 1; i < northEdge; i++)
            {
                Vector3Int boundaryWestPos = new Vector3Int(westEdge, i, zPos);
                Vector3Int boundaryEastPos = new Vector3Int(eastEdge, i, zPos);

                var westTile = CreateBoundary(boundaryWestPos, boundary);
                boundary.Add(westTile);
                var eastTile = CreateBoundary(boundaryEastPos, boundary);
                boundary.Add(eastTile);
            }

            // --------------- HORIZONTAL --------------- //
            for (int i = westEdge; i < eastEdge + 1; i++)
            {
                Vector3Int boundarySouthPos = new Vector3Int(i, southEdge, zPos);
                Vector3Int boundaryNorthPos = new Vector3Int(i, northEdge, zPos);

                var southTile = CreateBoundary(boundarySouthPos, boundary);
                boundary.Add(southTile);
                var northTile = CreateBoundary(boundaryNorthPos, boundary);
                boundary.Add(northTile);
            }
        }

        protected virtual J_Tile CreateBoundary(Vector3Int position, J_TileInfo groundTileInfo)
        {
            var tile = new J_Tile(position, _tileDrawer, groundTileInfo, null);
            return tile;
        }

        private T[] FillMapWithTiles(J_MapData map)
        {
            var tiles = new T[map.Length];
            var width = map.Width;

            for (int i = 0; i < map.Length; i++)
            {
                // --------------- POSITION --------------- //
                Vector3Int position = new Vector3Int(i % width, i / width, zPos);
                position += _startPoint;

                // --------------- TILE --------------- //
                var groundTileInfo  = map.GetGroundTile(i, _tileInfoGetter);
                var overgroundTiles = map.GetOvergroundTiles(i, _tileInfoGetter);
                var tile            = CreateTile(i, position, groundTileInfo, overgroundTiles);

                // --------------- STORE TILE --------------- //
                groundTileInfo.Add(tile);
                tiles[i] = tile;
            }

            return tiles;
        }

        // --------------- ABSTRACT AND VIRTUAL --------------- //
        /// <summary>
        /// specifically creates the tile 
        /// </summary>
        /// <param name="index">the current index of the tile</param>
        /// <param name="position">the position of the tile</param>
        /// <param name="groundTileInfo">the base ground used to draw the map</param>
        /// <param name="overgroundTiles"></param>
        /// <returns></returns>
        protected abstract T CreateTile(int index, Vector3Int position, J_TileInfo groundTileInfo, J_TileInfo[] overgroundTiles);

        protected virtual void BeforeGeneration(J_MapData map) {}

        protected virtual void AfterGeneration(J_MapData map) {}
    }

    public interface iIntArrayGetter
    {
        int[] ArrayCode { get; }
        int Length { get; }
    }
}
