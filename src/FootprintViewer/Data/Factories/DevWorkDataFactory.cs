using FootprintViewer.Data.DataManager;
using FootprintViewer.FileSystem;
using System.Collections.Generic;
using System.Linq;

namespace FootprintViewer.Data;

public class DevWorkDataFactory : IDataFactory
{
    public FootprintViewer.Data.DataManager.IDataManager CreateDataManager()
    {
        var dataManager = new DataManager.DataManager();

        var connectionString = DbHelper.ToConnectionString("localhost", 5432, "FootprintViewerDatabase", "postgres", "user");

        // footprints
        var footprintsKey = DbKeys.Footprints.ToString();
        var footprintSource = new FootprintViewer.Data.DataManager.Sources.DatabaseSource(footprintsKey, connectionString, "Footprints");
        dataManager.RegisterSource(footprintsKey, footprintSource);

        // groundTargets
        var groundTargetsKey = DbKeys.GroundTargets.ToString();
        var groundTargetsSource = new FootprintViewer.Data.DataManager.Sources.DatabaseSource(groundTargetsKey, connectionString, "GroundTargets");
        dataManager.RegisterSource(groundTargetsKey, groundTargetsSource);

        // satellites
        var satellitesKey = DbKeys.Satellites.ToString();
        var satellitesSource = new FootprintViewer.Data.DataManager.Sources.DatabaseSource(satellitesKey, connectionString, "Satellites");
        dataManager.RegisterSource(satellitesKey, satellitesSource);

        // groundStations
        var groundStationsKey = DbKeys.GroundStations.ToString();
        var groundStationsSource = new FootprintViewer.Data.DataManager.Sources.DatabaseSource(groundStationsKey, connectionString, "GroundStations");
        dataManager.RegisterSource(groundStationsKey, groundStationsSource);

        // userGeometries
        var userGeometriesKey = DbKeys.UserGeometries.ToString();
        var userGeometriesSource = new FootprintViewer.Data.DataManager.Sources.EditableDatabaseSource(userGeometriesKey, connectionString, "UserGeometries");
        dataManager.RegisterSource(userGeometriesKey, userGeometriesSource);

        // maps
        var mapsKey = DbKeys.Maps.ToString();
        var directory1 = System.IO.Path.Combine(new SolutionFolder("data").FolderDirectory, "world");
        var directory2 = System.IO.Path.Combine(new SolutionFolder("userData").FolderDirectory, "world");

        var paths1 = System.IO.Directory.GetFiles(directory1, "*.mbtiles").Select(System.IO.Path.GetFullPath).ToList();
        var paths2 = System.IO.Directory.GetFiles(directory2, "*.mbtiles").Select(System.IO.Path.GetFullPath).ToList();
        var mapSource1 = new FootprintViewer.Data.DataManager.Sources.FileSource(paths1)
        {
            Loader = MapResource.Builder
        };
        var mapSource2 = new FootprintViewer.Data.DataManager.Sources.FileSource(paths2)
        {
            Loader = MapResource.Builder
        };
        dataManager.RegisterSource(mapsKey, mapSource1);
        dataManager.RegisterSource(mapsKey, mapSource2);

        // footprintPreviews
        var footprintPreviewsKey = DbKeys.FootprintPreviews.ToString();
        var directory3 = System.IO.Path.Combine(new SolutionFolder("data").FolderDirectory, "footprints");
        var directory4 = System.IO.Path.Combine(new SolutionFolder("userData").FolderDirectory, "footprints");

        var paths3 = System.IO.Directory.GetFiles(directory3, "*.mbtiles").Select(System.IO.Path.GetFullPath).ToList();
        var paths4 = System.IO.Directory.GetFiles(directory4, "*.mbtiles").Select(System.IO.Path.GetFullPath).ToList();
        var mapSource3 = new FootprintViewer.Data.DataManager.Sources.FileSource(paths3)
        {
            Loader = FootprintPreview.Builder
        };
        var mapSource4 = new FootprintViewer.Data.DataManager.Sources.FileSource(paths4)
        {
            Loader = FootprintPreview.Builder
        };
        dataManager.RegisterSource(footprintPreviewsKey, mapSource3);
        dataManager.RegisterSource(footprintPreviewsKey, mapSource4);

        // footprintPreviewGeometries
        var footprintPreviewGeometriesKey = DbKeys.FootprintPreviewGeometries.ToString();
        var path5 = new SolutionFolder("data").GetPath("mosaic-tiff-ruonly.shp", "mosaics-geotiff") ?? string.Empty;
        var mapSource5 = new FootprintViewer.Data.DataManager.Sources.FileSource(new List<string>() { path5 })
        {
            Loader = FootprintPreviewGeometry.Builder
        };
        dataManager.RegisterSource(footprintPreviewGeometriesKey, mapSource5);

        return dataManager;
    }
}
