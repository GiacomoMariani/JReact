using System;

namespace JReact
{
    public enum Direction { Up, Right, Down, Left, None }

    [Flags] enum J_Direction { Right = 1 << 0, East = 1 << 0, Up = 1 << 1, North = 1 << 1 }

    public enum OperatorType { And, Or }
    public enum ComparisonType { Less, Equal, More }
}
