using NetTopologySuite.Geometries;
using System.Collections.Generic;

namespace FootprintViewer.Data.Sources
{
    public interface ISatelliteDataSource
    {
        IEnumerable<Satellite> GetSatellites();

        IDictionary<string, Dictionary<int, List<List<Point>>>> GetLeftStrips();

        IDictionary<string, Dictionary<int, List<List<Point>>>> GetRightStrips();

        IDictionary<string, Dictionary<int, List<List<(double lon, double lat)>>>> GetGroundTracks();
    }
}
