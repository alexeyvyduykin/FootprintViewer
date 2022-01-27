using FootprintViewer.Data.Sources;
using Mapsui.Geometries;
using System.Collections.Generic;

namespace FootprintViewer.Data
{
    public class FootprintPreviewGeometryProvider
    {
        private readonly List<IFootprintPreviewGeometryDataSource> _sources;

        public FootprintPreviewGeometryProvider()
        {
            _sources = new List<IFootprintPreviewGeometryDataSource>();
        }

        public void AddSource(IFootprintPreviewGeometryDataSource source)
        {
            _sources.Add(source);
        }

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

        public IEnumerable<IFootprintPreviewGeometryDataSource> Sources => _sources;
    }
}
