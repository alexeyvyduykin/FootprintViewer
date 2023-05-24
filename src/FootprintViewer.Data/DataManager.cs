using DynamicData;
using FootprintViewer.Data.Caches;
using Nito.AsyncEx;
using ReactiveUI;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Reactive.Linq;

namespace FootprintViewer.Data;

public class DataManager : IDataManager
{
    private readonly AsyncLock _mutex = new();
    private readonly Cache<string, ISource> _sourceCache = new();
    private readonly IDictionary<string, IList<ISource>> _sources = new Dictionary<string, IList<ISource>>();

    public DataManager()
    {

    }

    public DataManager(IDictionary<string, IList<ISource>> dict) : this()
    {
        foreach (var key in dict.Keys)
        {
            var list = dict[key].ToList();

            _sources.Add(key, list);
        }
    }

    public void RegisterSource(string key, ISource source)
    {
        if (_sources.ContainsKey(key) == true)
        {
            _sources[key].Add(source);
        }

        if (_sources.ContainsKey(key) == false)
        {
            _sources.Add(key, new List<ISource>() { source });
        }
    }

    public void UnregisterSource(string key, ISource source)
    {
        if (_sources.ContainsKey(key) == true)
        {
            _sources[key].Remove(source);

            if (_sources[key].Count == 0)
            {
                _sources.Remove(key);
            }
        }

        _sourceCache.Clear(key, new[] { source });
    }

    public void UnregisterSources(string key)
    {
        if (_sources.ContainsKey(key) == true)
        {
            _sources[key].Clear();

            _sources.Remove(key);
        }

        _sourceCache.Clear(key);
    }

    public IReadOnlyList<ISource> GetSources(string key)
    {
        if (_sources.TryGetValue(key, out var sources) == true)
        {
            return sources.ToImmutableList();
        }

        return new List<ISource>().ToImmutableList();
    }

    public IReadOnlyList<string> GetKeys()
    {
        return _sources.Keys.ToImmutableList();
    }

    public IReadOnlyDictionary<string, IReadOnlyList<ISource>> GetSources()
    {
        var keys = _sources.Keys.ToList();

        var dict = new Dictionary<string, IReadOnlyList<ISource>>();

        foreach (var key in keys)
        {
            if (_sources.TryGetValue(key, out var sources) == true)
            {
                dict.Add(key, sources.ToImmutableList());
            }
        }

        return new ReadOnlyDictionary<string, IReadOnlyList<ISource>>(dict);
    }

    public async Task<IList<T>> GetDataAsync<T>(string key, bool caching = true)
    {
        return await Observable.StartAsync(async () =>
        {
            if (_sources.ContainsKey(key) == true)
            {
                if (caching == false)
                {
                    var list = new List<object>();

                    foreach (var source in _sources[key])
                    {
                        var values = await source.GetValuesAsync();
                        list.Add(values);
                    }

                    return list.Cast<T>().ToList();
                }

                foreach (var source in _sources[key])
                {
                    if (_sourceCache.ContainsKeys(key, source) == false)
                    {
                        using (await _mutex.LockAsync())
                        {
                            if (_sourceCache.ContainsKeys(key, source) == false)
                            {
                                var list = await source.GetValuesAsync();
                                _sourceCache.Caching(key, source, list);
                            }
                        }
                    }
                }
                return _sourceCache.GetValues<T>(key);
            }

            return new List<T>();
        }, RxApp.TaskpoolScheduler);
    }

    public async Task<bool> TryAddAsync(string key, object value)
    {
        return await Observable.StartAsync(async () =>
        {
            if (_sources.ContainsKey(key) == true)
            {
                if (_sources.TryGetValue(key, out var list) == true)
                {
                    foreach (var item in list)
                    {
                        if (item is IEditableSource editableSource)
                        {
                            using (await _mutex.LockAsync())
                            {
                                _sourceCache.Clear(key, new[] { editableSource });
                            }

                            await editableSource.AddAsync(key, value);

                            return true;
                        }
                    }
                }
            }

            return false;
        }, RxApp.TaskpoolScheduler);
    }

    public async Task<bool> TryRemoveAsync(string key, object value)
    {
        return await Observable.StartAsync(async () =>
        {
            if (_sources.ContainsKey(key) == true)
            {
                if (_sources.TryGetValue(key, out var list) == true)
                {
                    foreach (var item in list)
                    {
                        if (item is IEditableSource editableSource)
                        {
                            using (await _mutex.LockAsync())
                            {
                                _sourceCache.Clear(key, new[] { editableSource });
                            }

                            await editableSource.RemoveAsync(key, value);

                            return true;
                        }
                    }
                }
            }

            return false;
        }, RxApp.TaskpoolScheduler);
    }

    public async Task<bool> TryEditAsync(string key, string id, object newValue)
    {
        return await Observable.StartAsync(async () =>
        {
            if (_sources.ContainsKey(key) == true)
            {
                if (_sources.TryGetValue(key, out var list) == true)
                {
                    foreach (var item in list)
                    {
                        if (item is IEditableSource editableSource)
                        {
                            using (await _mutex.LockAsync())
                            {
                                _sourceCache.Clear(key, new[] { editableSource });
                            }

                            await editableSource.EditAsync(key, id, newValue);

                            return true;
                        }
                    }
                }
            }

            return false;
        }, RxApp.TaskpoolScheduler);
    }
}
