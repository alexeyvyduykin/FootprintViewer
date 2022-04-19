using FootprintViewer.Data.Sources;
using NetTopologySuite.Geometries;
using System.Collections.Generic;

namespace FootprintViewer.Data
{
    public class FootprintPreviewGeometryProvider : BaseProvider<IFootprintPreviewGeometryDataSource>
    {
        public virtual IDictionary<string, Geometry> GetFootprintPreviewGeometries()
        {
            var dict = new Dictionary<string, Geometry>();

            foreach (var source in Sources)
            {
                foreach (var item in source.GetFootprintPreviewGeometries())
                {
                    dict.TryAdd(item.Key, item.Value);
                }
            }

            return dict;
        }
    }
}
