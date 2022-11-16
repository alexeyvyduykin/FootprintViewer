using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace FootprintViewer.Data.DataManager.Sources;

public class JsonSource : BaseSource
{
    private readonly string? _path;
    private readonly IList<string>? _paths;
    private readonly string _key;

    public JsonSource(string key, string path)
    {
        _key = key;
        _path = path;
    }

    public JsonSource(string key, IList<string> paths)
    {
        _key = key;
        _paths = paths;
    }

    public string Key => _key;

    public IList<string> Paths => _paths ?? ( (_path != null) ? new List<string>() { _path } : new List<string>());

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
