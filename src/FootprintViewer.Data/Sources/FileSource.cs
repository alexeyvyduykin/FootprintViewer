namespace FootprintViewer.Data.Sources;

public class FileSource : BaseSource
{
    private readonly IList<string> _paths;
    private readonly Func<string, object> _builder;

    public FileSource(IList<string> paths, Func<string, object> builder)
    {
        _paths = new List<string>(paths);
        _builder = builder;
    }

    public IList<string> Paths => _paths;

    public override async Task<IList<object>> GetValuesAsync()
    {
        return await Task.Run(() => _paths.Select(path => _builder.Invoke(path)).ToList());
    }
}
