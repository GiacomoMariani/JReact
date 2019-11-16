using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace JReact.Tilemaps.Generator
{
    public abstract class J_Map_Generator<T> : MonoBehaviour
        where T : J_Tile
    {
        private const int zPos = 0;
        // --------------- CREATORS --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] protected J_TileRetriever _tileInfoGetter;
        // --------------- UNITY TILEMAPS --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField, Required] protected Grid _unityMapGrid;
        [BoxGroup("Setup", true, true, 0), SerializeField, Required] protected Tilemap _groundTileMap;

        // --------------- THE MAP GRID --------------- //
        [InfoBox("NULL => No Boundaries"), BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly]
        private J_TileInfo _boundary;
        [BoxGroup("Setup", true, true, 0), SerializeField] protected Vector3Int _startPoint;
        [BoxGroup("Setup", true, true, 0), SerializeField] protected int _width;
        [BoxGroup("Setup", true, true, 0), SerializeField] protected int _Height => _GroundCodes.Length / _width;

        // --------------- ABSTRACT --------------- //
        protected abstract JMapGrid<T> _MapGrid { get; }
        protected abstract iIntArrayGetter _GroundCodes { get; }

        // --------------- GENERATION --------------- //
        [BoxGroup("Test", true, true, 100), Button(ButtonSizes.Medium)]
        public void Generate()
        {
            // --------------- INITIATION --------------- //
            if (_GroundCodes                 == null ||
                _GroundCodes.Length % _width != 0)
                throw new ArgumentException($"{name} {nameof(_GroundCodes)} is null {_GroundCodes == null}, " +
                                            $"or not divisible for width {_width}. Not enough columns");

            Initiate();

            // --------------- BOUNDARIES  --------------- //
            if (_boundary != null) DrawBoundaries(_boundary, _width, _Height);

            // --------------- CREATION --------------- //
            var allTiles = FillMapWithTiles();

            // --------------- CONFIRM --------------- //
            _MapGrid.InitiateMap(_unityMapGrid, allTiles, _width);
        }

        // --------------- INITIATION --------------- //
        protected virtual void Initiate()
        {
            _tileInfoGetter.ClearTileList();
            _groundTileMap.ClearAllTiles();
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
            for (int i = westEdge; i < eastEdge +1; i++)
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
            var tile = new J_Tile(position, _groundTileMap, groundTileInfo);
            return tile;
        }

        private T[] FillMapWithTiles()
        {
            var tiles = new T[_GroundCodes.Length];

            for (int i = 0; i < _GroundCodes.Length; i++)
            {
                // --------------- POSITION --------------- //
                Vector3Int position = new Vector3Int(i % _width, i / _width, zPos);
                position += _startPoint;

                // --------------- TILE --------------- //
                var groundTileInfo = _tileInfoGetter.GetItemFromId(_GroundCodes.ArrayCode[i]);
                var tile           = CreateTile(i, position, groundTileInfo);

                // --------------- STORE TILE --------------- //
                groundTileInfo.Add(tile);
                tiles[i] = tile;
            }

            return tiles;
        }

        // --------------- ABSTRACT --------------- //
        /// <summary>
        /// specifically creates the tile 
        /// </summary>
        /// <param name="index">the current index of the tile</param>
        /// <param name="position">the position of the tile</param>
        /// <param name="groundTileInfo">the base ground used to draw the map</param>
        /// <returns></returns>
        protected abstract T CreateTile(int index, Vector3Int position, J_TileInfo groundTileInfo);
    }

    public interface iIntArrayGetter
    {
        int[] ArrayCode { get; }
        int Length { get; }
    }
}
