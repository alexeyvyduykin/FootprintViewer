using NetTopologySuite.Geometries;
using System.Collections.Generic;

namespace FootprintViewer.Data.Sources
{
    public interface IFootprintPreviewGeometryDataSource
    {
        IDictionary<string, Geometry> GetFootprintPreviewGeometries();
    }
}
