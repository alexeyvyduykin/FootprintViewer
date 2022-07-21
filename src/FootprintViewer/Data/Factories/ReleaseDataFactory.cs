﻿using FootprintViewer.Data.Sources;
using FootprintViewer.FileSystem;
using System;

namespace FootprintViewer.Data
{
    public class ReleaseDataFactory : BaseDataFactory, IDataFactory
    {
        protected override IDataSource[] GetFootprintPreviewSources()
        {
            return Array.Empty<IDataSource>();
        }

        protected override IDataSource[] GetMapBackgroundSources()
        {
            var directory1 = System.IO.Path.Combine(new SolutionFolder("data").FolderDirectory, "world");

            return new[]
            {
                new FolderSource()
                {
                    Directory = directory1,
                    SearchPattern = "*.mbtiles",
                },
            };
        }

        protected override IDataSource[] GetFootprintPreviewGeometrySources()
        {
            return Array.Empty<IDataSource>();
        }

        protected override IDataSource[] GetUserGeometrySources()
        {
            return Array.Empty<IDataSource>();
        }

        protected override IDataSource[] GetSatelliteSources()
        {
            return Array.Empty<IDataSource>();
        }

        protected override IDataSource[] GetFootprintSources()
        {
            return Array.Empty<IDataSource>();
        }

        protected override IDataSource[] GetGroundTargetSources()
        {
            return Array.Empty<IDataSource>();
        }

        protected override IDataSource[] GetGroundStationSources()
        {
            return Array.Empty<IDataSource>();
        }
    }
}
