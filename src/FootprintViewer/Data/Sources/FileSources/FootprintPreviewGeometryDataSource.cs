using FootprintViewer.ViewModels;
using Mapsui.Nts.Extensions;
using Mapsui.Projections;
using NetTopologySuite.Geometries;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.Data.Sources
{
    public class FootprintPreviewGeometryDataSource : IDataSource<(string, Geometry)>
    {
        private readonly string? _path;

        public FootprintPreviewGeometryDataSource(string? path)
        {
            _path = path;
        }

        public async Task<List<(string, Geometry)>> GetValuesAsync(IFilter<(string, Geometry)>? filter = null)
        {
            return await Task.Run(() =>
            {
                var shp = new NetTopologySuite.IO.ShapeFile.Extended.ShapeDataReader(_path);

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
