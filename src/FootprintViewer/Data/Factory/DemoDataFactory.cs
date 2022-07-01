using FootprintViewer.Data.Sources;
using FootprintViewer.FileSystem;
using NetTopologySuite.Geometries;

namespace FootprintViewer.Data
{
    public class DemoDataFactory : BaseDataFactory, IDataFactory
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

        protected override IDataSource<Satellite>[] GetSatelliteSources()
        {
            return new[]
            {
                new RandomSatelliteDataSource(),
            };
        }

        protected override IDataSource<Footprint>[] GetFootprintSources()
        {
            // TODO: temporary solution, all random sources not chaining
            var satelliteDataSource = new RandomSatelliteDataSource();

            return new[]
            {
                new RandomFootprintDataSource(satelliteDataSource),
            };
        }

        protected override IDataSource<GroundTarget>[] GetGroundTargetSources()
        {
            // TODO: temporary solution, all random sources not chaining
            var satelliteDataSource = new RandomSatelliteDataSource();
            var footprintDataSource = new RandomFootprintDataSource(satelliteDataSource);

            return new[]
            {
                new RandomGroundTargetDataSource(footprintDataSource),
            };
        }

        protected override IDataSource<GroundStation>[] GetGroundStationSources()
        {
            return new[]
            {
               new RandomGroundStationDataSource(),
            };
        }
    }
}
