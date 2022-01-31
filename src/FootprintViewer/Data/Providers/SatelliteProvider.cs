using FootprintViewer.Data.Sources;
using NetTopologySuite.Geometries;
using System.Collections.Generic;

namespace FootprintViewer.Data
{
    public class SatelliteProvider : BaseProvider<ISatelliteDataSource>
    {
        public IEnumerable<Satellite> GetSatellites()
        {
            var list = new List<Satellite>();

            foreach (var source in Sources)
            {
                list.AddRange(source.GetSatellites());
            }

            return list;
        }

        public IDictionary<string, Dictionary<int, List<List<Point>>>> GetLeftStrips()
        {
            var dict = new Dictionary<string, Dictionary<int, List<List<Point>>>>();

            foreach (var source in Sources)
            {
                foreach (var item in source.GetLeftStrips())
                {
                    dict.TryAdd(item.Key, item.Value);
                }
            }

            return dict;
        }

        public IDictionary<string, Dictionary<int, List<List<Point>>>> GetRightStrips()
        {
            var dict = new Dictionary<string, Dictionary<int, List<List<Point>>>>();

            foreach (var source in Sources)
            {
                foreach (var item in source.GetRightStrips())
                {
                    dict.TryAdd(item.Key, item.Value);
                }
            }

            return dict;
        }

        public IDictionary<string, Dictionary<int, List<List<(double lon, double lat)>>>> GetGroundTracks()
        {
            var dict = new Dictionary<string, Dictionary<int, List<List<(double lon, double lat)>>>>();

            foreach (var source in Sources)
            {
                foreach (var item in source.GetGroundTracks())
                {
                    dict.TryAdd(item.Key, item.Value);
                }
            }

            return dict;
        }
    }
}
