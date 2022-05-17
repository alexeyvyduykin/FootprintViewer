using FootprintViewer.ViewModels;
using ReactiveUI;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;

namespace FootprintViewer.Data
{
    public class FootprintProvider : BaseProvider<IDataSource<FootprintInfo>>, IProvider<FootprintInfo>
    {
        public FootprintProvider()
        {
            Loading = ReactiveCommand.CreateFromTask(_ => GetValuesAsync(null));
        }

        public ReactiveCommand<Unit, List<FootprintInfo>> Loading { get; }

        public async Task<List<FootprintInfo>> GetValuesAsync(IFilter<FootprintInfo>? filter = null)
        {
            return await Sources.First().GetValuesAsync(filter);
        }
    }
}
