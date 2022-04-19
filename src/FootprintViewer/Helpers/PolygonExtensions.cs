using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FootprintViewer
{   
    public static class PolygonExtensions
    {
        public static bool Intersection(this Polygon parent, Polygon child, bool isFullCover = false)
        {
            var points1 = parent.ExteriorRing.Coordinates.Select(s => new Point(s)).ToList();//Vertices;
            var points2 = child.ExteriorRing.Coordinates.Select(s => new Point(s)).ToList();//Vertices;

            bool fullContain = false;

            foreach (var item in points2)
            {
                if (parent.Contains(item) == false)
                {
                    fullContain = false;
                    break;
                }

                fullContain = true;
            }

            if (fullContain == true)
            {
                return true;
            }

            if (isFullCover == true)
            {
                return fullContain;
            }

            IList<(Point, Point)> lines1 = new List<(Point, Point)>();
            IList<(Point, Point)> lines2 = new List<(Point, Point)>();

            for (int i = 0; i < points1.Count; i++)
            {
                if (i == points1.Count - 1)
                {
                    lines1.Add((points1[i], points1[0]));
                }
                else
                {
                    lines1.Add((points1[i], points1[i + 1]));
                }
            }

            for (int i = 0; i < points2.Count; i++)
            {
                if (i == points2.Count - 1)
                {
                    lines2.Add((points2[i], points2[0]));
                }
                else
                {
                    lines2.Add((points2[i], points2[i + 1]));
                }
            }

            foreach (var (p1, p2) in lines1)
            {
                foreach (var (q1, q2) in lines2)
                {
                    var res = DoIntersect(p1, p2, q1, q2);

                    if (res == true)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        // Given three collinear points p, q, r, the function checks if point q lies on line segment 'pr'
        private static bool onSegment(Point p, Point q, Point r)
        {
            if (q.X <= Math.Max(p.X, r.X) && q.X >= Math.Min(p.X, r.X) &&
                q.Y <= Math.Max(p.Y, r.Y) && q.Y >= Math.Min(p.Y, r.Y))
                return true;

            return false;
        }

        // To find orientation of ordered triplet (p, q, r).
        // The function returns following values
        // 0 --> p, q and r are collinear
        // 1 --> Clockwise
        // 2 --> Counterclockwise
        private static int Orientation(Point p, Point q, Point r)
        {
            // See https://www.geeksforgeeks.org/orientation-3-ordered-points/
            // for details of below formula.
            double val = (q.Y - p.Y) * (r.X - q.X) -
                    (q.X - p.X) * (r.Y - q.Y);

            if (val == 0)
                return 0; // collinear

            return (val > 0) ? 1 : 2; // clock or counterclock wise
        }

        // The main function that returns true if line segment 'p1q1' and 'p2q2' intersect.
        private static bool DoIntersect(Point p1, Point q1, Point p2, Point q2)
        {
            // Find the four orientations needed for general and
            // special cases
            int o1 = Orientation(p1, q1, p2);
            int o2 = Orientation(p1, q1, q2);
            int o3 = Orientation(p2, q2, p1);
            int o4 = Orientation(p2, q2, q1);

            // General case
            if (o1 != o2 && o3 != o4)
                return true;

            // Special Cases
            // p1, q1 and p2 are collinear and p2 lies on segment p1q1
            if (o1 == 0 && onSegment(p1, p2, q1))
                return true;

            // p1, q1 and q2 are collinear and q2 lies on segment p1q1
            if (o2 == 0 && onSegment(p1, q2, q1))
                return true;

            // p2, q2 and p1 are collinear and p1 lies on segment p2q2
            if (o3 == 0 && onSegment(p2, p1, q2))
                return true;

            // p2, q2 and q1 are collinear and q1 lies on segment p2q2
            if (o4 == 0 && onSegment(p2, q1, q2))
                return true;

            return false; // Doesn't fall in any of the above cases
        }
    }
}
