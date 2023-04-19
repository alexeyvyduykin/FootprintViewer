using FootprintViewer.Data;
using FootprintViewer.Data.DbContexts;
using FootprintViewer.Data.Sources;
using FootprintViewer.FileSystem;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FootprintViewer.Factories;

public class DevWorkDataFactory : IDataFactory
{
    public IDataManager CreateDataManager()
    {
        var connectionString = DbHelper.ToConnectionString("localhost", 5432, "FootprintViewerDatabase", "postgres", "user");

        // userGeometries
        var userGeometriesKey = DbKeys.UserGeometries.ToString();
        var userGeometriesSource = new EditableDatabaseSource(userGeometriesKey, connectionString, "UserGeometries");

        // plannedSchedules
        var plannedSchedulesKey = DbKeys.PlannedSchedules.ToString();
        var plannedSchedulesSource = new DatabaseSource(plannedSchedulesKey, connectionString, "PlannedSchedules");

        // maps
        var mapsKey = DbKeys.Maps.ToString();
        var directory1 = Path.Combine(new SolutionFolder("data").FolderDirectory, "world");
        var directory2 = Path.Combine(new SolutionFolder("userData").FolderDirectory, "world");

        var paths1 = Directory.GetFiles(directory1, "*.mbtiles").Select(Path.GetFullPath).ToList();
        var paths2 = Directory.GetFiles(directory2, "*.mbtiles").Select(Path.GetFullPath).ToList();
        var mapSource1 = new FileSource(mapsKey, paths1);
        var mapSource2 = new FileSource(mapsKey, paths2);

        // footprintPreviews
        var footprintPreviewsKey = DbKeys.FootprintPreviews.ToString();
        var directory3 = Path.Combine(new SolutionFolder("data").FolderDirectory, "footprints");
        var directory4 = Path.Combine(new SolutionFolder("userData").FolderDirectory, "footprints");

        var paths3 = Directory.GetFiles(directory3, "*.mbtiles").Select(Path.GetFullPath).ToList();
        var paths4 = Directory.GetFiles(directory4, "*.mbtiles").Select(Path.GetFullPath).ToList();
        var mapSource3 = new FileSource(footprintPreviewsKey, paths3);
        var mapSource4 = new FileSource(footprintPreviewsKey, paths4);

        // footprintPreviewGeometries
        var footprintPreviewGeometriesKey = DbKeys.FootprintPreviewGeometries.ToString();
        var path5 = new SolutionFolder("data").GetPath("mosaic-tiff-ruonly.shp", "mosaics-geotiff") ?? string.Empty;
        var mapSource5 = new FileSource(footprintPreviewGeometriesKey, new List<string>() { path5 });

        var sources = new Dictionary<string, IList<ISource>>()
        {
            { userGeometriesKey, new[] { userGeometriesSource } },
            { mapsKey, new[] { mapSource1, mapSource2 } },
            { footprintPreviewsKey, new[] { mapSource3, mapSource4 } },
            { footprintPreviewGeometriesKey, new[] { mapSource5 } },
            { plannedSchedulesKey, new[] { plannedSchedulesSource } }
        };

        return new DataManager(sources);
    }
}
