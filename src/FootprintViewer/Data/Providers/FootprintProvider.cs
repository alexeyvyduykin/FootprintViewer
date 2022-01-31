using DynamicData;
using FootprintViewer.Data.Sources;
using System;
using System.Collections.Generic;
using System.Text;

namespace FootprintViewer.Data
{
    public class FootprintProvider
    {
        private readonly List<IFootprintDataSource> _sources;

        public FootprintProvider()
        {
            _sources = new List<IFootprintDataSource>();
        }

        public void AddSource(IFootprintDataSource source)
        {
            _sources.Add(source);
        }

        public IEnumerable<Footprint> GetFootprints()
        {
            var list = new List<Footprint>();

            foreach (var source in Sources)
            {
                list.AddRange(source.GetFootprints());
            }

            return list;
        }

        public IEnumerable<IFootprintDataSource> Sources => _sources;
    }
}
