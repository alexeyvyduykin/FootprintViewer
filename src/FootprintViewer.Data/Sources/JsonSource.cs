namespace FootprintViewer.Data.Sources;

public class JsonSource : BaseSource
{
    private readonly IList<string> _paths;
    private readonly Func<string, object> _reader;

    public JsonSource(string path, Func<string, object> reader)
    {
        _paths = new List<string>() { path };
        _reader = reader;
    }

    public JsonSource(IList<string> paths, Func<string, object> reader)
    {
        _paths = paths;
        _reader = reader;
    }

    public IList<string> Paths => _paths;

    public override async Task<IList<object>> GetValuesAsync()
    {
        return await Task.Run(() => _paths.Select(path => _reader.Invoke(path)).ToList());
    }
}
