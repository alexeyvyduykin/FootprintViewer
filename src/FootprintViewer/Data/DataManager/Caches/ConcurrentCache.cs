using DynamicData;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Linq;

namespace FootprintViewer.Data.DataManager.Caches;

public class ConcurrentCache<TKey, TSubKey>
    where TKey : class
    where TSubKey : class
{
    public readonly ConcurrentDictionary<TKey, IDictionary<TSubKey, IList<object>>> _cache = new();

    private void AddKeys(TKey key, IList<TSubKey> subKeys)
    {
        if (ContainsKey(key) == false)
        {
            _cache.TryAdd(key, subKeys.ToDictionary<TSubKey, TSubKey, IList<object>>(s => s, _ => new List<object>()));
        }
        else
        {
            _cache.TryGetValue(key, out var dict);

            if (dict is not null)
            {
                foreach (var subKey in subKeys)
                {
                    if (dict.ContainsKey(subKey) == false)
                    {
                        dict.Add(subKey, new List<object>());
                    }
                }
            }
        }
    }

    public bool ContainsKey(TKey key) => _cache.ContainsKey(key);

    public bool ContainsKeys(TKey key, TSubKey subKey)
    {
        if (ContainsKey(key) == true)
        {
            _cache.TryGetValue(key, out var dict);

            if (dict is not null)
            {
                return dict.ContainsKey(subKey);
            }
        }

        return false;
    }

    public void Clear(TKey key, IList<TSubKey> subKeys)
    {
        if (ContainsKey(key) == true)
        {
            _cache.TryGetValue(key, out var dict);

            if (dict is not null)
            {
                foreach (var subKey in subKeys)
                {
                    if (dict.ContainsKey(subKey) == true)
                    {
                        dict.Remove(subKey);
                    }
                }

                if (dict.Count == 0)
                {
                    _cache.TryRemove(key, out _);
                }
            }
        }
    }

    public void Caching(TKey key, TSubKey subKey, IList<object> values)
    {
        try
        {
            AddKeys(key, new[] { subKey });

            _cache.TryGetValue(key, out var dict);

            if (dict is not null)
            {
                dict[subKey].Clear();
                dict[subKey].AddRange(values);
            }
        }
        catch (Exception)
        {
            throw new Exception($"Not exist {key} or {subKey} keys.");
        }
    }

    public IList<T> GetValues<T>(TKey key)
    {
        _cache.TryGetValue(key, out var dict);

        try
        {
            return dict?.Values.SelectMany(s => s.Select(s => (T)s)).ToList() ?? new();
        }
        catch (InvalidCastException)
        {
            throw;
        }
    }

    public IList<T> GetValues<T>(TKey key, TSubKey subKey)
    {
        _cache.TryGetValue(key, out var dict);

        try
        {
            return dict?[subKey].Select(s => (T)s).ToList() ?? new();
        }
        catch (InvalidCastException)
        {
            throw;
        }
    }

    public IReadOnlyList<TKey> GetKeys()
    {
        return _cache.Keys.ToImmutableList();
    }

    public IReadOnlyList<TSubKey> GetSubKeys()
    {
        var keys = _cache.Keys;

        var list = new List<TSubKey>();

        foreach (var key in keys)
        {
            list.AddRange(_cache[key].Keys);
        }

        return list.ToImmutableList();
    }
}
