using FootprintViewer.Data;
using FootprintViewer.Data.DbContexts;
using FootprintViewer.Data.Models;
using ReactiveUI;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace FootprintViewer.Services;

public class LocalStorageService : ILocalStorageService
{
    private readonly IDataManager _dataManager = new DataManager();
    private readonly HashSet<string> _dirtyKeys = new();
    private readonly Subject<string[]> _changeSubj = new();
    private readonly ReactiveCommand<Unit, IList<PlannedScheduleResult>> _command;

    public LocalStorageService()
    {
        _command = ReactiveCommand.CreateFromTask(GetPlannedSchedulesAsync);

        PlannedScheduleObservable = _command.AsObservable();
    }

    public IObservable<string[]> DataChanged => _changeSubj.AsObservable();

    public IObservable<IList<PlannedScheduleResult>> PlannedScheduleObservable { get; }

    private async Task<IList<PlannedScheduleResult>> GetPlannedSchedulesAsync()
    {
        return await GetValuesAsync<PlannedScheduleResult>(DbKeys.PlannedSchedules.ToString());
    }

    public void RegisterSource(string key, ISource source, bool dirty = true)
    {
        if (dirty == true)
        {
            _dirtyKeys.Add(key);
        }

        _dataManager.RegisterSource(key, source);
    }

    public void RegisterSources(IDictionary<string, IList<ISource>> sources, bool dirty = true)
    {
        foreach (var (key, sourceList) in sources)
        {
            foreach (var source in sourceList)
            {
                RegisterSource(key, source, dirty);
            }
        }
    }

    public void UnregisterSource(string key, ISource source)
    {
        var hasKey = _dataManager.GetKeys().Contains(key);

        if (hasKey == true)
        {
            _dirtyKeys.Add(key);
        }

        _dataManager.UnregisterSource(key, source);
    }

    public void UnregisterSources(string key)
    {
        var hasKey = _dataManager.GetKeys().Contains(key);

        if (hasKey == true)
        {
            _dirtyKeys.Add(key);
        }

        _dataManager.UnregisterSources(key);
    }

    public async Task<IList<T>> GetValuesAsync<T>(string key)
    {
        return await _dataManager.GetDataAsync<T>(key, true);
    }

    public IReadOnlyList<ISource> GetSources(string key)
    {
        return _dataManager.GetSources(key);
    }

    public async Task<bool> TryAddAsync(string key, object value)
    {
        var res = await _dataManager.TryAddAsync(key, value);

        if (res == true)
        {
            ForceUpdateData(key);
        }

        return res;
    }

    public async Task<bool> TryRemoveAsync(string key, object value)
    {
        var res = await _dataManager.TryRemoveAsync(key, value);

        if (res == true)
        {
            ForceUpdateData(key);
        }

        return res;
    }

    public async Task<bool> TryEditAsync(string key, string id, object newValue)
    {
        var res = await _dataManager.TryEditAsync(key, id, newValue);

        if (res == true)
        {
            ForceUpdateData(key);
        }

        return res;
    }

    public async Task<bool> TryAddAsync_Test(string key, object value)
    {
        return await _dataManager.TryAddAsync(key, value);
    }

    public async Task<bool> TryRemoveAsync_Test(string key, object value)
    {
        return await _dataManager.TryRemoveAsync(key, value);
    }

    public async Task<bool> TryEditAsync_Test(string key, string id, object newValue)
    {
        return await _dataManager.TryEditAsync(key, id, newValue);
    }

    public void UpdateData()
    {
        _changeSubj.OnNext(_dirtyKeys.ToArray());

        if (_dirtyKeys.Contains(DbKeys.PlannedSchedules.ToString()))
        {
            _command.Execute(Unit.Default).Subscribe();
        }

        _dirtyKeys.Clear();
    }

    public void InvalidatePlannedSchedule()
    {
        _command.Execute(Unit.Default).Subscribe();
    }
    public void UpdateData_Test_Remove_After()
    {

    }
    private void ForceUpdateData(string key)
    {
        _changeSubj.OnNext(new[] { key });
    }

    public void ForceUpdateData_Test(string key)
    {
        _changeSubj.OnNext(new[] { key });
    }
}