using FootprintViewer.Data.DataManager;
using FootprintViewer.Data.DataManager.Sources;
using FootprintViewer.FileSystem;
using System.Collections.Generic;
using System.Linq;

namespace FootprintViewer.Data;

public class ReleaseDataFactory : IDataFactory
{
    public IDataManager CreateDataManager()
    {
        // maps
        var mapsKey = DbKeys.Maps.ToString();
        var directory = System.IO.Path.Combine(new SolutionFolder("data").FolderDirectory, "world");

        var paths = System.IO.Directory.GetFiles(directory, "*.mbtiles").Select(System.IO.Path.GetFullPath).ToList();
        var mapSource = new FileSource(paths)
        {
            Loader = MapResource.Builder
        };

        var sources = new Dictionary<string, IList<ISource>>()
        {
            { mapsKey, new[] { mapSource } }
        };

        return new DataManager.DataManager(sources);
    }
}
