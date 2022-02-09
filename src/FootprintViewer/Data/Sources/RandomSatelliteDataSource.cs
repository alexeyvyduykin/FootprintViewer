using System.Collections.Generic;

namespace FootprintViewer.Data.Sources
{
    public class RandomSatelliteDataSource : ISatelliteDataSource
    {
        private IEnumerable<Satellite>? _satellites;

        public IEnumerable<Satellite> GetSatellites() =>
            _satellites ??= SatelliteBuilder.Create();
    }
}
