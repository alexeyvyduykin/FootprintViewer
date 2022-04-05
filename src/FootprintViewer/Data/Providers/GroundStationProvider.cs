using FootprintViewer.Data.Sources;
using ReactiveUI;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;

namespace FootprintViewer.Data
{
    public class GroundStationProvider : BaseProvider<IGroundStationDataSource>
    {
        public GroundStationProvider()
        {
            Loading = ReactiveCommand.CreateFromTask(GetGroundStationsAsync);
        }

        public ReactiveCommand<Unit, List<GroundStation>> Loading { get; }

        public IEnumerable<GroundStation> GetGroundStations()
        {
            var list = new List<GroundStation>();

            foreach (var source in Sources)
            {
                list.AddRange(source.GetGroundStationsAsync().Result);
            }

            return list;
        }

        public async Task<List<GroundStation>> GetGroundStationsAsync()
        {
            return await Sources.First().GetGroundStationsAsync();
        }
    }
}
