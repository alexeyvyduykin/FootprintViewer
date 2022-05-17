using FootprintViewer.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FootprintViewer.Data.Sources
{
    public interface IGroundTargetDataSource
    {
        Task<List<GroundTargetInfo>> GetGroundTargetInfosAsync(IFilter<GroundTargetInfo>? filter);
    }
}
