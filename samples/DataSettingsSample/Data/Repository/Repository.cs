using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DataSettingsSample.Data
{
    public class Repository
    {
        private readonly IDictionary<string, IList<object>> _cache = new Dictionary<string, IList<object>>();
        private readonly IDictionary<string, ISource> _sources = new Dictionary<string, ISource>();

        public void RegisterSource(string key, ISource source)
        {
            if (_sources.ContainsKey(key) == false)
            {
                _sources.Add(key, source);
            }
        }

        public async Task<IList<T>> GetDataAsync<T>(string key)
        {
            if (_sources.ContainsKey(key) == true)
            {
                if (_cache.ContainsKey(key) == false)
                {
                    var list = await _sources[key].GetValuesAsync();

                    _cache.Add(key, list);
                }

                return _cache[key].Cast<T>().ToList();
            }

            return new List<T>();
        }

        public IList<T> GetData<T>(string key)
        {
            if (_sources.ContainsKey(key) == true)
            {
                if (_cache.ContainsKey(key) == false)
                {
                    var list = _sources[key].GetValues();

                    _cache.Add(key, list);
                }

                return _cache[key].Cast<T>().ToList();
            }

            return new List<T>();
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
