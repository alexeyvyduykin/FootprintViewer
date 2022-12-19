using System.Collections.Generic;
using System.Threading.Tasks;

namespace FootprintViewer.Data.DataManager.Sources;

public class JsonSource : BaseSource
{
    private readonly IList<string> _paths;
    private readonly string _key;

    public JsonSource(string key, string path)
    {
        _key = key;
        _paths = new List<string>() { path };
    }

    public JsonSource(string key, IList<string> paths)
    {
        _key = key;
        _paths = paths;
    }

    public string Key => _key;

    public IList<string> Paths => _paths;

    public override async Task<IList<object>> GetValuesAsync()
    {
        return await Task.Run(() => DbHelper.JsonReaderFromPaths(_key).Invoke(_paths));
    }
}
