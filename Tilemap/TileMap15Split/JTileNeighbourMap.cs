using System;
using System.Collections.Generic;
using UnityEngine;

namespace JReact.Tilemaps.Split15
{
    public static class JTileNeighbourMap
    {
        public static readonly Vector3Int[] Neighbours = { new(0, 0, 0), new(1, 0, 0), new(0, 1, 0), new(1, 1, 0) };
        
        // This dictionary stores the "rules", each 4-neighbour configuration corresponds to a tile
        // |_1_|_2_|
        // |_3_|_4_|
        //true is GRASS
        //false is NONE
        public static readonly Dictionary<Tuple<bool, bool, bool, bool>, int> NeighbourTupleToTile = new()
        {
            { new(true, true, true, true), 6 },      // CENTRAL
            { new(false, false, false, true), 13 },  // OUTER_BOTTOM_RIGHT
            { new(false, false, true, false), 0 },   // OUTER_BOTTOM_LEFT
            { new(false, true, false, false), 8 },   // OUTER_TOP_RIGHT
            { new(true, false, false, false), 15 },  // OUTER_TOP_LEFT
            { new(false, true, false, true), 1 },    // EDGE_RIGHT
            { new(true, false, true, false), 11 },   // EDGE_LEFT
            { new(false, false, true, true), 3 },    // EDGE_BOTTOM
            { new(true, true, false, false), 9 },    // EDGE_TOP
            { new(false, true, true, true), 5 },     // INNER_BOTTOM_RIGHT
            { new(true, false, true, true), 2 },     // INNER_BOTTOM_LEFT
            { new(true, true, false, true), 10 },    // INNER_TOP_RIGHT
            { new(true, true, true, false), 7 },     // INNER_TOP_LEFT
            { new(false, true, true, false), 14 },   // DUAL_UP_RIGHT
            { new(true, false, false, true), 4 },    // DUAL_DOWN_RIGHT
            { new(false, false, false, false), 12 }, // NO TILE AROUND
        };
    }
}
