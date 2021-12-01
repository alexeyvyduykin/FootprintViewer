using FootprintViewer.Data.Science;
using NetTopologySuite.Geometries;
using System.Collections.Generic;
using System.Linq;

namespace FootprintViewer.Data
{
    internal static class StripBuilder
    {
        public static (Dictionary<string, Dictionary<int, List<List<Point>>>> left, Dictionary<string, Dictionary<int, List<List<Point>>>> right) Create(IEnumerable<Satellite> satellites)
        {
            var leftStrips = new Dictionary<string, Dictionary<int, List<List<Point>>>>();
            var rightStrips = new Dictionary<string, Dictionary<int, List<List<Point>>>>();

            foreach (var satellite in satellites)
            {
                var sat = satellite.ToPRDCTSatellite();
                var sensorLeft = satellite.ToPRDCTSensor(SatelliteStripDirection.Left);
                var sensorRight = satellite.ToPRDCTSensor(SatelliteStripDirection.Right);

                var band1 = new Band(sat.Orbit, sensorLeft.VerticalHalfAngleDEG, sensorLeft.RollAngleDEG);
                var band2 = new Band(sat.Orbit, sensorRight.VerticalHalfAngleDEG, sensorRight.RollAngleDEG);

                var left = BuildStrips(sat, band1);
                var right = BuildStrips(sat, band2);

                leftStrips.Add(satellite.Name, left);
                rightStrips.Add(satellite.Name, right);
            }

            return (leftStrips, rightStrips);
        }

        private static Dictionary<int, List<List<Point>>> BuildStrips(PRDCTSatellite satellite, Band band)
        {
            var strips = new Dictionary<int, List<List<Point>>>();

            foreach (var node in satellite.Nodes().Select(s => s.Value))
            {
                var near = band.GetNearGroundTrack(satellite, node - 1, ScienceConverters.From180To180).ToList();
                var far = band.GetFarGroundTrack(satellite, node - 1, ScienceConverters.From180To180).ToList();

                BandCore2D engine2D = new BandCore2D(near, far, band.IsCoverPolis);

                var shapes = engine2D.CreateShapes(false, false);

                strips.Add(node, new List<List<Point>>());

                foreach (var item in shapes)
                {
                    strips[node].Add(new List<Point>(item.Select(s => new Point(s.Lon, s.Lat))));
                }
            }

            return strips;
        }
    }
}
