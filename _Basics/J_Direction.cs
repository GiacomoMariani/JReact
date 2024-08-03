using System;
using Sirenix.OdinInspector;

namespace JReact
{
    public enum J_DirectionEnum : byte { Left = 0, East = 0, Down = 0, Center = 1, North = 2, Up = 2, Right = 2 }

    public enum J_AllDirectionEnum : byte
    {
        UpLeft = 0,
        NorthWest = 0,
        Up = 1,
        North = 1,
        UpRight = 2,
        NorthEast = 2,
        Left = 3,
        West = 3,
        Center = 4,
        Mid = 4,
        Right = 5,
        East = 5,
        DownLeft = 6,
        SouthWest = 6,
        Down = 7,
        South = 7,
        DownRight = 8,
        SouthEast = 8
    }

    [Serializable]
    public readonly struct J_Direction : IEquatable<J_Direction>
    {
        [ReadOnly, ShowInInspector] public readonly J_DirectionEnum HorizontalState;
        [ReadOnly, ShowInInspector] public readonly J_DirectionEnum VerticalState;
        [ReadOnly, ShowInInspector] public float HorizontalValue => 0.5f * (float)HorizontalState;
        [ReadOnly, ShowInInspector] public float VerticalValue => 0.5f   * (float)VerticalState;

        public J_Direction(J_DirectionEnum horizontal, J_DirectionEnum vertical)
        {
            HorizontalState = horizontal;
            VerticalState   = vertical;
        }

        public static readonly J_Direction Center = new J_Direction(J_DirectionEnum.Center,   J_DirectionEnum.Center);
        public static readonly J_Direction Up = new J_Direction(J_DirectionEnum.Center,       J_DirectionEnum.Up);
        public static readonly J_Direction Down = new J_Direction(J_DirectionEnum.Center,     J_DirectionEnum.Down);
        public static readonly J_Direction Left = new J_Direction(J_DirectionEnum.Left,       J_DirectionEnum.Center);
        public static readonly J_Direction Right = new J_Direction(J_DirectionEnum.Right,     J_DirectionEnum.Center);
        public static readonly J_Direction UpRight = new J_Direction(J_DirectionEnum.Right,   J_DirectionEnum.Up);
        public static readonly J_Direction UpLeft = new J_Direction(J_DirectionEnum.Left,     J_DirectionEnum.Up);
        public static readonly J_Direction DownRight = new J_Direction(J_DirectionEnum.Right, J_DirectionEnum.Down);
        public static readonly J_Direction DownLeft = new J_Direction(J_DirectionEnum.Left,   J_DirectionEnum.Down);

        public bool Equals(J_Direction other) => HorizontalState == other.HorizontalState && VerticalState == other.VerticalState;

        public override bool Equals(object obj) => obj is J_Direction other && Equals(other);

        public override int GetHashCode() => HashCode.Combine((int)HorizontalState, (int)VerticalState);

        public override string ToString() => $"{HorizontalState} - {VerticalState} ({HorizontalValue} {VerticalValue})";
    }

    public static class J_DirectionExtensions
    {
        public static J_Direction ToDirection(this J_AllDirectionEnum directionEnum)
        {
            switch (directionEnum)
            {
                case J_AllDirectionEnum.UpLeft:    return J_Direction.UpLeft;
                case J_AllDirectionEnum.Up:        return J_Direction.Up;
                case J_AllDirectionEnum.UpRight:   return J_Direction.UpRight;
                case J_AllDirectionEnum.Left:      return J_Direction.Left;
                case J_AllDirectionEnum.Center:    return J_Direction.Center;
                case J_AllDirectionEnum.Right:     return J_Direction.Right;
                case J_AllDirectionEnum.DownLeft:  return J_Direction.DownLeft;
                case J_AllDirectionEnum.Down:      return J_Direction.Down;
                case J_AllDirectionEnum.DownRight: return J_Direction.DownRight;
                default:                           throw new ArgumentOutOfRangeException(nameof(directionEnum), directionEnum, null);
            }
        }
    }
}
