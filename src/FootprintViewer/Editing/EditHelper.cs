using Mapsui;
using Mapsui.UI;
using Mapsui.Utilities;
using System.Collections.Generic;

namespace FootprintViewer
{
    public static class EditHelper
    {
        /// <summary>
        /// Inserts a vertex into a list of vertices at the location of the MapInfo location.
        /// It fails if the distance of the location to the line is larger thatn the screenDistance.
        /// </summary>
        /// <param name="mapInfo">The MapInfo object that contains the location</param>
        /// <param name="vertices">The list of vertices to insert into</param>
        /// <param name="screenDistance"></param>
        /// <returns></returns>
        public static bool TryInsertVertex(MapInfo mapInfo, IList<MPoint> vertices, double screenDistance)
        {
            var (distance, segment) = GetDistanceAndSegment(mapInfo.WorldPosition, vertices);
            if (IsCloseEnough(distance, mapInfo.Resolution, screenDistance))
            {
                vertices.Insert(segment + 1, mapInfo.WorldPosition.Copy());
                return true;
            }
            return false;
        }

        private static bool IsCloseEnough(double distance, double resolution, double screenDistance)
        {
            return distance <= resolution * screenDistance;
        }

        private static (double Distance, int segment) GetDistanceAndSegment(MPoint point, IList<MPoint> points)
        {
            // Move this to Mapsui

            var minDist = double.MaxValue;
            int segment = 0;

            for (var i = 0; i < points.Count - 1; i++)
            {
                var dist = /*CGAlgorithms*/Algorithms.DistancePointLine(point, points[i], points[i + 1]);
                if (dist < minDist)
                {
                    minDist = dist;
                    segment = i;
                }
            }

            return (minDist, segment);
        }
    }
}
