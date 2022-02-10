using System.Collections.Generic;
using System.Threading.Tasks;

namespace FootprintViewer.Data.Sources
{
    public class RandomSatelliteDataSource : ISatelliteDataSource
    {
        private List<Satellite>? _satellites;

        public async Task<List<Satellite>> GetSatellitesAsync()
        {
            return await Task.Run(() =>
            {
                return _satellites ??= new List<Satellite>(SatelliteBuilder.Create());                
            });
        }
    }
}
