﻿using DynamicData;
using FootprintViewer.Data.DataManager.Caches;
using Nito.AsyncEx;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.Data.DataManager;

public class DataManager : IDataManager
{
    private readonly AsyncLock _mutex = new();
    private readonly Cache<string, ISource> _sourceCache = new();
    private readonly IDictionary<string, IList<ISource>> _sources = new Dictionary<string, IList<ISource>>();
    private readonly HashSet<string> _dirtyKeys = new();

    public DataManager()
    {
        DataChanged = ReactiveCommand.Create<string[], string[]>(s => s, outputScheduler: RxApp.MainThreadScheduler);
    }

    public DataManager(IDictionary<string, IList<ISource>> dict) : this()
    {
        foreach (var key in dict.Keys)
        {
            var list = dict[key].ToList();

            _sources.Add(key, list);
        }
    }

    public IObservable<string[]> DataChanged { get; }

    public void RegisterSource(string key, ISource source, bool dirty = true)
    {
        if (dirty == true)
        {
            _dirtyKeys.Add(key);
        }

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
            _dirtyKeys.Add(key);

            _sources[key].Remove(source);

            if (_sources[key].Count == 0)
            {
                _sources.Remove(key);
            }
        }

        _sourceCache.Clear(key, new[] { source });
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

                            ForceUpdateData(key);

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

                            ForceUpdateData(key);

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

                            ForceUpdateData(key);

                            return true;
                        }
                    }
                }
            }

            return false;
        }, RxApp.TaskpoolScheduler);
    }
}
