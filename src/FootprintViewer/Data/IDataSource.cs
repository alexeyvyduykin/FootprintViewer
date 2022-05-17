using FootprintViewer.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FootprintViewer.Data
{
    public interface IDataSource<T>
    {
        Task<List<T>> GetValuesAsync(IFilter<T>? filter);
    }
}
