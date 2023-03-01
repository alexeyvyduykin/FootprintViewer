using DynamicData;
using System.Collections.Immutable;
using System.Reactive.Linq;

namespace FootprintViewer.Data.Caches;

public class Cache<TKey, TSubKey>
    where TKey : class
    where TSubKey : class
{
    public readonly Dictionary<TKey, IDictionary<TSubKey, IList<object>>> _cache = new();

    private void AddKeys(TKey key, IList<TSubKey> subKeys)
    {
        if (ContainsKey(key) == false)
        {
            _cache.Add(key, subKeys.ToDictionary<TSubKey, TSubKey, IList<object>>(s => s, _ => new List<object>()));
        }
        else
        {
            foreach (var subKey in subKeys)
            {
                if (_cache[key].ContainsKey(subKey) == false)
                {
                    _cache[key].Add(subKey, new List<object>());
                }
            }
        }
    }

    public bool ContainsKey(TKey key) => _cache.ContainsKey(key);

    public bool ContainsKeys(TKey key, TSubKey subKey) => ContainsKey(key) && _cache[key].ContainsKey(subKey);

    public void Clear(TKey key, IList<TSubKey> subKeys)
    {
        if (ContainsKey(key) == true)
        {
            foreach (var subKey in subKeys)
            {
                if (_cache[key].ContainsKey(subKey) == true)
                {
                    _cache[key][subKey].Clear();
                    _cache[key].Remove(subKey);
                }
            }

            if (_cache[key].Count == 0)
            {
                _cache.Remove(key);
            }
        }
    }

    public void Caching(TKey key, TSubKey subKey, IList<object> values)
    {
        try
        {
            AddKeys(key, new[] { subKey });

            _cache[key][subKey].Clear();
            _cache[key][subKey].AddRange(values);
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
        try
        {
            return _cache[key][subKey].Select(s => (T)s).ToList();
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
