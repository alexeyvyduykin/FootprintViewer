using NetTopologySuite.Geometries;
using System.Collections.Generic;

namespace FootprintViewer.Data.Sources
{
    public interface ISatelliteDataSource
    {
        IEnumerable<Satellite> GetSatellites();
    }
}
