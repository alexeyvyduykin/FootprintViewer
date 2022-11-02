﻿using FootprintViewer.Data.DataManager;
using FootprintViewer.FileSystem;
using System.Linq;

namespace FootprintViewer.Data;

public class ReleaseDataFactory : IDataFactory
{
    public FootprintViewer.Data.DataManager.IDataManager CreateDataManager()
    {
        var dataManager = new DataManager.DataManager();

        // maps
        var mapsKey = DbKeys.Maps.ToString();
        var directory = System.IO.Path.Combine(new SolutionFolder("data").FolderDirectory, "world");

        var paths = System.IO.Directory.GetFiles(directory, "*.mbtiles").Select(System.IO.Path.GetFullPath).ToList();
        var mapSource = new FootprintViewer.Data.DataManager.Sources.FileSource(paths)
        {
            Loader = MapResource.Builder
        };
        dataManager.RegisterSource(mapsKey, mapSource);

        return dataManager;
    }
}
