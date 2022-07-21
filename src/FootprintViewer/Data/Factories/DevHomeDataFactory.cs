using FootprintViewer.Data.Sources;
using FootprintViewer.FileSystem;

namespace FootprintViewer.Data
{
    public class DevHomeDataFactory : BaseDataFactory, IDataFactory
    {
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

        protected override IDataSource[] GetSatelliteSources()
        {
            return new[]
            {
                new RandomSource()
                {
                    Name = "RandomSatellites",
                },
            };
        }

        protected override IDataSource[] GetFootprintSources()
        {
            // TODO: temporary solution, all random sources not chaining
            //       var satelliteDataSource = new RandomSatelliteDataSource();

            return new[]
            {
                new RandomSource(/*satelliteDataSource*/)
                {
                    Name = "RandomFootprints",
                },
            };
        }

        protected override IDataSource[] GetGroundTargetSources()
        {
            // TODO: temporary solution, all random sources not chaining
            //      var satelliteDataSource = new RandomSatelliteDataSource();
            //      var footprintDataSource = new RandomFootprintDataSource(satelliteDataSource);

            return new[]
            {
                new RandomSource(/*footprintDataSource*/)
                {
                    Name = "RandomGroundTargets",
                },
            };
        }

        protected override IDataSource[] GetGroundStationSources()
        {
            return new[]
            {
               new RandomSource()
               {
                   Name = "RandomGroundStations",
               },
            };
        }
    }
}
