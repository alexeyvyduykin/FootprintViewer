using NetTopologySuite.Geometries;
using System.Collections.Generic;

namespace DatabaseCreatorSample.Data
{
    public interface IDataSource
    {
        IEnumerable<Satellite> Satellites { get; }
        IEnumerable<Footprint> Footprints { get; }
        IDictionary<string, Dictionary<int, List<List<Point>>>> LeftStrips { get; }
        IDictionary<string, Dictionary<int, List<List<Point>>>> RightStrips { get; }
        IEnumerable<GroundTarget> Targets { get; }
        IDictionary<string, Dictionary<int, List<List<(double lon, double lat)>>>> GroundTracks { get; }
    }
}
