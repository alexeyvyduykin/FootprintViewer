using System.Collections.Generic;

namespace FootprintViewer.Data.Sources
{
    public interface IMapDataSource
    {
        IEnumerable<MapResource> GetMapResources();
    }
}
