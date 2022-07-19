using FootprintViewer.Data.Sources;
using FootprintViewer.FileSystem;
using NetTopologySuite.Geometries;

namespace FootprintViewer.Data
{
    public class DevWorkDataFactory : BaseDataFactory, IDataFactory
    {
        protected override IDataSource<FootprintPreview>[] GetFootprintPreviewSources()
        {
            var directory1 = System.IO.Path.Combine(new SolutionFolder("data").FolderDirectory, "footprints");
            var directory2 = System.IO.Path.Combine(new SolutionFolder("userData").FolderDirectory, "footprints");

            return new[]
            {
                new FootprintPreviewDataSource()
                {                
                    Directory = directory1,                
                    SearchPattern = "*.mbtiles",
                },
                new FootprintPreviewDataSource()
                {               
                    Directory = directory2,                      
                    SearchPattern = "*.mbtiles",
                },
            };
        }

        protected override IDataSource<MapResource>[] GetMapBackgroundSources()
        {
            var directory1 = System.IO.Path.Combine(new SolutionFolder("data").FolderDirectory, "world");
            var directory2 = System.IO.Path.Combine(new SolutionFolder("userData").FolderDirectory, "world");

            return new[]
            {
                new MapDataSource()                
                {
                    Directory = directory1,
                    SearchPattern = "*.mbtiles",
                },
                new MapDataSource()                
                {
                    Directory = directory2,
                    SearchPattern = "*.mbtiles",
                },
            };
        }

        protected override IDataSource<(string, Geometry)>[] GetFootprintPreviewGeometrySources()
        {
            var folder = new SolutionFolder("data");
            var path = folder.GetPath("mosaic-tiff-ruonly.shp", "mosaics-geotiff") ?? string.Empty;

            return new[]
            {
                new FootprintPreviewGeometryDataSource()
                {                
                    Path = path,
                },
            };
        }

        protected override IDataSource<UserGeometry>[] GetUserGeometrySources()
        {
            return new[]
            {
                new UserGeometryDataSource()
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

        protected override IDataSource<Satellite>[] GetSatelliteSources()
        {
            return new[]
            {
                new SatelliteDataSource()
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

        protected override IDataSource<Footprint>[] GetFootprintSources()
        {
            return new[]
            {
                new FootprintDataSource()
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

        protected override IDataSource<GroundTarget>[] GetGroundTargetSources()
        {
            return new[]
            {
                new GroundTargetDataSource()
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

        protected override IDataSource<GroundStation>[] GetGroundStationSources()
        {
            return new[]
            {
                new GroundStationDataSource()
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
}
