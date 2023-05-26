using FootprintViewer.Data;
using FootprintViewer.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FootprintViewer.Services;

public interface ILocalStorageService
{
    IObservable<string[]> DataChanged { get; }

    IObservable<IList<PlannedScheduleResult>> PlannedScheduleObservable { get; }

    void RegisterSource(string key, ISource source, bool dirty = true);

    void RegisterSources(IDictionary<string, IList<ISource>> sources, bool dirty = true);

    void UnregisterSource(string key, ISource source);

    void UnregisterSources(string key);

    void UpdateData();

    Task<IList<T>> GetValuesAsync<T>(string key);

    IReadOnlyList<ISource> GetSources(string key);

    Task TryAddAsync(string key, object value);

    Task TryEditAsync(string key, string id, object newValue);

    Task TryRemoveAsync(string key, object value);
}
