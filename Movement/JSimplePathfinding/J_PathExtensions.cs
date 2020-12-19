using System.Collections.Generic;

namespace JReact.Pathfinding
{
    public static class J_PathExtensions
    {
        public static int GetPathLength<T>(this List<T> path)
            where T : J_PathNode
        {
            if (path == null) return -1;
            int length = 0;

            for (int i = 0; i < path.Count; i++) { length += path[i].GetWeight(); }

            return length;
        }
    }
}
