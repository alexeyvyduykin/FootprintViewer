using DynamicData;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FootprintViewer.Data
{
    public interface IProvider
    {
        SourceList<IDataSource> Sources { get; }
    }

    public interface IProvider<TNative> : IProvider
    {
        Task<List<TNative>> GetNativeValuesAsync(IFilter<TNative>? filter);

        Task<List<T>> GetValuesAsync<T>(IFilter<T>? filter, Func<TNative, T> converter);
    }
}
