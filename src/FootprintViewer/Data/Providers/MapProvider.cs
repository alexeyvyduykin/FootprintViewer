using FootprintViewer.Data.Sources;
using System;
using System.Collections.Generic;
using System.Text;

namespace FootprintViewer.Data
{
    public class MapProvider
    {
        private readonly List<IMapDataSource> _sources;

        public MapProvider()
        {
            _sources = new List<IMapDataSource>();
        }

        public void AddSource(IMapDataSource source)
        {
            _sources.Add(source);
        }

        public IEnumerable<MapResource> GetMapResources()
        {
            var list = new List<MapResource>();

            foreach (var source in Sources)
            {
                list.AddRange(source.GetMapResources());
            }

            return list;
        }

        public IEnumerable<IMapDataSource> Sources => _sources;
    }
}
