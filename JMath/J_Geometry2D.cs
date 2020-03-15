using System;
using UnityEngine;

namespace JMath2D
{
    //3D Line Intersection Algorithm
    //http://inis.jinr.ru/sl/vol1/CMC/Graphics_Gems_1,ed_A.Glassner.pdf
    public struct J_Line
    {
        public enum LineType { Line2D, Ray2D, Segment2D };

        public readonly Vector2 A;
        public readonly Vector2 B;
        public readonly Vector2 Vector;
        public readonly LineType Type;

        public J_Line(Vector2 a, Vector2 b, LineType type = LineType.Line2D)
        {
            A      = a;
            B      = b;
            Type   = type;
            Vector = new Vector2(B.x - A.x, B.y - A.y);
        }

        public J_Line(Vector2 position, Vector2 vector)
        {
            A      = position;
            B      = position + vector;
            Type   = LineType.Segment2D;
            Vector = vector;
        }

        public Vector2 Reflect(Vector2 normal)
        {
            Vector2 normalized       = normal.normalized;
            Vector2 vectorNormalized = Vector.normalized;

            float dotProduct = normalized.Dot(vectorNormalized);

            if (Math.Abs(dotProduct) < J_Math2D.Tolerance) return Vector;

            float doubleDotProduct = dotProduct * 2;
            return vectorNormalized - normalized * doubleDotProduct;
        }

        public float IntersectsAt(J_Line line)
        {
            Vector2 perpendicular  = line.Vector.GetPerpendicular();
            float   parallelFactor = perpendicular.Dot(Vector);
            if (Math.Abs(parallelFactor) < J_Math2D.Tolerance) { return float.NaN; }

            Vector2 c                = line.A - this.A;
            float   dotPerpendicular = perpendicular.Dot(c);
            float   t                = dotPerpendicular / parallelFactor;

            if ((t < 0 || t > 1) &&
                Type == LineType.Segment2D) { return float.NaN; }

            if (t    < 0 &&
                Type == LineType.Ray2D) { return float.NaN; }

            return t;
        }

        public Vector2 Lerp(float t)
        {
            switch (Type)
            {
                case LineType.Segment2D:
                    t = Mathf.Clamp(t, 0, 1);
                    break;
                case LineType.Ray2D when t < 0:
                    t = 0;
                    break;
            }

            float xt = A.x + Vector.x * t;
            float yt = A.y + Vector.y * t;

            return new Vector2(xt, yt);
        }
    }

    public struct Plane2D
    {
        public readonly Vector2 A;
        public readonly Vector2 B;
        public readonly Vector2 C;
        public readonly Vector2 V;
        public readonly Vector2 U;

        public Plane2D(Vector2 pointA, Vector2 b, Vector2 c, bool fromVectors = false)
        {
            A = pointA;
            if (!fromVectors)
            {
                B = b;
                C = c;
                V = B - this.A;
                U = C - this.A;
            }
            else
            {
                A = pointA;
                this.V = new Vector2(b.x, b.y);
                this.U = new Vector2(c.x, c.y);
                B      = A + V;
                C      = A + U;
            }
        }

        public Vector2 Lerp(float s, float t)
        {
            float xt = A.x + V.x * s + U.x * t;
            float yt = A.y + V.y * s + U.y * t;

            return new Vector2(xt, yt);
        }
    }
}
