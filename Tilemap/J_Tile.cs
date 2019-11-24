using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Tilemaps;

namespace JReact.Tilemaps
{
    [Serializable]
    public class J_Tile
    {
        [HideInInspector] public Action<(J_TileInfo previous, J_TileInfo next)> OnGroundChanged;
        [HideInInspector] public Action<(J_TileInfo[] previous, J_TileInfo[] next)> OnOverGroundChanged;

        [FoldoutGroup("Tile", false, 5), ReadOnly, ShowInInspector] public Vector3Int CellPosition { get; private set; }
        [FoldoutGroup("Tile", false, 5), ReadOnly, ShowInInspector] public Vector2 WorldPosition { get; private set; }
        [FoldoutGroup("Tile", false, 5), ReadOnly, ShowInInspector] public J_TileInfo Ground { get; private set; }
        [FoldoutGroup("Tile", false, 5), ReadOnly, ShowInInspector] public J_TileInfo[] OverGround { get; private set; }

        [FoldoutGroup("Tile", false, 5), ReadOnly, ShowInInspector] protected J_Mono_TileDrawer _tileDrawer { get; private set; }

        /// <summary>
        /// tile constructor
        /// </summary>
        public J_Tile(Vector3Int cellPosition, J_Mono_TileDrawer tileDrawer, J_TileInfo ground, J_TileInfo[] overground)
        {
            (_tileDrawer, CellPosition) = (tileDrawer, cellPosition);
            WorldPosition               = tileDrawer.GetWorldPosition(cellPosition);
            SetGround(ground);
            if (overground != null) SetOverGround(overground);
        }

        // --------------- VIEW COMMAND --------------- //
        private void DrawTile(J_TileInfo tileInfo)
        {
            Assert.IsNotNull(tileInfo, $"{this} - requires a {nameof(tileInfo)}");
            _tileDrawer.DrawTile(tileInfo, CellPosition);
        }
        
        // --------------- COMMANDS --------------- //
        /// <summary>
        /// changes the ground with the new one
        /// </summary>
        /// <param name="newGround"></param>
        public void SetGround(J_TileInfo newGround)
        {
            // --------------- DRAW --------------- //
            DrawTile(newGround);

            // --------------- LOGIC --------------- //
            J_TileInfo oldGround = Ground;
            Ground = newGround;
            OnGroundChanged?.Invoke((oldGround, newGround));
        }

        public void SetOverGround(J_TileInfo[] newOverGround)
        {
            // --------------- VIEW --------------- //
            for (int i = 0; i < newOverGround.Length; i++) DrawTile(newOverGround[i]);

            // --------------- LOGIC --------------- //
            var oldGround = OverGround;
            OverGround = newOverGround;
            OnOverGroundChanged?.Invoke((oldGround, newOverGround));
        }

        public override string ToString() => $"{CellPosition}_{Ground}\nWorld Pos: {WorldPosition}";
    }
}
