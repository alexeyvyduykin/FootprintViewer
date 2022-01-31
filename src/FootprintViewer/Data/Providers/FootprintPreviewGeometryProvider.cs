using FootprintViewer.Data.Sources;
using Mapsui.Geometries;
using System.Collections.Generic;

namespace FootprintViewer.Data
{
    public class FootprintPreviewGeometryProvider : BaseProvider<IFootprintPreviewGeometryDataSource>
    {     
        public virtual IDictionary<string, IGeometry> GetFootprintPreviewGeometries()
        {
            var dict = new Dictionary<string, IGeometry>();

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
