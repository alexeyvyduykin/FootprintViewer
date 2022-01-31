using System.Collections.Generic;

namespace FootprintViewer.Data.Sources
{
    public class RandomFootprintDataSource : IFootprintDataSource
    {
        private readonly IEnumerable<Footprint> _footprints;

        public RandomFootprintDataSource(IEnumerable<Satellite> satellites)
        {
            _footprints = FootprintBuilder.Create(satellites);
        }
        
        public IEnumerable<Footprint> GetFootprints() => _footprints;
    }
}
