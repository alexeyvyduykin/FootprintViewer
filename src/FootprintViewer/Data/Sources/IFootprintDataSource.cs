using System.Collections.Generic;

namespace FootprintViewer.Data.Sources
{
    public interface IFootprintDataSource
    {
        IEnumerable<Footprint> GetFootprints();
    }
}
