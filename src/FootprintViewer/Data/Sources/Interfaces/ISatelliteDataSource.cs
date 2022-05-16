using FootprintViewer.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FootprintViewer.Data.Sources
{
    public interface ISatelliteDataSource
    {
        Task<List<SatelliteInfo>> GetSatelliteInfosAsync();
    }
}
