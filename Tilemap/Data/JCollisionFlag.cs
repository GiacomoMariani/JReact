using System;

namespace JReact.Tilemaps
{
    [Flags]
    public enum JCollisionFlag
    {
        None = 0,
        //terrain
        LandWild = 1 << 0,
        Land = 1     << 1,
        Water = 1    << 2,
        Stone = 1    << 3,
        Lava = 1     << 4,

        //items
        ItemOnGround = 1 << 8,
        TallItem = 1     << 9,

        //specials
        AirWind = 1  << 16,
        Boundary = 1 << 31,

        //combined
        WalkObstacles = Water     | Stone    | Lava  | ItemOnGround | TallItem | Boundary,
        WaterObstacles = LandWild | Land     | Stone | Lava         | Stone    | Lava | ItemOnGround | TallItem | Boundary | Boundary,
        FlyObstacles = AirWind    | TallItem | Boundary,
    }

    public static class JCollisionFlagExtensions
    {
        public static int ToBitFlag(this JCollisionFlag flag) => (int)flag;

        public static JCollisionFlag Combine(this JCollisionFlag flag1, JCollisionFlag flag2) => flag1 | flag2;

        public static bool HasCollisionWith(this JCollisionFlag flag1, JCollisionFlag flag2) => (flag1 & flag2) != JCollisionFlag.None;
    }
}
