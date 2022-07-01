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
                new FootprintPreviewDataSource(directory1, "*.mbtiles"),
                new FootprintPreviewDataSource(directory2, "*.mbtiles"),
            };
        }

        protected override IDataSource<MapResource>[] GetMapBackgroundSources()
        {
            var directory1 = System.IO.Path.Combine(new SolutionFolder("data").FolderDirectory, "world");
            var directory2 = System.IO.Path.Combine(new SolutionFolder("userData").FolderDirectory, "world");

            return new[]
            {
                new MapDataSource(directory1, "*.mbtiles"),
                new MapDataSource(directory2, "*.mbtiles"),
            };
        }

        protected override IDataSource<(string, Geometry)>[] GetFootprintPreviewGeometrySources()
        {
            var folder = new SolutionFolder("data");
            var path = folder.GetPath("mosaic-tiff-ruonly.shp", "mosaics-geotiff");

            return new[]
            {
                new FootprintPreviewGeometryDataSource(path),
            };
        }

        protected override IDataSource<UserGeometry>[] GetUserGeometrySources()
        {
            var options = BuildDbContextOptions<UserGeometryDbContext>("14.1", "localhost", 5432, "FootprintViewerDatabase", "postgres", "user");

            return new[]
            {
                new UserGeometryDataSource(options, "UserGeometries"),
            };
        }

        protected override IDataSource<Satellite>[] GetSatelliteSources()
        {
            var options = BuildDbContextOptions<SatelliteDbContext>("14.1", "localhost", 5432, "FootprintViewerDatabase", "postgres", "user");

            return new[]
            {
                new SatelliteDataSource(options, "Satellites"),
            };
        }

        protected override IDataSource<Footprint>[] GetFootprintSources()
        {
            var options = BuildDbContextOptions<FootprintDbContext>("14.1", "localhost", 5432, "FootprintViewerDatabase", "postgres", "user");

            return new[]
            {
                new FootprintDataSource(options, "Footprints"),
            };
        }

        protected override IDataSource<GroundTarget>[] GetGroundTargetSources()
        {
            var options = BuildDbContextOptions<DbCustomContext>("14.1", "localhost", 5432, "FootprintViewerDatabase", "postgres", "user");

            return new[]
            {
                new GroundTargetDataSource(options, "GroundTargets"),
            };
        }

        protected override IDataSource<GroundStation>[] GetGroundStationSources()
        {
            var options = BuildDbContextOptions<DbCustomContext>("14.1", "localhost", 5432, "FootprintViewerDatabase", "postgres", "user");

            return new[]
            {
                new GroundStationDataSource(options, "GroundStations"),
            };
        }
    }
}
