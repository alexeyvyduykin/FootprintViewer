using DynamicData;
using Nito.AsyncEx;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.Data.DataManager;

// TODO: try to ConcurentDictionary
public class DataManager : IDataManager
{
    private readonly AsyncLock _mutex = new();
    private readonly Dictionary<string, IDictionary<ISource, IList<object>>> _cache = new();
    private readonly IDictionary<string, IList<ISource>> _sources = new Dictionary<string, IList<ISource>>();
    private readonly HashSet<string> _dirtyKeys = new();

    public DataManager()
    {
        DataChanged = ReactiveCommand.Create<string[], string[]>(s => s, outputScheduler: RxApp.MainThreadScheduler);
    }

    public DataManager(IDictionary<string, IList<ISource>> sources) : this()
    {
        var keys = sources.Keys;

        foreach (var key in keys)
        {
            var list = sources[key].ToList();

            _sources.Add(key, list);

            _cache.Add(key, new Dictionary<ISource, IList<object>>());
        }
    }

    public IObservable<string[]> DataChanged { get; }

    public void RegisterSource(string key, ISource source)
    {
        _dirtyKeys.Add(key);

        if (_sources.ContainsKey(key) == true)
        {
            _sources[key].Add(source);
        }

        if (_sources.ContainsKey(key) == false)
        {
            _sources.Add(key, new List<ISource>() { source });
        }

        if (_cache.ContainsKey(key) == false)
        {
            _cache.Add(key, new Dictionary<ISource, IList<object>>());
        }
    }

    public void UnregisterSource(string key, ISource source)
    {
        if (_sources.ContainsKey(key) == true)
        {
            _dirtyKeys.Add(key);

            _sources[key].Remove(source);

            // clear cache
            var isRemove = _cache[key].Remove(source);

            if (isRemove == false)
            {
                throw new Exception();
            }

            if (_sources[key].Count == 0)
            {
                _sources.Remove(key);

                _cache[key].Clear();

                _cache.Remove(key);
            }
        }
    }

    public void UpdateData()
    {
        ((ReactiveCommand<string[], string[]>)DataChanged)
            .Execute(_dirtyKeys.ToArray())
            .Subscribe();

        _dirtyKeys.Clear();
    }

    public void ForceUpdateData(string key)
    {
        ((ReactiveCommand<string[], string[]>)DataChanged)
            .Execute(new[] { key })
            .Subscribe();
    }

    public IReadOnlyList<ISource> GetSources(string key)
    {
        if (_sources.TryGetValue(key, out var sources) == true)
        {
            return sources.ToImmutableList();
        }

        return new List<ISource>().ToImmutableList();
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
        return await Task.Run(async () =>
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
                    if (_cache[key].ContainsKey(source) == false)
                    {
                        using (await _mutex.LockAsync())
                        {
                            if (_cache[key].ContainsKey(source) == false)
                            {
                                var list = await source.GetValuesAsync();
                                _cache[key].Add(source, list);
                            }
                        }
                    }
                }

                return _cache[key].Values.SelectMany(s => s).Cast<T>().ToList();
            }

            return new List<T>();
        });
    }

    // TODO: not safety
    public async Task<bool> TryAddAsync(string key, object value)
    {
        return await Task.Run(async () =>
        {
            if (_sources.ContainsKey(key) == true)
            {
                if (_sources.TryGetValue(key, out var list) == true)
                {
                    foreach (var item in list)
                    {
                        if (item is IEditableSource editableSource)
                        {
                            await editableSource.AddAsync(key, value);

                            if (_cache.ContainsKey(key) == true)
                            {
                                if (_cache[key].ContainsKey(item) == true)
                                {
                                    _cache[key][item].Add(value);
                                }
                            }

                            return true;
                        }
                    }
                }
            }

            return false;
        });
    }

    // TODO: not safety
    public async Task<bool> TryRemoveAsync(string key, object value)
    {
        return await Task.Run(async () =>
        {
            if (_sources.ContainsKey(key) == true)
            {
                if (_sources.TryGetValue(key, out var list) == true)
                {
                    foreach (var item in list)
                    {
                        if (item is IEditableSource editableSource)
                        {
                            await editableSource.RemoveAsync(key, value);

                            if (_cache.ContainsKey(key) == true)
                            {
                                if (_cache[key].ContainsKey(item) == true)
                                {
                                    _cache[key][item].Remove(value);
                                }
                            }

                            return true;
                        }
                    }
                }
            }

            return false;
        });
    }

    // TODO: not safety
    public async Task<bool> TryEditAsync(string key, string id, object newValue)
    {
        return await Task.Run(async () =>
        {
            if (_sources.ContainsKey(key) == true)
            {
                if (_sources.TryGetValue(key, out var list) == true)
                {
                    foreach (var item in list)
                    {
                        if (item is IEditableSource editableSource)
                        {
                            await editableSource.EditAsync(key, id, newValue);

                            if (_cache.ContainsKey(key) == true)
                            {
                                if (_cache[key].ContainsKey(item) == true)
                                {
                                    //  _cache[key][item].Replace(oldValue, newValue);
                                    // clear cache
                                    _cache[key].Remove(item);//.Replace(oldValue, newValue);
                                }
                            }

                            return true;
                        }
                    }
                }
            }

            return false;
        });
    }
}
