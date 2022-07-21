using Mapsui.Nts.Extensions;
using Mapsui.Projections;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.Data.Managers
{
    public class FootprintPreviewGeometryDataManager : BaseDataManager<(string, Geometry), IFileSource>
    {
        protected override async Task<List<(string, Geometry)>> GetNativeValuesAsync(IFileSource dataSource, IFilter<(string, Geometry)>? filter)
        {
            return await Task.Run(() =>
            {
                var shp = new NetTopologySuite.IO.ShapeFile.Extended.ShapeDataReader(dataSource.Path);

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

        protected override Task<List<T>> GetValuesAsync<T>(IFileSource dataSource, IFilter<T>? filter, Func<(string, Geometry), T> converter) => throw new Exception();
    }
}
