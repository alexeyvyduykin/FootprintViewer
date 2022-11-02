using Mapsui.Nts.Extensions;
using Mapsui.Projections;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO.ShapeFile.Extended;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FootprintViewer.Data;

public class FootprintPreviewGeometry
{
    public string? Name { get; set; }

    public Geometry? Geometry { get; set; }

    public static Func<IList<string>, IList<object>> Builder =>
        paths => paths.SelectMany(path =>
        {
            var list = new List<FootprintPreviewGeometry>();

            var shp = new ShapeDataReader(path);

            foreach (var shapefileFeature in shp.ReadByMBRFilter(shp.ShapefileBounds))
            {
                var obj = shapefileFeature.Attributes["LABEL"];

                if (obj is string name)
                {
                    var geometry = shapefileFeature.Geometry;

                    var points = geometry.Coordinates;

                    var vertices = points.Select(s => SphericalMercator.FromLonLat(s.X, s.Y).ToCoordinate()).ToArray();

                    var poly = new GeometryFactory().CreatePolygon(vertices.ToClosedCoordinates());

                    list.Add(new FootprintPreviewGeometry()
                    {
                        Name = name,
                        Geometry = poly
                    });
                }
            }

            return list;
        }).ToList<object>();
}
