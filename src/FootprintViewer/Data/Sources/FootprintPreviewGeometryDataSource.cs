using FootprintViewer.FileSystem;
using Mapsui.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FootprintViewer.Data.Sources
{
    public class FootprintPreviewGeometryDataSource : IFootprintPreviewGeometryDataSource
    {
        private readonly SolutionFolder _dataFolder;
        private readonly string _file;
        private readonly string? _subFolder;

        public FootprintPreviewGeometryDataSource(string file, string folder, string? subFolder = null)
        {
            _file = file;

            if (file.Contains("*.") == true)
            {
                throw new Exception();
            }

            _subFolder = subFolder;

            _dataFolder = new SolutionFolder(folder);
        }

        public IDictionary<string, IGeometry> GetFootprintPreviewGeometries()
        {
            var dict = new SortedDictionary<string, IGeometry>();

            var shapeFileName = _dataFolder.GetPath(_file, _subFolder);

            var shp = new NetTopologySuite.IO.ShapeFile.Extended.ShapeDataReader(shapeFileName);

            foreach (var shapefileFeature in shp.ReadByMBRFilter(shp.ShapefileBounds))
            {
                var obj = shapefileFeature.Attributes["LABEL"];

                if (obj is string name)
                {
                    var geometry = shapefileFeature.Geometry;

                    var points = geometry.Coordinates;
                    var exteriorRing = new LinearRing(points.Select(s => Mapsui.Projection.SphericalMercator.FromLonLat(s.X, s.Y)));
                    var poly = new Polygon(exteriorRing);

                    dict.Add(name, poly);
                }
            }

            return dict;
        }
    }
}
