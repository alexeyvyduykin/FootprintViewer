using FootprintViewer.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.Data.Sources
{
    public class RandomSatelliteDataSource : ISatelliteDataSource
    {
        private List<SatelliteInfo>? _satellites;

        public async Task<List<SatelliteInfo>> GetSatelliteInfosAsync()
        {
            return await Task.Run(() =>
            {
                return _satellites ??= new List<SatelliteInfo>(SatelliteBuilder.Create().Select(s => new SatelliteInfo(s)));
            });
        }
    }
}
