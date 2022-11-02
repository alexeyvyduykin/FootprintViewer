using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FootprintViewer.Data.DataManager.Sources;

public class FileSource : BaseSource
{
    private readonly IList<string> _paths;

    public FileSource(IList<string> paths)
    {
        _paths = paths;
    }

    public Func<IList<string>, IList<object>>? Loader { get; set; }

    public override async Task<IList<object>> GetValuesAsync()
    {
        return await Task.Run(() => Loader?.Invoke(_paths) ?? new List<object>());
    }
}
