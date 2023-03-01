using FootprintViewer.Data;
using FootprintViewer.Data.DbContexts;
using FootprintViewer.Data.Sources;
using FootprintViewer.FileSystem;
using System.Collections.Generic;
using System.Linq;

namespace FootprintViewer.Factories;

public class ReleaseDataFactory : IDataFactory
{
    public IDataManager CreateDataManager()
    {
        // maps
        var mapsKey = DbKeys.Maps.ToString();
        var directory = System.IO.Path.Combine(new SolutionFolder("data").FolderDirectory, "world");

        var paths = System.IO.Directory.GetFiles(directory, "*.mbtiles").Select(System.IO.Path.GetFullPath).ToList();
        var mapSource = new FileSource(mapsKey, paths);

        var sources = new Dictionary<string, IList<ISource>>()
        {
            { mapsKey, new[] { mapSource } }
        };

        return new DataManager(sources);
    }
}
