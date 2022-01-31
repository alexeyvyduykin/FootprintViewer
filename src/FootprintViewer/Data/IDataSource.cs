using NetTopologySuite.Geometries;
using System.Collections.Generic;

namespace FootprintViewer.Data
{
    public interface IDataSource
    {
        IEnumerable<Satellite> Satellites { get; } 
        IDictionary<string, Dictionary<int, List<List<Point>>>> LeftStrips { get; }
        IDictionary<string, Dictionary<int, List<List<Point>>>> RightStrips { get; }
        IDictionary<string, Dictionary<int, List<List<(double lon, double lat)>>>> GroundTracks { get; }
    }
}
