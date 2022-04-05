using FootprintViewer.Data.Science;
using Mapsui.Geometries;
using System.Collections.Generic;
using System.Linq;

namespace FootprintViewer.Data
{
    internal class GroundStationResult
    {
        public Point Center { get; init; } = new Point();

        public double InnerAngle { get; init; }

        public double OuterAngle { get; init; }

        public IList<IEnumerable<IEnumerable<Point>>> Areas { get; init; } = new List<IEnumerable<IEnumerable<Point>>>();

        public IEnumerable<IEnumerable<Point>> InnerBorder { get; init; } = new List<IEnumerable<Point>>();

        public IEnumerable<IEnumerable<Point>> OuterBorder { get; init; } = new List<IEnumerable<Point>>();
    }

    internal static class GroundStationBuilder
    {
        private static readonly double[] _defaultAngles = new[] { 0.0, 10.0 };

        public static IList<GroundStationResult> Create(IEnumerable<GroundStation> groundStations)
        {
            var list = new List<GroundStationResult>();

            foreach (var gs in groundStations)
            {
                if (gs.Center != null)
                {
                    var lon = gs.Center.X;
                    var lat = gs.Center.Y;
                    var (isHole, angles) = Verify(gs.Angles);
                    var circles = EarthGeometry.BuildCircles(lon, lat, angles);

                    list.Add(new GroundStationResult()
                    {
                        Center = new Point(lon, lat),
                        InnerAngle = (isHole == false) ? 0.0 : angles.First(),
                        OuterAngle = angles.Last(),
                        Areas = circles.Select(s => s.Areas).ToList(),
                        InnerBorder = (isHole == false) ? new List<IEnumerable<Point>>() : circles.First().Borders,
                        OuterBorder = circles.Last().Borders,
                    });
                }
            }

            return list;
        }

        private static (bool isHole, double[] angles) Verify(double[] anglesSource)
        {
            var list = new List<double>(anglesSource);

            list.Sort();

            if (list.Count == 1)
            {
                list.Insert(0, 0.0);
            }

            if (list.Where(s => s != 0.0).Any() == false)
            {
                list.Clear();
                list.AddRange(_defaultAngles);
            }

            var first = list.First();

            var isHole = (first != 0.0);

            var angles = list.Where(s => s != 0.0).ToArray();

            return (isHole, angles);
        }
    }
}
