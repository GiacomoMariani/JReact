using System;
using Sirenix.OdinInspector;
using Unity.Mathematics;
using UnityEngine;

namespace JReact.Tilemaps
{
    public readonly struct JTile : IEquatable<JTile>
    {
        private const string FoldoutGroupTitle = "Tile";

        [FoldoutGroup(FoldoutGroupTitle, false, 5), ReadOnly, ShowInInspector] public readonly int2 cellPosition;
        [FoldoutGroup(FoldoutGroupTitle, false, 5), ReadOnly, ShowInInspector] public readonly float2 worldPosition;
        [FoldoutGroup(FoldoutGroupTitle, false, 5), ReadOnly, ShowInInspector] public readonly int id;
        [FoldoutGroup(FoldoutGroupTitle, false, 5), ReadOnly, ShowInInspector] public readonly int weight;
        public static readonly JTile DefaultTile = new JTile(default, default, default);

        public JTile(Vector3Int cellPosition, Vector3 worldPosition, int id, int weight = 0)
        {
            this.cellPosition  = cellPosition.ToInt2();
            this.worldPosition = worldPosition.ToFloat2();
            this.id            = id;
            this.weight        = weight;
        }

        public bool IsDefault() => this.Equals(DefaultTile);

        public bool Equals(JTile other)
            => cellPosition.Equals(other.cellPosition) && worldPosition.Equals(other.worldPosition) && id == other.id;

        public override string ToString() => $"{cellPosition}_({id}), World Pos: {worldPosition}";

        public int ConvertToIndex(int2 adjustments, int width)
        {
            int x = cellPosition.x + adjustments.x;
            int y = cellPosition.y + adjustments.y;
            return (y * width) + x;
        }

        public int ConvertToIndex(J_Mono_MainTileBoard board) => ConvertToIndex(board.StartPoint.ToInt2(), board.Width);

        public override bool Equals(object obj) { return obj is JTile other && Equals(other); }

        public override int GetHashCode() => HashCode.Combine(cellPosition, worldPosition, id);

        public static bool operator ==(JTile left, JTile right) => left.Equals(right);
        public static bool operator !=(JTile left, JTile right) => !left.Equals(right);
    }
}
