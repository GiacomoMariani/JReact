using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.Tilemaps
{
    /// <summary>
    /// An inclusive rectangular range of grid coordinates (Min..Max on both axes) — the bounding box
    /// used for region queries, iteration, and framing. A Location's valid cells are usually an
    /// irregular subset of its bounds, so always confirm validity against the grid model, not the box.
    /// </summary>
    public readonly struct JGridBounds
    {
        public readonly JCoord Min;
        public readonly JCoord Max;

        public JGridBounds(JCoord min, JCoord max)
        {
            Min = min;
            Max = max;
        }

        public int Width  => Max.X - Min.X + 1;
        public int Height => Max.Y - Min.Y + 1;

        public bool Contains(JCoord c) => c.X >= Min.X && c.X <= Max.X && c.Y >= Min.Y && c.Y <= Max.Y;

        /// <summary>
        /// The equivalent single-plane <see cref="BoundsInt"/> (origin Min, z=0, depth 1) for tilemap block APIs
        /// such as <c>Tilemap.SetTilesBlock</c>. Its volume matches the iteration count, and the expected
        /// x-fastest-then-y tile order matches this struct's <see cref="GetEnumerator"/>.
        /// </summary>
        public BoundsInt ToBoundsInt() => new BoundsInt(Min.X, Min.Y, 0, Width, Height, 1);

        /// <summary>This box grown outward by <paramref name="margin"/> cells on every side (negative shrinks).</summary>
        public JGridBounds Expand(int margin)
            => new JGridBounds(new JCoord(Min.X - margin, Min.Y - margin), new JCoord(Max.X + margin, Max.Y + margin));

        /// <summary>
        /// Every coordinate in the box, row by row (bottom-up). Lazy — filter against the grid mask.
        /// Allocates a managed iterator; for hot or Burst paths use <c>foreach (JCoord c in bounds)</c>
        /// over the struct <see cref="GetEnumerator"/> instead.
        /// </summary>
        public IEnumerable<JCoord> AllCoords()
        {
            for (int y = Min.Y; y <= Max.Y; y++)
            {
                for (int x = Min.X; x <= Max.X; x++) { yield return new JCoord(x, y); }
            }
        }

        /// <summary>
        /// Zero-allocation, Burst-friendly iteration: <c>foreach (JCoord c in bounds)</c>. Walks the box
        /// row by row (bottom-up), same order as <see cref="AllCoords"/>. Pattern-based struct enumerator —
        /// no IEnumerable boxing. The loop body must itself be Burst-legal (use <c>c.X</c>/<c>c.Y</c>,
        /// not <c>ToVector3Int()</c>, inside jobs).
        /// </summary>
        public Enumerator GetEnumerator() => new Enumerator(this);

        /// <summary>Struct enumerator for <see cref="JGridBounds"/>. No <c>IDisposable</c>, so foreach emits no try/finally.</summary>
        public struct Enumerator
        {
            private readonly int _minX, _maxX, _maxY;
            private int          _x, _y;

            internal Enumerator(JGridBounds b)
            {
                _minX = b.Min.X;
                _maxX = b.Max.X;
                _maxY = b.Max.Y;
                _x    = b.Min.X - 1;                                            // before the first column
                _y    = (b.Max.X < b.Min.X || b.Max.Y < b.Min.Y) ? b.Max.Y + 1 // empty box -> already exhausted
                                                                  : b.Min.Y;
            }

            public JCoord Current => new JCoord(_x, _y);

            public bool MoveNext()
            {
                _x++;
                if (_x <= _maxX) { return _y <= _maxY; }
                _x = _minX;
                _y++;
                return _y <= _maxY;
            }
        }

        /// <summary>The tight bounding box around a non-empty set of coordinates.</summary>
        public static JGridBounds Encapsulate(IEnumerable<JCoord> coords)
        {
            int minX = int.MaxValue, minY = int.MaxValue, maxX = int.MinValue, maxY = int.MinValue;
            bool any = false;
            foreach (JCoord c in coords)
            {
                any = true;
                if (c.X < minX) { minX = c.X; }
                if (c.Y < minY) { minY = c.Y; }
                if (c.X > maxX) { maxX = c.X; }
                if (c.Y > maxY) { maxY = c.Y; }
            }

            Assert.IsTrue(any, "JGridBounds.Encapsulate: no coordinates supplied.");
            return new JGridBounds(new JCoord(minX, minY), new JCoord(maxX, maxY));
        }
    }
}
