using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace DataSettingsSample.Data
{
    public class JsonSource : BaseSource
    {
        private readonly string? _path;
        private readonly IList<string>? _paths;
        private readonly Uri? _uri;
        private readonly DbKeys _key;

        public JsonSource(DbKeys key, string path)
        {
            _key = key;
            _path = path;
        }

        public JsonSource(DbKeys key, Uri uri)
        {
            _key = key;
            _uri = uri;
        }

        public JsonSource(DbKeys key, IList<string> paths)
        {
            _key = key;
            _paths = paths;
        }

        public override async Task<IList<object>> GetValuesAsync()
        {
            if (_paths != null)
            {
                return await Task.Run(() => DbHelper.JsonReaderFromPaths(_key).Invoke(_paths));
            }
            else if (string.IsNullOrEmpty(_path) == false)
            {
                return await Task.Run(() => DbHelper.JsonReaderFromPath(_key).Invoke(_path));
            }
            else if (_uri != null)
            {
                return await Task.Run(() => DbHelper.JsonReaderFromUri(_key).Invoke(_uri));
            }

            return new List<object>();
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
