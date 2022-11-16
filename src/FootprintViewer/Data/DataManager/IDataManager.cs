using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FootprintViewer.Data.DataManager;

public interface IDataManager
{
    void RegisterSource(string key, ISource source);

    void UnregisterSource(string key, ISource source);

    IReadOnlyList<ISource> GetSources(string key);

    IReadOnlyDictionary<string, IReadOnlyList<ISource>> GetSources();

    Task<IList<T>> GetDataAsync<T>(string key, bool caching = true);

    Task<bool> TryAddAsync(string key, object value);

    Task<bool> TryRemoveAsync(string key, object value);

    Task<bool> TryEditAsync(string key, string id, object newValue);

    IObservable<string[]> DataChanged { get; }

    void UpdateData();

    void ForceUpdateData(string key);
}
