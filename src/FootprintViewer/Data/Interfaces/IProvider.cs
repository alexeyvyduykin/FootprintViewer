using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FootprintViewer.Data
{
    public interface IProvider<TNative>
    {
        Task<List<TNative>> GetNativeValuesAsync(IFilter<TNative>? filter);

        Task<List<T>> GetValuesAsync<T>(IFilter<T>? filter, Func<TNative, T> converter);
    }
}
