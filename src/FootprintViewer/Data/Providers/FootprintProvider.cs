using FootprintViewer.Data.Sources;
using ReactiveUI;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;

namespace FootprintViewer.Data
{
    public class FootprintProvider : BaseProvider<IFootprintDataSource>
    {
        public FootprintProvider()
        {      
            Loading = ReactiveCommand.CreateFromTask(GetFootprintsAsync);
        }

        public ReactiveCommand<Unit, List<Footprint>> Loading { get; }

        public IEnumerable<Footprint> GetFootprints()
        {
            var list = new List<Footprint>();

            foreach (var source in Sources)
            {
                list.AddRange(source.GetFootprintsAsync().Result);
            }

            return list;
        }

        public async Task<List<Footprint>> GetFootprintsAsync()
        {
            return await Sources.First().GetFootprintsAsync();

            //List<Footprint> list = new List<Footprint>();

            //foreach (var source in Sources)
            //{
            //    list.AddRange(await source.GetFootprintsAsync());
            //}

            //return list;
        }
    }
}
