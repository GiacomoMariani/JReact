using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.Tilemaps
{
    public sealed class J_Mono_MapBoundary : MonoBehaviour
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private J_TileInfo _boundaryTileInfo;

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private List<JTile> _boundaryTiles = new List<JTile>();

        internal void DrawBoundaries(J_Mono_MainTileBoard board, J_Mono_TilemapLayer ground)
        {
            Vector3Int startPoint = board.StartPoint;
            int        zPos       = board.ZPosision;
            // --------------- CALCULATE EDGES --------------- //
            int westEdge  = startPoint.x - 1;
            int eastEdge  = startPoint.x + board.Width;
            int southEdge = startPoint.y - 1;
            int northEdge = startPoint.y + board.Height;

            _boundaryTiles.Clear();

            // --------------- VERTICAL --------------- //
            for (int i = southEdge + 1; i < northEdge; i++)
            {
                Vector3Int boundaryWestPos = new Vector3Int(westEdge, i, zPos);
                Vector3Int boundaryEastPos = new Vector3Int(eastEdge, i, zPos);

                var westTile = CreateBoundary(board, boundaryWestPos, ground);
                _boundaryTiles.Add(westTile);
                var eastTile = CreateBoundary(board, boundaryEastPos, ground);
                _boundaryTiles.Add(eastTile);
            }

            // --------------- HORIZONTAL --------------- //
            for (int i = westEdge; i < eastEdge + 1; i++)
            {
                Vector3Int boundarySouthPos = new Vector3Int(i, southEdge, zPos);
                Vector3Int boundaryNorthPos = new Vector3Int(i, northEdge, zPos);

                var southTile = CreateBoundary(board, boundarySouthPos, ground);
                _boundaryTiles.Add(southTile);
                var northTile = CreateBoundary(board, boundaryNorthPos, ground);
                _boundaryTiles.Add(northTile);
            }
        }

        private JTile CreateBoundary(J_Mono_MainTileBoard board, Vector3Int position, J_Mono_TilemapLayer ground)
        {
            var tile = new JTile(position, board.GetWorldPosition(position), _boundaryTileInfo.TileInfoId);
            ground.DirectDrawTile(tile, _boundaryTileInfo);
            return tile;
        }
    }
}
