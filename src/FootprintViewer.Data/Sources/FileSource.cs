using FootprintViewer.Data.DbContexts;

namespace FootprintViewer.Data.Sources;

public class FileSource : BaseSource
{
    private readonly string _key;
    private readonly IList<string> _paths;

    public FileSource(string key, IList<string> paths)
    {
        _key = key;
        _paths = paths;
    }

    public string Key => _key;

    public IList<string> Paths => _paths;

    public override async Task<IList<object>> GetValuesAsync()
    {
        return await Task.Run(() => DbHelper.Loader(_key).Invoke(_paths) ?? new List<object>());
    }
}
