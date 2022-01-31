using System.Collections.Generic;

namespace FootprintViewer.Data.Sources
{
    public class RandomFootprintDataSource : IFootprintDataSource
    {
        private readonly IEnumerable<Footprint> _footprints;

        public RandomFootprintDataSource(ISatelliteDataSource source)
        {
            _footprints = FootprintBuilder.Create(source.GetSatellites());
        }
        
        public IEnumerable<Footprint> GetFootprints() => _footprints;
    }
}
