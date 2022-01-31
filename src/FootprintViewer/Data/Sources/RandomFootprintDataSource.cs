using System.Collections.Generic;

namespace FootprintViewer.Data.Sources
{
    public class RandomFootprintDataSource : IFootprintDataSource
    {
        private IEnumerable<Footprint>? _footprints;
        private readonly ISatelliteDataSource _source;

        public RandomFootprintDataSource(ISatelliteDataSource source)
        {
            _source = source;
        }

        public IEnumerable<Footprint> GetFootprints() => 
            _footprints ??= FootprintBuilder.Create(_source.GetSatellites());
    }
}
