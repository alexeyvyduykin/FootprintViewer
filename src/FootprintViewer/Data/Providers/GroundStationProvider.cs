using FootprintViewer.Data.Sources;
using FootprintViewer.ViewModels;
using ReactiveUI;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;

namespace FootprintViewer.Data
{
    public class GroundStationProvider : BaseProvider<IGroundStationDataSource>, IProvider<GroundStationInfo>
    {
        public GroundStationProvider()
        {
            Loading = ReactiveCommand.CreateFromTask(s => GetValuesAsync(null));
        }

        public ReactiveCommand<Unit, List<GroundStationInfo>> Loading { get; }

        public async Task<List<GroundStationInfo>> GetValuesAsync(IFilter<GroundStationInfo>? filter = null)
        {
            return await Sources.First().GetGroundStationInfosAsync();
        }
    }
}
