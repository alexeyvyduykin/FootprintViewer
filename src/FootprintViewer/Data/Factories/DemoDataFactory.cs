using FootprintViewer.Data.Sources;
using FootprintViewer.FileSystem;

namespace FootprintViewer.Data
{
    public class DemoDataFactory : BaseDataFactory, IDataFactory
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
                    GenerateCount = 5,
                },
            };
        }

        protected override IDataSource[] GetFootprintSources()
        {
            return new[]
            {
                new RandomSource()
                {
                    Name = "RandomFootprints",
                    GenerateCount = 2000,
                },
            };
        }

        protected override IDataSource[] GetGroundTargetSources()
        {
            return new[]
            {
                new RandomSource()
                {
                    Name = "RandomGroundTargets",
                    GenerateCount = 5000,
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
                   GenerateCount = 6,
               },
            };
        }
    }
}
