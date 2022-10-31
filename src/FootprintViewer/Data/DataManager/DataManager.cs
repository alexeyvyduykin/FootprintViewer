using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.Data.DataManager;

// TODO: try to ConcurentDictionary
public class DataManager : IDataManager
{
    private readonly Dictionary<string, IDictionary<ISource, IList<object>>> _cache = new();
    private readonly IDictionary<string, IList<ISource>> _sources = new Dictionary<string, IList<ISource>>();

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

        if (_cache.ContainsKey(key) == false)
        {
            _cache.Add(key, new Dictionary<ISource, IList<object>>());
        }
    }

    public void UnregisterSource(string key, ISource source)
    {
        if (_sources.ContainsKey(key) == true)
        {
            _sources[key].Remove(source);

            // clear cache
            _cache[key].Remove(source);

            if (_sources[key].Count == 0)
            {
                _sources.Remove(key);

                _cache[key].Clear();

                _cache.Remove(key);
            }
        }
    }

    public IReadOnlyList<ISource> GetSources(string key)
    {
        if (_sources.TryGetValue(key, out var sources) == true)
        {
            return sources.ToImmutableList();
        }

        return new List<ISource>().ToImmutableList();
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
                        var list = await source.GetValuesAsync();
                        _cache[key].Add(source, list);
                    }
                }

                return _cache[key].Values.SelectMany(s => s).Cast<T>().ToList();
            }

            return new List<T>();
        });
    }
}
