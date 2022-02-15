using FootprintViewer.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FootprintViewer.Data.Sources
{
    public interface IGroundTargetDataSource
    {
        Task<List<GroundTarget>> GetGroundTargetsAsync();

        Task<List<GroundTarget>> GetGroundTargetsAsync(string[] names);

        Task<List<GroundTargetInfo>> GetGroundTargetInfosAsync(string[] names);

        Task<List<GroundTargetInfo>> GetGroundTargetInfosExAsync(Func<GroundTarget, bool> func);
    }
}
