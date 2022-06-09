using FootprintViewer.ViewModels;

namespace FootprintViewer.Designer
{
    public class DesignTimeAppSettings : AppSettings
    {
        public DesignTimeAppSettings() : base()
        {
            FootprintSources.Add(new DatabaseSourceInfo()
            {
                Version = "14.1",
                Host = "localhost",
                Port = 5432,
                Database = "FootprintViewerDatabase",
                Username = "postgres",
                Password = "user",
                Table = "Footprints"
            });

            GroundTargetSources.Add(new DatabaseSourceInfo()
            {
                Version = "14.1",
                Host = "localhost",
                Port = 5432,
                Database = "FootprintViewerDatabase",
                Username = "postgres",
                Password = "user",
                Table = "GroundTargets"
            });

            GroundStationSources.Add(new DatabaseSourceInfo()
            {
                Version = "14.1",
                Host = "localhost",
                Port = 5432,
                Database = "FootprintViewerDatabase",
                Username = "postgres",
                Password = "user",
                Table = "GroundStations"
            });

            SatelliteSources.Add(new DatabaseSourceInfo()
            {
                Version = "14.1",
                Host = "localhost",
                Port = 5432,
                Database = "FootprintViewerDatabase",
                Username = "postgres",
                Password = "user",
                Table = "Satellites"
            });

            UserGeometrySources.Add(new DatabaseSourceInfo()
            {
                Version = "14.1",
                Host = "localhost",
                Port = 5432,
                Database = "FootprintViewerDatabase",
                Username = "postgres",
                Password = "user",
                Table = "UserGeometries"
            });

            FootprintPreviewGeometrySources.Add(new FileSourceInfo()
            {
                Path = @"C:\Users\User\AlexeyVyduykin\CSharpProjects\FootprintViewer\data\mosaics-geotiff\mosaic-tiff-ruonly.shp",
            });

            MapBackgroundSources.Add(new FolderSourceInfo()
            {
                Directory = @"C:\Users\User\AlexeyVyduykin\CSharpProjects\FootprintViewer\data\world",
                SearchPattern = "*.mbtiles",
            });

            MapBackgroundSources.Add(new FolderSourceInfo()
            {
                Directory = @"C:\Users\User\AlexeyVyduykin\CSharpProjects\FootprintViewer\userData\world",
                SearchPattern = "*.mbtiles",
            });

            FootprintPreviewSources.Add(new FolderSourceInfo()
            {
                Directory = @"C:\Users\User\AlexeyVyduykin\CSharpProjects\FootprintViewer\data\footprints",
                SearchPattern = "*.mbtiles",
            });

            FootprintPreviewSources.Add(new FolderSourceInfo()
            {
                Directory = @"C:\Users\User\AlexeyVyduykin\CSharpProjects\FootprintViewer\userData\footprints",
                SearchPattern = "*.mbtiles",
            });
        }
    }
}
