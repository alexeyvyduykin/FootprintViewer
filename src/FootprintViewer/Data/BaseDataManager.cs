using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FootprintViewer.Data
{
    //public abstract class BaseDataManager<TNative, TSource> : IDataManager<TNative> where TSource : IDataSource
    //{
    //    Task<List<TNative>> IDataManager<TNative>.GetNativeValuesAsync(IDataSource dataSource, IFilter<TNative>? filter)
    //    {
    //        return GetNativeValuesAsync((TSource)dataSource, filter);
    //    }

    //    Task<List<T>> IDataManager<TNative>.GetValuesAsync<T>(IDataSource dataSource, IFilter<T>? filter, Func<TNative, T> converter)
    //    {
    //        return GetValuesAsync((TSource)dataSource, filter, converter);
    //    }

    //    protected abstract Task<List<TNative>> GetNativeValuesAsync(TSource dataSource, IFilter<TNative>? filter);

    //    protected abstract Task<List<T>> GetValuesAsync<T>(TSource dataSource, IFilter<T>? filter, Func<TNative, T> converter);
    //}
}
