using System;
using Sirenix.OdinInspector;
using Unity.Mathematics;
using UnityEngine;

namespace JReact.Tilemaps
{
    public readonly struct JTile : IEquatable<JTile>
    {
        private const string _FoldoutGroupTitle = "Tile";
        private static readonly JTile _DefaultTile;

        [FoldoutGroup(_FoldoutGroupTitle, false, 5), ReadOnly, ShowInInspector] public readonly Vector3Int cellPosition;
        [FoldoutGroup(_FoldoutGroupTitle, false, 5), ReadOnly, ShowInInspector] public readonly int id;
        [FoldoutGroup(_FoldoutGroupTitle, false, 5), ReadOnly, ShowInInspector] public readonly int weight;

        static JTile() { _DefaultTile = new JTile(default, default, default); }

        public JTile(Vector3Int cellPosition, int id, int weight = 0)
        {
            this.cellPosition  = cellPosition;
            this.id            = id;
            this.weight        = weight;
        }

        public bool IsDefault() => this.Equals(_DefaultTile);

        public bool Equals(JTile other)
            => cellPosition.Equals(other.cellPosition) && id == other.id;

        public override string ToString() => $"{cellPosition}_({id})";

        private int ConvertToIndex(int2 adjustments, int width)
        {
            int x = cellPosition.x + adjustments.x;
            int y = cellPosition.y + adjustments.y;
            return (y * width) + x;
        }

        public int ConvertToIndex(J_Mono_MainTileBoard board) => ConvertToIndex(board.StartPoint.ToInt2(), board.Width);

        public override bool Equals(object obj) { return obj is JTile other && Equals(other); }

        public override int GetHashCode() => HashCode.Combine(cellPosition, id);

        public static bool operator ==(JTile                 left, JTile right) => left.Equals(right);
        public static bool operator !=(JTile                 left, JTile right) => !left.Equals(right);
        public        Vector3 ToWorldPosition(J_Mono_MapGrid mapGrid) => mapGrid.GetWorldPosition(this);
    }
}
