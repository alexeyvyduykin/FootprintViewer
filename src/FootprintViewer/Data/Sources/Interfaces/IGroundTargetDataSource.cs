using System.Collections.Generic;
using System.Threading.Tasks;

namespace FootprintViewer.Data.Sources
{
    public interface IGroundTargetDataSource
    {
        Task<List<GroundTarget>> GetGroundTargetsAsync();
    }
}
