using FootprintViewer.ViewModels;
using Mapsui.Geometries;
using System.Collections.Generic;
using System.Linq;

namespace DatabaseCreatorSample.Data
{
    public class SensorDataSource
    {
        private readonly Satellite _satellite;      
        private readonly Dictionary<int, List<List<Point>>> _leftStrips;
        private readonly Dictionary<int, List<List<Point>>> _rightStrips;

        public SensorDataSource(Satellite satellite)
        {
            _satellite = satellite;

            var sat = satellite.ToPRDCTSatellite();
            var sensorLeft = satellite.ToPRDCTSensor("Left");
            var sensorRight = satellite.ToPRDCTSensor("Right");
    
            var band1 = new Band(sat.Orbit, sensorLeft.VerticalHalfAngleDEG, sensorLeft.RollAngleDEG);
            var band2 = new Band(sat.Orbit, sensorRight.VerticalHalfAngleDEG, sensorRight.RollAngleDEG);

            _leftStrips = Build(sat, band1);
            _rightStrips = Build(sat, band2);
        }

        private static Dictionary<int, List<List<Point>>> Build(PRDCTSatellite satellite, Band band)
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

        public Satellite Satellite => _satellite;

        public Dictionary<int, List<List<Point>>> LeftStrips => _leftStrips; 

        public Dictionary<int, List<List<Point>>> RightStrips => _rightStrips;
    }
}
