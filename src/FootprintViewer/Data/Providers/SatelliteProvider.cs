using FootprintViewer.Data.Sources;
using FootprintViewer.ViewModels;
using ReactiveUI;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;

namespace FootprintViewer.Data
{
    public class SatelliteProvider : BaseProvider<ISatelliteDataSource>, IProvider<SatelliteInfo>
    {
        public SatelliteProvider()
        {
            Loading = ReactiveCommand.CreateFromTask(s => GetValuesAsync(null));
        }

        public ReactiveCommand<Unit, List<SatelliteInfo>> Loading { get; }

        public async Task<List<SatelliteInfo>> GetValuesAsync(IFilter<SatelliteInfo>? filter = null)
        {
            return await Sources.First().GetSatelliteInfosAsync();
        }
    }
}
