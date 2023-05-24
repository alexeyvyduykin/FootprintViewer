using FootprintViewer.Data;
using FootprintViewer.Data.DbContexts;
using FootprintViewer.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FootprintViewer.Services;

public interface ILocalStorageService
{
    void InvalidatePlannedSchedule();

    void UpdateData_Test_Remove_After();

    IObservable<string[]> DataChanged { get; }

    IObservable<IList<PlannedScheduleResult>> PlannedScheduleObservable { get; }

    void RegisterSource(string key, ISource source, bool dirty = true);

    void RegisterSources(IDictionary<string, IList<ISource>> sources, bool dirty = true);

    void UnregisterSource(string key, ISource source);

    void UnregisterSources(string key);

    Task<IList<T>> GetValuesAsync<T>(string key);

    IReadOnlyList<ISource> GetSources(string key);

    Task<bool> TryAddAsync_Test(string key, object value);

    Task<bool> TryRemoveAsync_Test(string key, object value);

    Task<bool> TryEditAsync_Test(string key, string id, object newValue);

    void ForceUpdateData_Test(string key);
}
