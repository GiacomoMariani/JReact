using Sirenix.OdinInspector;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.Tilemaps
{
    //this element contains all the data inside the map
    public class J_Mono_MapGrid : MonoBehaviour
    {
        // --------------- GRID --------------- //
        [FoldoutGroup("Grid", false, -10), ShowInInspector] private NativeArray<JTile> _allTiles;

        // --------------- STATE --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField, ChildGameObjectsOnly, Required] private Grid _grid;
        public Grid Grid => _grid;
        [FoldoutGroup("State", false, 5), Sirenix.OdinInspector.ReadOnly, ShowInInspector] public int Width { get; private set; }
        [FoldoutGroup("State", false, 5), Sirenix.OdinInspector.ReadOnly, ShowInInspector] public int Height { get; private set; }
        [FoldoutGroup("State", false, 5), Sirenix.OdinInspector.ReadOnly, ShowInInspector] public int TotalCells => Height * Width;

        // --------------- MAP CONSTRUCTION --------------- //
        /// <summary>
        /// initiates the map with the starting array
        /// </summary>
        public void InitiateMap(NativeArray<JTile> tiles, int width)
        {
            Validate(tiles.AsReadOnly(), width);
            Width     = width;
            Height    = tiles.Length / Width;
            _allTiles = new NativeArray<JTile>(tiles.Length, Allocator.Persistent);
            NativeArray<JTile>.Copy(tiles ,_allTiles);

            for (int i = 0; i < tiles.Length; i++)
            {
                int x = i % Width;
                int y = i / Width;
                _allTiles[y * Width + x] = tiles[i];
            }
        }

        private void Validate(NativeArray<JTile>.ReadOnly tiles, int width)
        {
            Assert.IsTrue(tiles.IsCreated, $"{name} array not created for {nameof(_allTiles)}");
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
        /// Retrieves a tile from the given vector coordinates.
        /// </summary>
        private JTile GetTile(Vector3Int v)  => GetTile(v.x, v.y);

        /// <summary>
        /// retrieves a tile from the given int2
        /// </summary>
        public JTile GetTile(int2 v) => GetTile(v.x, v.y);

        /// <summary>
        /// retrieves the coordinate from the given world position
        /// </summary>
        public Vector3Int GetCoordinateFromWorld(Vector3 position) => Grid.WorldToCell(position);
        
        /// <summary>
        /// retrieves the tile from the given world position
        /// </summary>
        public JTile GetTileFromWorld(Vector3 position) => GetTile(Grid.WorldToCell(position));

        /// <summary>
        /// Converts the given tile's cell position to world position.
        /// </summary>
        public Vector3 GetWorldPosition(JTile tile) => Grid.GetCellCenterWorld(tile.cellPosition);

        /// <summary>
        /// Checks if the given x and y coordinates are within the bounds of the map.
        /// </summary>
        /// <param name="x">The x coordinate to check.</param>
        /// <param name="y">The y coordinate to check.</param>
        /// <returns>True if the coordinates are within bounds, otherwise false.</returns>
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
