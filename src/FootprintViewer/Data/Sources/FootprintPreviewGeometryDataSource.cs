using FootprintViewer.FileSystem;
using FootprintViewer.ViewModels;
using Mapsui.Nts.Extensions;
using Mapsui.Projections;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.Data.Sources
{
    public class FootprintPreviewGeometryDataSource : IDataSource<(string, Geometry)>
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

        public async Task<List<(string, Geometry)>> GetValuesAsync(IFilter<(string, Geometry)>? filter = null)
        {
            return await Task.Run(() =>
            {
                var shapeFileName = _dataFolder.GetPath(_file, _subFolder);

                var shp = new NetTopologySuite.IO.ShapeFile.Extended.ShapeDataReader(shapeFileName);

                var list = new List<(string, Geometry)>();

                foreach (var shapefileFeature in shp.ReadByMBRFilter(shp.ShapefileBounds))
                {
                    var obj = shapefileFeature.Attributes["LABEL"];

                    if (obj is string name)
                    {
                        var geometry = shapefileFeature.Geometry;

                        var points = geometry.Coordinates;

                        var vertices = points.Select(s => SphericalMercator.FromLonLat(s.X, s.Y).ToCoordinate()).ToArray();

                        var poly = new GeometryFactory().CreatePolygon(vertices.ToClosedCoordinates());

                        list.Add((name, poly!));
                    }
                }

                return list;
            });
        }
    }
}
