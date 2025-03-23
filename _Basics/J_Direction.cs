using System;
using Sirenix.OdinInspector;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact
{
    public enum J_DirectionEnum : byte
    {
        Left = 0,
        East = 0,
        Down = 0,
        Center = 1,
        North = 2,
        Up = 2,
        Right = 2,
        MaxVaalue = 2
    }

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
        [ReadOnly, ShowInInspector] public int ClockWiseIndex => Array.IndexOf(ClockWiseDirections, this);

        public J_Direction(J_DirectionEnum horizontal, J_DirectionEnum vertical)
        {
            HorizontalState = horizontal;
            VerticalState   = vertical;
        }

        private static readonly J_Direction[] ClockWiseDirections;
        public static readonly J_Direction Center;
        public static readonly J_Direction Up;
        public static readonly J_Direction Down;
        public static readonly J_Direction Left;
        public static readonly J_Direction Right;
        public static readonly J_Direction UpRight;
        public static readonly J_Direction UpLeft;
        public static readonly J_Direction DownRight;
        public static readonly J_Direction DownLeft;

        static J_Direction()
        {
            Center              = new J_Direction(J_DirectionEnum.Center, J_DirectionEnum.Center);
            Up                  = new J_Direction(J_DirectionEnum.Center, J_DirectionEnum.Up);
            Down                = new J_Direction(J_DirectionEnum.Center, J_DirectionEnum.Down);
            Left                = new J_Direction(J_DirectionEnum.Left,   J_DirectionEnum.Center);
            Right               = new J_Direction(J_DirectionEnum.Right,  J_DirectionEnum.Center);
            UpRight             = new J_Direction(J_DirectionEnum.Right,  J_DirectionEnum.Up);
            UpLeft              = new J_Direction(J_DirectionEnum.Left,   J_DirectionEnum.Up);
            DownRight           = new J_Direction(J_DirectionEnum.Right,  J_DirectionEnum.Down);
            DownLeft            = new J_Direction(J_DirectionEnum.Left,   J_DirectionEnum.Down);
            ClockWiseDirections = new[] { Up, UpRight, Right, DownRight, Down, DownLeft, Left, UpLeft };
        }

        // --------------- INTERACTORS --------------- //
        /// Reverses the current direction to its opposite counterpart.
        /// The horizontal and vertical states of the current direction are reversed based on their predefined opposite values.
        /// <return>Returns a new J_Direction instance representing the reversed direction.</return>
        public J_Direction GoBack() => new J_Direction(HorizontalState.Reverse(), VerticalState.Reverse());

        /// Rotates the current direction by a specified number of steps within the sequence of pre-defined directions.
        /// A positive value rotates the direction clockwise, while a negative value rotates it counter-clockwise.
        /// If the direction is Center, the result remains Center.
        /// <param name="steps">The number of rotational steps to apply. Positive values rotate clockwise, negative values counter-clockwise.</param>
        /// <return>Returns the new direction obtained after applying the specified number of rotational steps.</return>
        public J_Direction Turn(int steps)
        {
            if (this == Center) return Center;

            int clockWiseIndex = ClockWiseIndex;
            Assert.IsTrue(clockWiseIndex != -1, $"Direction {this} not found in {ClockWiseDirections.PrintAll()}");

            int newIndex = (clockWiseIndex + steps) % ClockWiseDirections.Length;
            if (newIndex < 0) { newIndex += ClockWiseDirections.Length; }

            return ClockWiseDirections[newIndex];
        }

        /// Rotates the current direction 90 degrees to the right (clockwise).
        /// For example, turning right from Up will result in UpRight.
        /// <return>Returns the new direction after rotating 90 degrees to the right.</return>
        public J_Direction TurnRight() => Turn(2);

        /// Rotates the current direction 90 degrees to the left (counter-clockwise).
        /// For example, turning left from Up will result in UpLeft.
        /// <return>Returns the new direction after rotating 90 degrees to the left.</return>
        public J_Direction TurnLeft() => Turn(-2);

        // --------------- INTERFACE IMPLEMENTATION --------------- //
        public bool Equals(J_Direction other) => HorizontalState == other.HorizontalState && VerticalState == other.VerticalState;

        public override bool Equals(object obj) => obj is J_Direction other && Equals(other);

        public static bool operator ==(J_Direction left, J_Direction right) => left.Equals(right);

        public static bool operator !=(J_Direction left, J_Direction right) => !left.Equals(right);

        public override int GetHashCode() => HashCode.Combine((byte)HorizontalState, (byte)VerticalState);

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

        public static J_DirectionEnum Reverse(this J_DirectionEnum directionEnum)
            => (J_DirectionEnum)((byte)J_DirectionEnum.MaxVaalue - (byte)directionEnum);

        public static int2 AddDirection(this int2 start, J_Direction direction, int amount = 1)
        {
            int x = start.x;
            switch (direction.HorizontalState)
            {
                case J_DirectionEnum.Left:  x -= amount; break;
                case J_DirectionEnum.Right: x += amount; break;
            }

            var y = start.y;
            switch (direction.VerticalState)
            {
                case J_DirectionEnum.Down: y -= amount; break;
                case J_DirectionEnum.Up:   y += amount; break;
            }

            return new int2(x, y);
        }
    }
}
