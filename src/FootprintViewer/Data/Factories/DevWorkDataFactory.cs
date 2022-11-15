using FootprintViewer.Data.DataManager;
using FootprintViewer.Data.DataManager.Sources;
using FootprintViewer.FileSystem;
using System.Collections.Generic;
using System.Linq;

namespace FootprintViewer.Data;

public class DevWorkDataFactory : IDataFactory
{
    public IDataManager CreateDataManager()
    {
        var connectionString = DbHelper.ToConnectionString("localhost", 5432, "FootprintViewerDatabase", "postgres", "user");

        // footprints
        var footprintsKey = DbKeys.Footprints.ToString();
        var footprintSource = new DatabaseSource(footprintsKey, connectionString, "Footprints");

        // groundTargets
        var groundTargetsKey = DbKeys.GroundTargets.ToString();
        var groundTargetsSource = new DatabaseSource(groundTargetsKey, connectionString, "GroundTargets");

        // satellites
        var satellitesKey = DbKeys.Satellites.ToString();
        var satellitesSource = new DatabaseSource(satellitesKey, connectionString, "Satellites");

        // groundStations
        var groundStationsKey = DbKeys.GroundStations.ToString();
        var groundStationsSource = new DatabaseSource(groundStationsKey, connectionString, "GroundStations");

        // userGeometries
        var userGeometriesKey = DbKeys.UserGeometries.ToString();
        var userGeometriesSource = new EditableDatabaseSource(userGeometriesKey, connectionString, "UserGeometries");

        // maps
        var mapsKey = DbKeys.Maps.ToString();
        var directory1 = System.IO.Path.Combine(new SolutionFolder("data").FolderDirectory, "world");
        var directory2 = System.IO.Path.Combine(new SolutionFolder("userData").FolderDirectory, "world");

        var paths1 = System.IO.Directory.GetFiles(directory1, "*.mbtiles").Select(System.IO.Path.GetFullPath).ToList();
        var paths2 = System.IO.Directory.GetFiles(directory2, "*.mbtiles").Select(System.IO.Path.GetFullPath).ToList();
        var mapSource1 = new FileSource(paths1)
        {
            Loader = MapResource.Builder
        };
        var mapSource2 = new FileSource(paths2)
        {
            Loader = MapResource.Builder
        };

        // footprintPreviews
        var footprintPreviewsKey = DbKeys.FootprintPreviews.ToString();
        var directory3 = System.IO.Path.Combine(new SolutionFolder("data").FolderDirectory, "footprints");
        var directory4 = System.IO.Path.Combine(new SolutionFolder("userData").FolderDirectory, "footprints");

        var paths3 = System.IO.Directory.GetFiles(directory3, "*.mbtiles").Select(System.IO.Path.GetFullPath).ToList();
        var paths4 = System.IO.Directory.GetFiles(directory4, "*.mbtiles").Select(System.IO.Path.GetFullPath).ToList();
        var mapSource3 = new FileSource(paths3)
        {
            Loader = FootprintPreview.Builder
        };
        var mapSource4 = new FileSource(paths4)
        {
            Loader = FootprintPreview.Builder
        };

        // footprintPreviewGeometries
        var footprintPreviewGeometriesKey = DbKeys.FootprintPreviewGeometries.ToString();
        var path5 = new SolutionFolder("data").GetPath("mosaic-tiff-ruonly.shp", "mosaics-geotiff") ?? string.Empty;
        var mapSource5 = new FileSource(new List<string>() { path5 })
        {
            Loader = FootprintPreviewGeometry.Builder
        };

        var sources = new Dictionary<string, IList<ISource>>()
        {
            { footprintsKey, new[] { footprintSource } },
            { groundTargetsKey, new[] { groundTargetsSource } },
            { satellitesKey, new[] { satellitesSource } },
            { groundStationsKey, new[] { groundStationsSource } },
            { userGeometriesKey, new[] { userGeometriesSource } },
            { mapsKey, new[] { mapSource1, mapSource2 } },
            { footprintPreviewsKey, new[] { mapSource3, mapSource4 } },
            { footprintPreviewGeometriesKey, new[] { mapSource5 } }
        };

        return new DataManager.DataManager(sources);
    }
}
