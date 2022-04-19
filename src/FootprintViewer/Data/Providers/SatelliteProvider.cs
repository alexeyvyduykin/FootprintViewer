using FootprintViewer.Data.Sources;
using ReactiveUI;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;

namespace FootprintViewer.Data
{
    public class SatelliteProvider : BaseProvider<ISatelliteDataSource>
    {
        public SatelliteProvider()
        {
            Loading = ReactiveCommand.CreateFromTask(GetSatellitesAsync);
        }

        public ReactiveCommand<Unit, List<Satellite>> Loading { get; }

        public async Task<List<Satellite>> GetSatellitesAsync()
        {
            return await Sources.First().GetSatellitesAsync();

            //var list = new List<Satellite>();

            //foreach (var source in Sources)
            //{              
            //    list.AddRange(await source.GetSatellitesAsync());
            //}

            //return list;
        }
    }
}
