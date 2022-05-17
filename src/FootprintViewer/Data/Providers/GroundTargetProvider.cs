using FootprintViewer.Data.Sources;
using FootprintViewer.ViewModels;
using ReactiveUI;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;

namespace FootprintViewer.Data
{
    public class GroundTargetProvider : BaseProvider<IGroundTargetDataSource>, IProvider<GroundTargetInfo>
    {
        public GroundTargetProvider()
        {
            Loading = ReactiveCommand.CreateFromTask(_ => GetValuesAsync(null));
        }

        public ReactiveCommand<Unit, List<GroundTargetInfo>> Loading { get; }

        public async Task<List<GroundTargetInfo>> GetValuesAsync(IFilter<GroundTargetInfo>? filter)
        {
            return await Sources.First().GetGroundTargetInfosAsync(filter);
        }
    }
}
