using FootprintViewer.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FootprintViewer.Data.Sources
{
    public interface IGroundStationDataSource
    {
        Task<List<GroundStationInfo>> GetGroundStationInfosAsync();
    }
}
