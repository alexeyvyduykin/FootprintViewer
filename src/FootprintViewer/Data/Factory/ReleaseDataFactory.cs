using System;
using FootprintViewer.Data.Sources;
using FootprintViewer.FileSystem;
using NetTopologySuite.Geometries;

namespace FootprintViewer.Data
{
    public class ReleaseDataFactory : BaseDataFactory, IDataFactory
    {
        protected override IDataSource<FootprintPreview>[] GetFootprintPreviewSources()
        {
            return Array.Empty<IDataSource<FootprintPreview>>();
        }

        protected override IDataSource<MapResource>[] GetMapBackgroundSources()
        {
            var directory1 = System.IO.Path.Combine(new SolutionFolder("data").FolderDirectory, "world");

            return new[]
            {
                new MapDataSource(directory1, "*.mbtiles"),
            };
        }

        protected override IDataSource<(string, Geometry)>[] GetFootprintPreviewGeometrySources()
        {
            return Array.Empty<IDataSource<(string, Geometry)>>();
        }

        protected override IDataSource<UserGeometry>[] GetUserGeometrySources()
        {
            return Array.Empty<IDataSource<UserGeometry>>();
        }

        protected override IDataSource<Satellite>[] GetSatelliteSources()
        {
            return Array.Empty<IDataSource<Satellite>>();
        }

        protected override IDataSource<Footprint>[] GetFootprintSources()
        {
            return Array.Empty<IDataSource<Footprint>>();
        }

        protected override IDataSource<GroundTarget>[] GetGroundTargetSources()
        {
            return Array.Empty<IDataSource<GroundTarget>>();
        }

        protected override IDataSource<GroundStation>[] GetGroundStationSources()
        {
            return Array.Empty<IDataSource<GroundStation>>();
        }
    }
}
