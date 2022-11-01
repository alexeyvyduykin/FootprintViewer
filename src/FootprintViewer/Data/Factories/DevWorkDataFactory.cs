using FootprintViewer.Data.DataManager;
using FootprintViewer.Data.Sources;
using FootprintViewer.FileSystem;

namespace FootprintViewer.Data;

public class DevWorkDataFactory : BaseDataFactory, IDataFactory
{
    public override FootprintViewer.Data.DataManager.IDataManager CreateDataManager()
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
        var userGeometriesSource = new FootprintViewer.Data.DataManager.Sources.DatabaseSource(userGeometriesKey, connectionString, "UserGeometries");
        dataManager.RegisterSource(userGeometriesKey, userGeometriesSource);

        return dataManager;
    }

    protected override IDataSource[] GetFootprintPreviewSources()
    {
        var directory1 = System.IO.Path.Combine(new SolutionFolder("data").FolderDirectory, "footprints");
        var directory2 = System.IO.Path.Combine(new SolutionFolder("userData").FolderDirectory, "footprints");

        return new[]
        {
            new FolderSource()
            {
                Directory = directory1,
                SearchPattern = "*.mbtiles",
            },
            new FolderSource()
            {
                Directory = directory2,
                SearchPattern = "*.mbtiles",
            },
        };
    }

    protected override IDataSource[] GetMapBackgroundSources()
    {
        var directory1 = System.IO.Path.Combine(new SolutionFolder("data").FolderDirectory, "world");
        var directory2 = System.IO.Path.Combine(new SolutionFolder("userData").FolderDirectory, "world");

        return new[]
        {
            new FolderSource()
            {
                Directory = directory1,
                SearchPattern = "*.mbtiles",
            },
            new FolderSource()
            {
                Directory = directory2,
                SearchPattern = "*.mbtiles",
            },
        };
    }

    protected override IDataSource[] GetFootprintPreviewGeometrySources()
    {
        var folder = new SolutionFolder("data");
        var path = folder.GetPath("mosaic-tiff-ruonly.shp", "mosaics-geotiff") ?? string.Empty;

        return new[]
        {
            new FileSource()
            {
                Path = path,
            },
        };
    }

    protected override IDataSource[] GetUserGeometrySources()
    {
        return new[]
        {
            new DatabaseSource()
            {
                Version = "14.1",
                Host = "localhost",
                Port = 5432,
                Database = "FootprintViewerDatabase",
                Username = "postgres",
                Password = "user",
                Table = "UserGeometries"
            },
        };
    }

    protected override IDataSource[] GetSatelliteSources()
    {
        return new[]
        {
            new DatabaseSource()
            {
                Version = "14.1",
                Host = "localhost",
                Port = 5432,
                Database = "FootprintViewerDatabase",
                Username = "postgres",
                Password = "user",
                Table = "Satellites"
            },
        };
    }

    protected override IDataSource[] GetFootprintSources()
    {
        return new[]
        {
            new DatabaseSource()
            {
                Version = "14.1",
                Host = "localhost",
                Port = 5432,
                Database = "FootprintViewerDatabase",
                Username = "postgres",
                Password = "user",
                Table = "Footprints"
            },
        };
    }

    protected override IDataSource[] GetGroundTargetSources()
    {
        return new[]
        {
            new DatabaseSource()
            {
                Version = "14.1",
                Host = "localhost",
                Port = 5432,
                Database = "FootprintViewerDatabase",
                Username = "postgres",
                Password = "user",
                Table = "GroundTargets"
            },
        };
    }

    protected override IDataSource[] GetGroundStationSources()
    {
        return new[]
        {
            new DatabaseSource()
            {
                Version = "14.1",
                Host = "localhost",
                Port = 5432,
                Database = "FootprintViewerDatabase",
                Username = "postgres",
                Password = "user",
                Table = "GroundStations"
            },
        };
    }
}
