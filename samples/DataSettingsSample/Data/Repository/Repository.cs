using DynamicData;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DataSettingsSample.Data
{
    public class Repository
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

        public async Task<IList<T>> GetDataAsync<T>(string key)
        {
            return await Task.Run(async () =>
            {
                if (_sources.ContainsKey(key) == true)
                {
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

        public static List<T> DeserializeFromStream<T>(string path)
        {
            var jsonString = File.ReadAllText(path);

            try
            {
                return JsonConvert.DeserializeObject<List<T>>(jsonString) ?? new List<T>();
            }
            catch (System.Exception)
            {
                var res = JsonConvert.DeserializeObject<T>(jsonString);

                if (res == null)
                {
                    return new List<T>();
                }

                return new List<T>() { res };
            }
        }

        public static List<T> DeserializeFromStream<T>(Stream stream)
        {
            using var file = new StreamReader(stream);

            string jsonString = file.ReadToEnd();

            try
            {
                return JsonConvert.DeserializeObject<List<T>>(jsonString) ?? new List<T>();
            }
            catch (System.Exception)
            {
                var res = JsonConvert.DeserializeObject<T>(jsonString);

                if (res == null)
                {
                    return new List<T>();
                }

                return new List<T>() { res };
            }
        }
    }
}
