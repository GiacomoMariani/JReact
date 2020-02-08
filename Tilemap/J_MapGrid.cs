using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.Tilemaps
{
    //this element contains all the data inside the map
    public abstract class JMapGrid<T> : ScriptableObject
        where T : J_Tile
    {
        // --------------- EVENT --------------- //
        public Action OnMapGenerated;

        // --------------- GRID --------------- //
        [FoldoutGroup("Grid", false, -10), ShowInInspector] private T[] _AllTiles { get; set; } = new T[9];

        // --------------- STATE --------------- //
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public Grid ThisGrid { get; private set; }
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public int Width { get; private set; }
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public int Height { get; private set; }
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public int TotalCells => Height * Width;

        // --------------- MAP CONSTRUCTION --------------- //
        /// <summary>
        /// initiates the map with the starting array
        /// </summary>
        public void InitiateMap(Grid grid, T[] tiles, int width)
        {
            Validate(grid, tiles, width);
            ThisGrid  = grid;
            Width     = width;
            Height    = tiles.Length / Width;
            _AllTiles = new T[Width * Height];

            for (int i = 0; i < tiles.Length; i++)
            {
                int x = i % Width;
                int y = i / Width;
                _AllTiles[y * Width + x] = tiles[i];
            }

            OnMapGenerated?.Invoke();
        }

        private void Validate(Grid grid, T[] tiles, int width)
        {
            if (grid == null) throw new ArgumentNullException($"{name} requires a {nameof(Grid)}");
            if (tiles == null ||
                !tiles.ArrayIsValid()) throw new ArgumentNullException($"{name} non empty array required for {nameof(_AllTiles)}");

            if (width <= 0) throw new ArgumentException($"{name} width requires to be more than 0");

            if (tiles.Length % width != 0)
                JLog.Warning($"{name} given {nameof(tiles)} is not divisible its width {width}. Maybe not enough columns?");
        }

        // --------------- QUERIES --------------- //
        /// <summary>
        /// retrieves a tile from the given coordinates
        /// </summary>
        public T GetTile(int x, int y) => _AllTiles[y * Width + x];

        public T GetTile(Vector2Int v) => GetTile(v.x, v.y);

        /// <summary>
        /// retrieves a tile from the given world position
        /// </summary>
        public Vector3Int GetCoordinateFromPosition(Vector3 position) => ThisGrid.WorldToCell(position);

        public bool ValueChecks(int x, int y)
        {
            if (x < 0 ||
                x > Width)
            {
                JLog.Warning($"{name} invalid {nameof(x)} = {x}. Min = 0 - Max = {Width}", JLogTags.GameBoard, this);
                return false;
            }

            if (y < 0 ||
                y > Height)
            {
                JLog.Warning($"{name} invalid {nameof(y)} = {y}. Min = 0 - Max = {Height}", JLogTags.GameBoard, this);
                return false;
            }

            return true;
        }
    }
}
