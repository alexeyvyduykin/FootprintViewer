using FootprintViewer.Data.Science;
using System.Collections.Generic;
using System.Linq;

namespace FootprintViewer.Data
{
    internal static class StripBuilder
    {
        public static Dictionary<string, Dictionary<int, List<List<(double lon, double lat)>>>> CreateLeft(IEnumerable<Satellite> satellites)
        {
            var leftStrips = new Dictionary<string, Dictionary<int, List<List<(double, double)>>>>();

            foreach (var satellite in satellites)
            {
                var sat = satellite.ToPRDCTSatellite();

                var sensorLeft = satellite.ToPRDCTSensor(SatelliteStripDirection.Left);

                var band = new Band(sat.Orbit, sensorLeft.VerticalHalfAngleDEG, sensorLeft.RollAngleDEG);

                var left = BuildStrips(sat, band);

                leftStrips.Add(satellite.Name!, left);
            }

            return leftStrips;
        }

        public static Dictionary<string, Dictionary<int, List<List<(double lon, double lat)>>>> CreateRight(IEnumerable<Satellite> satellites)
        {
            var rightStrips = new Dictionary<string, Dictionary<int, List<List<(double, double)>>>>();

            foreach (var satellite in satellites)
            {
                var sat = satellite.ToPRDCTSatellite();

                var sensorRight = satellite.ToPRDCTSensor(SatelliteStripDirection.Right);

                var band = new Band(sat.Orbit, sensorRight.VerticalHalfAngleDEG, sensorRight.RollAngleDEG);

                var right = BuildStrips(sat, band);

                rightStrips.Add(satellite.Name!, right);
            }

            return rightStrips;
        }

        private static Dictionary<int, List<List<(double lon, double lat)>>> BuildStrips(PRDCTSatellite satellite, Band band)
        {
            var strips = new Dictionary<int, List<List<(double, double)>>>();

            foreach (var node in satellite.Nodes().Select(s => s.Value))
            {
                var near = band.GetNearGroundTrack(satellite, node - 1, ScienceConverters.From180To180).ToList();
                var far = band.GetFarGroundTrack(satellite, node - 1, ScienceConverters.From180To180).ToList();

                var engine2D = new BandCore2D(near, far, band.IsCoverPolis);

                var shapes = engine2D.CreateShapes(false, false);

                strips.Add(node, new List<List<(double, double)>>());

                foreach (var item in shapes)
                {
                    strips[node].Add(new List<(double, double)>(item.Select(s => (s.Lon, s.Lat))));
                }
            }

            return strips;
        }
    }
}
