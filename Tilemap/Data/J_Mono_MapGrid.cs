using Sirenix.OdinInspector;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.Tilemaps
{
    //this element contains all the data inside the map
    public class J_Mono_MapGrid : MonoBehaviour
    {
        // --------------- GRID --------------- //
        [FoldoutGroup("Grid", false, -10), ShowInInspector] private JTile[] _allTiles;

        // --------------- STATE --------------- //
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public Grid ThisGrid { get; private set; }
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public int Width { get; private set; }
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public int Height { get; private set; }
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public int TotalCells => Height * Width;

        // --------------- MAP CONSTRUCTION --------------- //
        /// <summary>
        /// initiates the map with the starting array
        /// </summary>
        public void InitiateMap(Grid grid, JTile[] tiles, int width)
        {
            Validate(grid, tiles, width);
            ThisGrid  = grid;
            Width     = width;
            Height    = tiles.Length / Width;
            _allTiles = new JTile[Width * Height];

            for (int i = 0; i < tiles.Length; i++)
            {
                int x = i % Width;
                int y = i / Width;
                _allTiles[y * Width + x] = tiles[i];
            }
        }

        private void Validate(Grid grid, JTile[] tiles, int width)
        {
            Assert.IsNotNull(grid,  $"Requires a {nameof(grid)}");
            Assert.IsNotNull(tiles, $"{name} requires a {nameof(tiles)}");
            Assert.IsTrue(tiles.ArrayIsValid(), $"{name} non empty array required for {nameof(_allTiles)}");
            Assert.IsTrue(width > 0,            $"{name} width requires to be more than 0");
            Assert.IsTrue(tiles.Length % width == 0,
                          $"{name} given {nameof(tiles)} is not divisible its width {width}. Maybe not enough columns?");
        }

        // --------------- QUERIES --------------- //
        /// <summary>
        /// retrieves a tile from the given index
        /// </summary>
        public JTile GetTile(int index) => _allTiles[index];

        /// <summary>
        /// retrieves a tile from the given coordinates
        /// </summary>
        public JTile GetTile(int x, int y) => GetTile(y * Width + x);

        /// <summary>
        /// retrieves a tile from the given vector
        /// </summary>
        public JTile GetTile(Vector2Int v) => GetTile(v.x, v.y);

        /// <summary>
        /// retrieves a tile from the given int2
        /// </summary>
        public JTile GetTile(int2 v) => GetTile(v.x, v.y);

        /// <summary>
        /// retrieves a tile from the given world position
        /// </summary>
        public Vector3Int GetCoordinateFromPosition(Vector3 position) => ThisGrid.WorldToCell(position);

        public bool WithinBounds(int x, int y)
        {
            if (x < 0 ||
                x > Width) { return false; }

            if (y < 0 ||
                y > Height) { return false; }

            return true;
        }

        public bool WithinBounds(Vector2Int v) => WithinBounds(v.x, v.y);
    }
}
